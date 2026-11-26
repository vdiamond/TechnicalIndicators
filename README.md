# TechnicalIndicators (TALib)

High-performance, allocation-conscious technical analysis library for .NET. It operates on ordered price/volume series (`IList<double>`) and produces indicator series of the same length.

## Projects

| Project | Role |
|---------|------|
| `TALib` | Core library: overlays, oscillators, conditions, and list pooling |
| `TALib.Tests` | xUnit tests |
| `ta_console` | Optional scratch/demo host |

## Requirements

- .NET 8 (library and tests)
- .NET 10 (`ta_console` only)

## Build and test

```bash
dotnet build TechnicalIndicators.slnx
dotnet test TechnicalIndicators.slnx
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
- `ListUtil` — pooling and slicing helpers

## License

MIT. See [LICENSE](LICENSE).
