# TALib Architecture

**Purpose**  
This document describes the high-level architecture of **TALib** (Technical Analysis Library) and the rules that keep it consistent, performant, and maintainable.

**Audience**  
Human developers and AI agents working on the library. Read this together with `AGENTS.md` and `coding-standards.md` before changing code.

**Scope**  
Applies to the `TALib` class library and any consumers that compose its APIs (for example `ta_console`). Legacy patterns should be brought into compliance when touched.

---

## 1. Mission and Design Goals

TALib is a **high-performance, allocation-conscious technical analysis library** for .NET. It operates on ordered price/volume **series** (`IList<double>` and related list types) and produces indicator and transform series of the same length.

Primary goals, in priority order:

1. **Correctness** — Indicator math must be reliable and deterministic for given inputs.
2. **Performance** — Hot paths should minimize allocations, avoid unnecessary LINQ, and support buffer reuse.
3. **Composability** — Indicators build on simpler primitives (for example MACD uses EMA; volatility uses ROC + log + stddev).
4. **Usability** — Call sites can use static classes (`Overlays.sma(...)`) or fluent extension methods (`series.sma(...)`).
5. **Maintainability** — One clear pattern for every indicator so new functions look like existing ones.

Non-goals:

- UI, charting, or broker integration
- Dependency injection or application lifecycle management
- I/O, networking, or persistence
- External numerical packages (use BCL only unless explicitly approved)

---

## 2. Solution Layout

```
TechnicalIndicators/
├── TALib/                 # Core library (class library, net8.0+)
│   ├── ListUtil.cs        # Infrastructure: pooling, slicing, list helpers
│   ├── Common.cs          # Shared constants / converters
│   ├── Basic.cs           # Element-wise math and unary transforms
│   ├── Overlays.cs        # Moving averages, support/resistance overlays
│   ├── Oscillators.cs     # Momentum, volatility, multi-series oscillators
│   ├── Conditions.cs      # Boolean filters and generic pointwise calc
│   └── TALib.csproj
├── ta_console/            # Optional host / scratch consumer
└── .grok/rules/           # Agent and team standards (this file lives here)
```

### Project roles

| Project       | Role                                                                 |
|---------------|----------------------------------------------------------------------|
| **TALib**     | Pure computational library. No console entry point. No I/O.          |
| **ta_console**| Thin executable host for experiments, demos, or manual verification. |

**Rule:** Put all reusable indicator and series logic in `TALib`. Keep host-specific code out of the library.

---

## 3. Architectural Style

TALib is a **static functional core** over **mutable list buffers**, not a service-oriented or OOP domain model.

| Characteristic              | Approach in TALib                                      |
|----------------------------|--------------------------------------------------------|
| API surface                | `static` classes + `partial` extension methods         |
| State                      | Stateless functions; state lives in caller-owned lists |
| Primary data type          | `IList<double>` (and generic `IList<T>` where needed)  |
| Composition                | Call lower-level indicators; dispose intermediates     |
| Allocation strategy        | Optional output buffer + `ListPool<T>`                 |
| Dependency injection       | Not used (and not required)                            |
| Threading model            | Functions are re-entrant if callers do not share buffers unsafely; pool is concurrent |

Think of TALib as a **numerical toolbox for series**, similar in spirit to vectorized array libraries, specialized for technical analysis.

---

## 4. Logical Layers

Dependencies flow **downward only**. Higher layers may call lower layers; reverse calls are forbidden.

```
┌─────────────────────────────────────────────────────────────┐
│  Consumers (ta_console, strategies, apps)                   │
└────────────────────────────┬────────────────────────────────┘
                             │
┌────────────────────────────▼────────────────────────────────┐
│  Indicator / Domain APIs                                    │
│  Overlays · Oscillators · Conditions · Basic                │
└────────────────────────────┬────────────────────────────────┘
                             │
┌────────────────────────────▼────────────────────────────────┐
│  Infrastructure                                             │
│  ListPool · ListPoolItem · Slicer · ListExtention           │
│  Shared helpers / constants (e.g. VolatilityConverter)      │
└─────────────────────────────────────────────────────────────┘
```

