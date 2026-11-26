# TechnicalIndicators (TALib)

High-performance, allocation-conscious technical analysis library for .NET. Indicator methods take ordered price/volume series (`IList<double>`) and return a series of the same length.

**Status.** `TALib` (`net8.0`) and `TALib.Tests` (`net8.0`, xUnit) are the library and its test suite. `ta_console` (`net10.0`) is a Hello-World placeholder: `Program.Main` prints `Hello, World!` and the project does not reference `TALib`. There is no CI workflow in the repo.

## Build, run, and test

Use the .NET 8 SDK for `TALib` and `TALib.Tests`, and the .NET 10 SDK for `ta_console`.

From the repository root:

```bash
dotnet build TechnicalIndicators.slnx
dotnet test TechnicalIndicators.slnx
```

`TechnicalIndicators.slnx` contains only `TALib` and `TALib.Tests`. Both commands succeed. The library build reports nullable warnings (optional `ret = null` on non-nullable parameters) and 0 errors. The xUnit suite passes.

`ta_console` is not in that solution. Build and run it with:

```bash
dotnet build ta_console/ta_console.slnx
dotnet run --project ta_console/ta_console.csproj
```

`dotnet run` prints `Hello, World!`. `ta_console/ta_console.slnx` lists `TALib` and `ta_console`, but `ta_console.csproj` has no `ProjectReference` to `TALib`.

## Layout

| Path | Role |
|------|------|
| `TALib/` | Class library. No NuGet packages. |
| `TALib.Tests/` | xUnit tests. `ProjectReference` to `../TALib/TALib.csproj`. |
| `ta_console/` | Console placeholder. No project references. |
| `TechnicalIndicators.slnx` | Library and tests. |
| `.cursor/rules/` | Coding standards and TALib architecture. |

Files in `TALib/`:

- `ListUtil.cs` — `ListPool<T>`, `ListPoolItem<T>`, `Slicer<T>`, `ListExtention` (`AdjustLength`, `Slice`, `Dispose`)
- `Common.cs` — `VolatilityConverter`
- `Basic.cs` — element-wise math
- `Overlays.cs` — `sma`, `ema`, `offset`, `rpt`, `spt`
- `Oscillators.cs` — momentum, volatility, and multi-input indicators
- `Conditions.cs` — boolean filters and pointwise `Calc`
- `Levels.cs` — `Levels<T>` price-bucket map
- `PriceLevels.cs` — stateful bar logger that stores turning points in `Levels<LevelData>`

## Dependencies on other vdiamond repos

None. Checked project files, source, and git:

- No `.gitmodules` and no submodule.
- The only `ProjectReference` is `TALib.Tests` → `../TALib/TALib.csproj` (inside this repo).
- `TALib` and `ta_console` have no `PackageReference`. `TALib.Tests` references NuGet packages only: `xunit` 2.5.3, `xunit.runner.visualstudio` 2.5.3, `Microsoft.NET.Test.Sdk` 17.8.0, `coverlet.collector` 6.0.0.
- No HTTP or other cross-repo calls.

Nothing in this repo needs a sibling clone. No dependents are recorded here.

## Conventions

`.cursor/rules/coding-standards.mdc` is the governing C# standard. `.cursor/rules/architecture.mdc` is the governing rule for indicator shape, pooling, and series contracts.

Existing code does not fully comply. Bring code into compliance when you touch it. Do not reformat unrelated code.

What the code does today:

- Nullable reference types and implicit usings are on. There is no root `.editorconfig`. Warnings are not errors.
- Public indicators use lowercase formula names (`sma`, `wilders_rsi`) on static classes, plus thin wrappers on `partial class ListDoubleExtensions`. Keep that naming. A PascalCase rename is a breaking public-API change and needs Victor's approval. Until then, `.cursor/rules/architecture.mdc` wins over the PascalCase method rule for these APIs.
- Pass an optional `ret` buffer to reuse an output list. If `ret` is omitted, the method rents from `ListPool<T>`. The caller owns a pool-allocated result and disposes it. Never dispose a caller-supplied `ret`. Dispose pooled intermediates before returning.
- Output length matches the input. Bars before the first valid value are `0.0` unless that method documents another warm-up.
- Multi-series methods throw when lengths differ.
- A period of `n` looks back `n` bars (`Common.cs`).
- Tests are xUnit `[Fact]` methods named `MethodName_StateUnderTest_ExpectedBehavior`, written as Arrange-Act-Assert.

## What not to touch

- Secrets and local config: `.env`, `*.pfx`, `*.p12`, `*.key`, `appsettings.*.local.json`. Do not commit them.
- Generated output: `bin/`, `obj/`, `TestResults/`, coverage files, NuGet `artifacts/`.
- Public indicator names, signatures, period indexing, and warm-up behavior, unless Victor asks.
- New NuGet packages, and any reference to another repository, unless Victor asks.

Development happens only through Victor and Cursor agents.
