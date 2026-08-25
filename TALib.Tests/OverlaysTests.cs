namespace TALib.Tests;

public sealed class OverlaysTests
{
    [Fact]
    public void Sma_Period2_ComputesRollingMeanOfThreeBars()
    {
        // Library convention: window length = period + 1
        var series = TestHelpers.Series(1, 2, 3, 4, 5);

        var result = Overlays.sma(series, 2);

        TestHelpers.AssertSeriesEqual(result, 0, 0, 2, 3, 4);
        result.Dispose();
    }

    [Fact]
    public void Ema_Period2_SeedsWithSmaThenAppliesMultiplier()
    {
        var series = TestHelpers.Series(1, 2, 3, 4);

        var result = Overlays.ema(series, 2);

        // seed at index 2: (1+2+3)/3 = 2
        // mult = 2 / (period + 2) = 0.5
        // index 3: ((4 - 2) * 0.5) + 2 = 3
        TestHelpers.AssertSeriesEqual(result, 0, 0, 2, 3);
        result.Dispose();
    }

    [Fact]
    public void Offset_Positive_ShiftsSeriesForwardWithLeadingZeros()
    {
        var series = TestHelpers.Series(10, 20, 30, 40);

        var result = Overlays.offset(series, 1);

        // ret[i] = series[i - offset]
        TestHelpers.AssertSeriesEqual(result, 0, 10, 20, 30);
        result.Dispose();
    }

    [Fact]
    public void Offset_Negative_ShiftsSeriesBackwardWithTrailingZeros()
    {
        var series = TestHelpers.Series(10, 20, 30, 40);

        var result = Overlays.offset(series, -1);

        TestHelpers.AssertSeriesEqual(result, 20, 30, 40, 0);
        result.Dispose();
    }

    [Fact]
    public void Rpt_TracksLastResistancePivot()
    {
        // Local peak at index 1 (value 5): left=1, mid=5, right=3
        var series = TestHelpers.Series(1, 5, 3, 4, 2);

        var result = Overlays.rpt(series);

        Assert.Equal(5, result.Count);
        Assert.Equal(1, result[0]);
        // After peak at mid=1, subsequent bars carry last resistance point 5 until a new peak
        Assert.Equal(5, result[2]);
        Assert.Equal(5, result[3]);
        result.Dispose();
    }

    [Fact]
    public void Spt_TracksLastSupportPivot()
    {
        // Local trough at index 1 (value 1): left=5, mid=1, right=3
        var series = TestHelpers.Series(5, 1, 3, 2, 4);

        var result = Overlays.spt(series);

        Assert.Equal(5, result.Count);
        Assert.Equal(5, result[0]);
        Assert.Equal(1, result[2]);
        Assert.Equal(1, result[3]);
        result.Dispose();
    }

    [Fact]
    public void Sma_WithRetBuffer_ReusesCallerBuffer()
    {
        var series = TestHelpers.Series(1, 2, 3, 4);
        var buffer = ListPool<double>.Get();

        var result = Overlays.sma(series, 1, buffer);

        Assert.Same(buffer, result);
        TestHelpers.AssertSeriesEqual(result, 0, 1.5, 2.5, 3.5);
        buffer.Dispose();
    }

    [Fact]
    public void ExtensionMethods_DelegateToOverlays()
    {
        var series = TestHelpers.Series(1, 2, 3, 4);

        var result = series.sma(1);

        TestHelpers.AssertSeriesEqual(result, 0, 1.5, 2.5, 3.5);
        result.Dispose();
    }
}