### 4.1 Infrastructure layer (`ListUtil.cs`, parts of `Common.cs`)

Responsibilities:

- **List pooling** (`ListPool<T>`, `ListPoolItem<T>`) for reusable output and intermediate buffers
- **Slicing** (`Slicer<T>`) for windowed views without copying
- **List helpers** (`AdjustLength`, `Slice`, `Dispose` extensions)
- **Shared conversion constants** (for example volatility annualization factors)

Rules:

- Infrastructure must remain domain-agnostic (no SMA/RSI knowledge).
- Pooling must stay thread-safe for concurrent `Get` / `Return`.
- Do not introduce third-party collections or memory packages without explicit approval.

### 4.2 Domain API layer

Organized by **indicator family**, each in its own static class file:

| Class          | Responsibility                                                                 |
|----------------|--------------------------------------------------------------------------------|
| `Basic`        | Element-wise arithmetic and math (`add`, `sub`, `mul`, `div`, `log`, `pow`, …) |
| `Overlays`     | Price overlays and smoothers (`sma`, `ema`, `offset`, `rpt`, `spt`)            |
| `Oscillators`  | Momentum / volatility / multi-input indicators (`roc`, `macd`, `wilders_rsi`, …) |
| `Conditions`   | Boolean series, filters, and generic pointwise `Calc`                           |

Each domain file also hosts matching members of the **`partial` class `ListDoubleExtensions`** (or related extension surface) so callers can write fluent pipelines.

Rules:

- One conceptual family per static class / file.
- Prefer extending an existing family over inventing a new top-level class unless the new concept is clearly distinct.
- Domain methods may call other domain methods and infrastructure; they must not reach into consumers.

---

## 5. Core Design Patterns

### 5.1 Dual API: static + extensions

Every public indicator should be available in both forms:

```csharp
// Static
var sma = Overlays.sma(close, 20);

// Extension (preferred at call sites for chaining)
var sma = close.sma(20);
```

**Rules:**

- Implementation lives once in the static class (`Overlays`, `Oscillators`, etc.).
- Extension methods are thin wrappers that forward to the static implementation.
- Extensions belong on the appropriate `partial` extension class (today primarily `ListDoubleExtensions`).
- Do not duplicate algorithm logic in the extension method.

### 5.2 Optional output buffer (`ret` parameter)

Canonical signature shape:

```csharp
static public IList<double> IndicatorName(
    IList<double> series,
    /* parameters */,
    IList<double> ret = null)
```

Contract:

1. If `ret` is `null`, obtain a buffer from `ListPool<T>.Get()` (or allocate only when pooling is inappropriate — default is pool).
2. Call `ret.AdjustLength(series.Count)` (or the correct output length for multi-series rules).
3. Write results into `ret` and **return `ret`**.
4. Never assume `ret` is empty; always size and overwrite intentionally.
5. Do not mutate input series unless the method is explicitly documented as in-place (default is **not** in-place on inputs).

This enables zero-allocation reuse in tight loops:

```csharp
var buffer = ListPool<double>.Get();
try
{
    for (/* many symbols */)
    {
        series.sma(20, buffer);
        // consume buffer...
    }
}
finally
{
    buffer.Dispose();
}
```

### 5.3 List pooling and disposal

`ListPoolItem<T>` extends `List<T>` and implements `IDisposable`. Returning a buffer to the pool is done via `Dispose()` / `ListPool<T>.Return`.

**Rules for intermediate buffers:**

- Any temporary series obtained from the pool **must** be disposed before the method returns (including on early-exit paths that already allocated).
- Prefer `try`/`finally` or carefully ordered dispose after successful use (match existing style in `Oscillators` for consistency until a wider cleanup).
- Callers who receive a pooled list from an indicator **own** it and must dispose it when done (unless they passed their own `ret` buffer).
- Never dispose a caller-supplied `ret` buffer inside an indicator.

**Rules for composition:**

```csharp
// Good: dispose intermediates, write final into ret
var shortEma = series.ema(periodShort);
var longEma = series.ema(periodLong);
shortEma.sub(longEma, ret);
shortEma.Dispose();
longEma.Dispose();
return ret;
```

