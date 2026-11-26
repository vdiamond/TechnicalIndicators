# TechnicalIndicators (TALib)

High-performance, allocation-conscious technical analysis library for .NET. It operates on ordered price/volume series (`IList<double>`) and produces indicator series of the same length.

Agent guidance: [AGENTS.md](AGENTS.md).

## Projects

| Project | Role |
|---------|------|
| `TALib` | Core library (`net8.0`): overlays, oscillators, conditions, and list pooling |
| `TALib.Tests` | xUnit tests (`net8.0`) |
| `ta_console` | Hello-World placeholder (`net10.0`). It does not reference TALib. |

## Requirements

- .NET 8 SDK (`TALib` and `TALib.Tests` target `net8.0`)
- .NET 10 SDK (`ta_console` targets `net10.0`)

This repo does not depend on other vdiamond repositories.

## Build and test

```bash
dotnet build TechnicalIndicators.slnx
dotnet test TechnicalIndicators.slnx
```

`TechnicalIndicators.slnx` does not include `ta_console`. Build and run the placeholder with:

```bash
dotnet build ta_console/ta_console.slnx
dotnet run --project ta_console/ta_console.csproj
```

## Usage

Indicators are available as static methods and as fluent extensions:

```csharp
var sma = Overlays.sma(close, 20);
var sma = close.sma(20);
```

Pass an optional `ret` buffer to reuse output lists in tight loops. Intermediate pooled lists should be disposed when you are done with them.

## Layout

- `Overlays` — moving averages, lags, support/resistance traces
- `Oscillators` — momentum, volatility, multi-input indicators
- `Basic` — element-wise math
- `Conditions` — boolean filters and pointwise maps
- `Levels` / `PriceLevels` — bucketed price levels and turning-point tracking
- `Common` — volatility annualization factors
- `ListUtil` — pooling and slicing helpers