### 5.4 Equal-length series contract

Most multi-series operations require equal lengths (for example `add`, `obv`, multi-input conditions).

**Rules:**

- Validate length equality up front with a clear exception message including both lengths.
- Prefer failing fast over silently truncating or padding.
- Output length equals the primary input series length unless a function is explicitly documented otherwise.

### 5.5 Warm-up / leading zeros convention

Many period-based indicators cannot produce a valid value until enough history exists.

**Default convention in this library:**

- Output series has the **same length** as the input.
- Indices before the first valid observation are filled with **`0.0`** (or the documented default).
- Document any deviation (for example first RSI at index `period`).

**Period semantics (library convention):**

- Documented sliding-window meaning: a period of `n` means look back `n` bars relative to the current index (see comment in `Common.cs`).
- Validate `period` against series length before computing.
- Use specific exceptions with parameter names and values in the message.

When adding or changing indicators, **preserve existing period indexing conventions** for that function family so historical consumers do not silently change meaning. If a convention must change, treat it as a breaking change and call it out explicitly.

### 5.6 Slicing without copying

`Slicer<T>` provides an `IList<T>` view over a sub-range of a base list.

**Rules:**

- Use slices for windowed access when avoiding allocation matters.
- Slices must not support structural mutation (`Add` / `Remove` / `Insert` / `Clear` throw).
- Element assignment may write through to the underlying list; treat that as intentional and document call-site ownership carefully.

---

## 6. Module Placement Rules

When adding a new function, place it as follows:

| Kind of function                                      | Place in          |
|-------------------------------------------------------|-------------------|
| Element-wise math / scalar transforms                 | `Basic`           |
| Smoothing overlays, lags, support/resistance traces   | `Overlays`        |
| Momentum, volatility, oscillators, multi-price TA     | `Oscillators`     |
| Boolean predicates, filters, generic pointwise maps   | `Conditions`      |
| Pooling, slicing, list shape utilities                | `ListUtil`        |
| Shared pure constants / converters                    | `Common`          |

**Rules:**

- Do not create a new top-level static class for a single method if it fits an existing family.
- Keep infrastructure free of indicator formulas.
- Keep indicator files free of new allocation infrastructure (use `ListPool` / helpers instead).
- Prefer smaller, focused methods that compose rather than one mega-method that reimplements lower-level math.

---

## 7. Function Design Checklist (Required for New Indicators)

Every new public indicator or transform **must**:

1. **Live in the correct static class** (see §6).
2. **Implement once** in the static class; add a **thin extension** wrapper.
3. **Accept optional `ret`** and return the result buffer.
4. **Validate inputs early** (nulls where appropriate, period bounds, equal lengths).
5. **Size output** with `AdjustLength` to the correct length.
6. **Follow warm-up convention** (leading zeros or documented first valid index).
7. **Dispose pooled intermediates** it creates.
8. **Not dispose** caller-owned `ret`.
9. **Avoid LINQ** on hot paths (`Where`, `Select`, `ToList`, etc.) unless there is a compelling readability win outside hot paths.
10. **Stay allocation-light**: reuse buffers; no per-bar heap allocations in the main loop.
11. **Remain free of I/O and DI**.
12. **Include XML docs** on public APIs when appropriate (especially non-obvious formulas, period meaning, and warm-up behavior).

### Recommended skeleton

```csharp
static public IList<double> my_indicator(IList<double> series, int period, IList<double> ret = null)
{
    if (period < 0)
    {
        throw new ArgumentOutOfRangeException(nameof(period), period, "Period cannot be less than 0.");
    }
    if (period >= series.Count)
    {
        throw new ArgumentException(
            $"Period ({period}) cannot be greater than or equal to series length ({series.Count}).",
            nameof(period));
    }

    if (ret == null)
    {
        ret = ListPool<double>.Get();
    }
    ret.AdjustLength(series.Count);

    // compute into ret...

    return ret;
}
```

Then add:

```csharp
static public IList<double> my_indicator(this IList<double> series, int period, IList<double> ret = null)
{
    return Oscillators.my_indicator(series, period, ret); // or appropriate static class
}
```

---

## 8. Performance Principles

### 8.1 Hot-path rules

- Prefer simple `for` loops with index access over iterators/LINQ for series math.
- Prefer single-pass or few-pass algorithms; avoid O(n²) unless inherent (and document cost).
- Reuse rolling sums / recursive forms (for example SMA rolling window, Wilder smoothing) instead of recomputing full windows when possible.
- Avoid boxing and avoid capturing lambdas in inner loops on performance-critical paths.
- Prefer `IList<T>` indexers over repeated enumeration.

### 8.2 Allocation rules

- Default output allocation path is **`ListPool<T>.Get()`**.
- Callers should pass `ret` when computing many series in a loop.
- Intermediate series must return to the pool.
- Do not allocate new arrays/lists inside the per-index loop.
- Slicing should prefer views (`Slicer<T>`) over copying sublists.

### 8.3 Numerical robustness

- Guard divisions by zero with an explicit policy (existing code often returns `0.0`; keep family-consistent and document).
- Be explicit about `NaN` / infinity handling when introducing new math that can produce them.
- Prefer stable recursive formulations where they match industry definitions (Wilder RSI/ATR/ADX, EMA, etc.).

### 8.4 Complexity awareness

When composing indicators (for example `normalize` → `stddev` → `variance` → `sma`), be aware of intermediate allocations and nested passes. Prefer:

1. Reusing existing primitives for correctness and maintainability.
2. Optimizing only when profiling shows a hotspot — then consider fused implementations without breaking the public API.

---

## 9. Consistency Rules

### 9.1 Naming

Current library style uses **short, formula-oriented names** (`sma`, `ema`, `roc`, `wilders_rsi`, `macd_hist`).

**Rules:**

- Match existing casing and naming style within the same module.
- Prefer well-known technical analysis abbreviations over long prose names.
- Extension method name should match the static method name.
- Boolean-producing APIs should read as predicates/filters (`Condition`, `Filter`, `PortionTrue`).

> Note: Project-wide coding standards prefer PascalCase for public methods. TALib historically uses lower-case indicator names for domain familiarity. **Do not mix styles within a family.** If a future migration to PascalCase is approved, do it as a deliberate, documented breaking change across the public surface—not piecemeal.

### 9.2 Error handling

- Fail fast with messages that include bad values and relevant lengths.
- Prefer specific exception types for new code (`ArgumentNullException`, `ArgumentOutOfRangeException`, `ArgumentException`) while remaining consistent with nearby methods when touching legacy call sites.
- Never swallow exceptions.
- Do not return partial garbage without a documented warm-up policy.

### 9.3 Mutability and ownership

| Object                         | Ownership / mutability                                      |
|--------------------------------|-------------------------------------------------------------|
| Input series                   | Read-only by default; do not mutate                         |
| Caller-supplied `ret`          | Owned by caller; written by function; never disposed inside |
| Pool-allocated return value    | Owned by caller after return; dispose when finished         |
| Intermediate pooled lists      | Owned by the function; must dispose before return           |
| `Slicer<T>`                    | View; may write through; no structural mutation             |

### 9.4 Determinism

- Pure functions of inputs + parameters: same inputs → same outputs.
- No hidden global mutable configuration that changes results (pool stats may change; results must not).
- Do not depend on wall-clock time, random numbers, or environment state inside indicators.

### 9.5 Dependencies

- **No new NuGet packages** unless explicitly requested/approved.
- Stick to .NET BCL and existing project packages.
- Do not pull charting, math.NET, or TA-Lib native wrappers without an explicit design decision.

---

## 10. Concurrency Model

- `ListPool<T>` uses `ConcurrentBag<T>` and is safe for concurrent rent/return of distinct items.
- Individual `ListPoolItem<T>` / result lists are **not** free-threaded for concurrent writes.
- Indicator methods are safe to call concurrently **only if** each call uses distinct input/output buffers (or externally synchronized shared buffers).
- Do not store per-call state in static fields of indicator classes.

**Rule:** Treat indicator execution as pure + buffer-local. Shared mutable static state for computation is forbidden.

---

## 11. Testing and Validation Expectations

When changing indicator math or infrastructure:

1. Build successfully (`dotnet build`) with no new warnings when possible.
2. Run existing tests (`dotnet test`) if/when a test project is present.
3. Prefer tests named `MethodName_StateUnderTest_ExpectedBehavior`.
4. Cover at least:
   - Happy path known values (hand-calculated or golden fixtures)
   - Period / length validation failures
   - Warm-up leading zeros / first valid index
   - Equal-length multi-series validation
   - Optional `ret` reuse (same instance returned, correct contents)
   - Intermediate dispose / no double-free assumptions for pooled items
5. For performance-sensitive changes, compare allocations (for example pool stats or benchmark) before/after when practical.

**Rule:** Do not change numerical meaning of an existing public indicator without treating it as a breaking change and updating tests/fixtures.

---

## 12. Evolution Guidelines

### Allowed without architecture review

- New indicators that follow §6–§7
- Bug fixes preserving public contracts
- Performance improvements that keep signatures and results stable
- Docs, XML comments, and tests

### Requires explicit design discussion

- Changing period indexing / warm-up semantics
- Replacing `IList<double>` with a different primary series type (`Span<T>`, custom buffers, etc.)
- Introducing async APIs for pure CPU math
- Introducing DI, interfaces-per-indicator, or plugin frameworks
- Adding external libraries
- Breaking renames of the public surface
- Multi-timeframe engines, streaming/incremental-only APIs, or candle object models that replace list-based series

### Preferred future direction (non-binding)

If the library evolves, prefer:

- Stronger validation helpers shared across indicators
- Consistent exception types and messages
- Clear first-valid-index metadata (without breaking equal-length outputs)
- Broader automated golden tests
- Careful use of `Span<T>` / `ReadOnlySpan<T>` **behind** stable `IList<double>` APIs if profiling demands it

Do not pursue these at the cost of a silent break to existing callers.

---

## 13. Anti-Patterns (Do Not)

1. **LINQ-heavy indicator bodies** that allocate enumerators and temporary collections per call.
2. **Forgetting to dispose** pooled intermediates (pool leaks / memory growth).
3. **Disposing caller `ret`** buffers.
4. **Mutating input series** as a side effect.
5. **Silent length truncation** when series lengths differ.
6. **New static mutable configuration** that changes calculations.
7. **Copy-pasting** full algorithms instead of composing `Basic` / `Overlays` / existing oscillators.
8. **Putting business/strategy logic** (orders, portfolio, broker calls) into TALib.
9. **Introducing external TA packages** that duplicate or conflict with local implementations.
10. **Inconsistent warm-up policy** within a family without documentation.
11. **Implementation logic inside extension methods** (extensions must stay thin).
12. **Per-bar heap allocations** in the main loop (`new`, boxing, string formatting, lambda allocations).

---

## 14. Relationship to Other Rule Files

| File                   | Concern                                      |
|------------------------|----------------------------------------------|
| `AGENTS.md`            | Agent workflow, stack, when to ask           |
| `coding-standards.md`  | C# style, member ordering, language features |
| `architecture.md`      | Library structure, TA patterns, perf rules   |

When rules appear to conflict, apply this priority:

1. **Correctness & robustness** of numerical results and contracts  
2. **This architecture’s public series contracts** (signatures, pooling, warm-up)  
3. **coding-standards.md** for general C# style  
4. **Performance** when justified by measurement  

For TALib-specific naming and dual static/extension APIs, **this document wins** over generic style preferences until a deliberate migration is approved.

---

## 15. Quick Reference: Mental Model

```
Input series (IList<double>...)
        │
        ▼
 Validate parameters & lengths
        │
        ▼
 Acquire / size output buffer (ret || ListPool)
        │
        ▼
 Compute (for-loops; compose lower indicators)
        │
        ├─ dispose intermediates
        ▼
 Return output series (caller owns if pool-allocated)
```

**Design mantra:**  
*Stateless functions, equal-length series, optional reusable buffers, pool intermediates, compose simple primitives into richer indicators.*

---

**Last Updated:** August 2026  
**Version:** 1.0  
**Review Cadence:** Update when module boundaries, series contracts, pooling model, or indicator conventions change.
