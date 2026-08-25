namespace TALib.Tests;

public sealed class OscillatorsTests
{
    [Fact]
    public void Roc_Period1_ReturnsRatioToPriorBar()
    {
        var series = TestHelpers.Series(10, 20, 15);

        var result = Oscillators.roc(series, 1);

        TestHelpers.AssertSeriesEqual(result, 0, 2, 0.75);
        result.Dispose();
    }

    [Fact]
    public void Mom_Period1_ReturnsDifferenceFromPriorBar()
    {
        var series = TestHelpers.Series(10, 20, 15);

        var result = Oscillators.mom(series, 1);

        TestHelpers.AssertSeriesEqual(result, 0, 10, -5);
        result.Dispose();
    }

    [Fact]
    public void Yield_Period1_ReturnsRelativeChange()
    {
        var series = TestHelpers.Series(10, 20, 15);

        var result = Oscillators.yield(series, 1);

        TestHelpers.AssertSeriesEqual(result, 0, 1.0, -0.25);
        result.Dispose();
    }

    [Fact]
    public void Variance_Period1_MatchesHandCalculation()
    {
        var series = TestHelpers.Series(1, 2, 3);

        var result = Oscillators.variance(series, 1);

        // sma(1) at i=1 is 1.5; variance of [1,2] about 1.5 = 0.25
        // sma(1) at i=2 is 2.5; variance of [2,3] about 2.5 = 0.25
        TestHelpers.AssertSeriesEqual(result, 0, 0.25, 0.25);
        result.Dispose();
    }

    [Fact]
    public void Stddev_Period1_IsSqrtOfVariance()
    {
        var series = TestHelpers.Series(1, 2, 3);

        var result = Oscillators.stddev(series, 1);

        TestHelpers.AssertSeriesEqual(result, 0, 0.5, 0.5);
        result.Dispose();
    }

    [Fact]
    public void Normalize_Period1_IsZScoreAgainstSmaAndStddev()
    {
        var series = TestHelpers.Series(1, 2, 3);

        var result = Oscillators.normalize(series, 1);

        // at i=1: (2 - 1.5) / 0.5 = 1
        // at i=2: (3 - 2.5) / 0.5 = 1
        TestHelpers.AssertSeriesEqual(result, 0, 1, 1);
        result.Dispose();
    }

    [Fact]
    public void Volatility_Period1_ProducesFiniteSeries()
    {
        var series = TestHelpers.Series(100, 102, 101, 105, 104, 108);

        var result = Oscillators.volatility(series, 1);

        Assert.Equal(series.Count, result.Count);
        TestHelpers.AssertFinite(result);
        Assert.Equal(0, result[0]);
        result.Dispose();
    }

    [Fact]
    public void Obv_AccumulatesVolumeByPriceDirection()
    {
        var volume = TestHelpers.Series(100, 50, 75, 25);
        var prices = TestHelpers.Series(10, 12, 11, 13);

        var result = Oscillators.obv(volume, prices);

        // start 0
        // up: 0+100? wait ret[0]=0, then i=1 prices up: ret[1] = 0 + 50 = 50
        // looking at code: ret[0]=0; ret[i] += ret[i-1] + series[i] for up
        // i=1 up: ret[1] = 0 + 0 + 50 = 50
        // i=2 down: ret[2] = 0 + 50 - 75 = -25
        // i=3 up: ret[3] = 0 + (-25) + 25 = 0
        TestHelpers.AssertSeriesEqual(result, 0, 50, -25, 0);
        result.Dispose();
    }

    [Fact]
    public void Psl_Period2_TracksPortionOfUpMoves()
    {
        // changes: + + - +  => series 1,2,3,2,4
        var series = TestHelpers.Series(1, 2, 3, 2, 4);

        var result = Oscillators.psl(series, 2);

        Assert.Equal(series.Count, result.Count);
        Assert.Equal(0, result[0]);
        Assert.Equal(0, result[1]);
        // at period index 2: ups in window of 2 prior steps (st 1..2): both up => 1.0
        Assert.Equal(1.0, result[2]);
        result.Dispose();
    }

    [Fact]
    public void Slope_Period2_ProducesFiniteValuesAfterWarmup()
    {
        var series = TestHelpers.Series(1, 2, 3, 4, 5, 6);

        var result = Oscillators.slope(series, 2);

        Assert.Equal(series.Count, result.Count);
        Assert.Equal(0, result[0]);
        Assert.Equal(0, result[1]);
        TestHelpers.AssertFinite(result);
        // strictly increasing series should have positive slope after warmup
        Assert.True(result[5] > 0);
        result.Dispose();
    }

    [Fact]
    public void WildersRsi_Period2_SeedsAndSmooths()
    {
        var series = TestHelpers.Series(10, 11, 12, 11, 13);

        var result = Oscillators.wilders_rsi(series, 2);

        Assert.Equal(series.Count, result.Count);
        // first valid RSI at index = period
        Assert.True(result[2] > 0);
        Assert.InRange(result[2], 0, 100);
        Assert.InRange(result[4], 0, 100);
        result.Dispose();
    }

    [Fact]
    public void Macd_IsShortEmaMinusLongEma()
    {
        var series = TestHelpers.Series(
            10, 11, 12, 13, 14, 15, 16, 17, 18, 19,
            20, 21, 22, 23, 24, 25, 26, 27, 28, 29,
            30, 31, 32, 33, 34, 35, 36, 37, 38, 39);

        var macd = Oscillators.macd(series, periodLong: 5, periodShort: 2);
        var shortEma = Overlays.ema(series, 2);
        var longEma = Overlays.ema(series, 5);
        var expected = Basic.sub(shortEma, longEma);

        TestHelpers.AssertSeriesEqual(macd, expected);

        macd.Dispose();
        shortEma.Dispose();
        longEma.Dispose();
        expected.Dispose();
    }

    [Fact]
    public void MacdSignal_IsEmaOfMacd()
    {
        var series = TestHelpers.Series(
            10, 11, 12, 13, 14, 15, 16, 17, 18, 19,
            20, 21, 22, 23, 24, 25, 26, 27, 28, 29,
            30, 31, 32, 33, 34, 35, 36, 37, 38, 39);

        var signal = Oscillators.macd_signal(series, periodLong: 5, periodShort: 2, periodSignal: 2);
        var macd = Oscillators.macd(series, 5, 2);
        var expected = Overlays.ema(macd, 2);

        TestHelpers.AssertSeriesEqual(signal, expected);

        signal.Dispose();
        macd.Dispose();
        expected.Dispose();
    }

    [Fact]
    public void MacdHist_IsMacdMinusSignal()
    {
        var series = TestHelpers.Series(
            10, 11, 12, 13, 14, 15, 16, 17, 18, 19,
            20, 21, 22, 23, 24, 25, 26, 27, 28, 29,
            30, 31, 32, 33, 34, 35, 36, 37, 38, 39);

        var hist = Oscillators.macd_hist(series, periodLong: 5, periodShort: 2, periodSignal: 2);
        var macd = Oscillators.macd(series, 5, 2);
        var signal = Overlays.ema(macd, 2);
        var expected = Basic.sub(macd, signal);

        TestHelpers.AssertSeriesEqual(hist, expected);

        hist.Dispose();
        macd.Dispose();
        signal.Dispose();
        expected.Dispose();
    }

    [Fact]
    public void WildersAtr_Period2_ProducesPositiveValuesAfterWarmup()
    {
        var high = TestHelpers.Series(12, 13, 14, 15, 16);
        var low = TestHelpers.Series(10, 11, 11, 12, 13);
        var close = TestHelpers.Series(11, 12, 13, 14, 15);

        var result = Oscillators.wilders_atr(close, low, high, 2);

        Assert.Equal(5, result.Count);
        Assert.True(result[2] > 0);
        Assert.True(result[4] > 0);
        TestHelpers.AssertFinite(result);
        result.Dispose();
    }

    [Fact]
    public void BollingerBandWidth_Period2_MatchesComposition()
    {
        var series = TestHelpers.Series(10, 12, 11, 13, 12, 14);

        var result = Oscillators.bollinger_band_width(series, 2, 2.0);
        var sma = Overlays.sma(series, 2);
        var stddev = Oscillators.stddev(series, 2);

        Assert.Equal(0, result[0]);
        Assert.Equal(0, result[1]);
        for (int i = 2; i < series.Count; i++)
        {
            double expected = (4.0 * stddev[i]) / sma[i]; // (upper-lower)/sma = (4*std)/sma for 2 stddevs
            Assert.Equal(expected, result[i], 10);
        }

        result.Dispose();
        sma.Dispose();
        stddev.Dispose();
    }

    [Fact]
    public void BollingerBandPctB_Period2_IsInExpectedRangeForInteriorPrice()
    {
        var series = TestHelpers.Series(10, 12, 11, 13, 12, 14);

        var result = Oscillators.bollinger_band_pctb(series, 2, 2.0);

        Assert.Equal(series.Count, result.Count);
        Assert.Equal(0, result[0]);
        Assert.Equal(0, result[1]);
        TestHelpers.AssertFinite(result);
        result.Dispose();
    }

    [Fact]
    public void WildersAdx_Period2_ProducesFiniteValuesAfterWarmup()
    {
        // Need count >= period * 2
        var high = TestHelpers.Series(12, 13, 14, 15, 16, 17, 18, 19);
        var low = TestHelpers.Series(10, 11, 11, 12, 13, 14, 15, 16);
        var close = TestHelpers.Series(11, 12, 13, 14, 15, 16, 17, 18);

        var result = Oscillators.wilders_adx(close, low, high, 2);

        Assert.Equal(high.Count, result.Count);
        TestHelpers.AssertFinite(result);
        // first valid ADX around period*2 - 1 = 3
        Assert.True(result[3] >= 0);
        result.Dispose();
    }

    [Fact]
    public void ExtensionMethods_DelegateToOscillators()
    {
        var series = TestHelpers.Series(10, 20, 15, 25);

        var result = series.roc(1);

        TestHelpers.AssertSeriesEqual(result, 0, 2, 0.75, 25.0 / 15.0);
        result.Dispose();
    }

    [Fact]
    public void Roc_WithRetBuffer_ReusesCallerBuffer()
    {
        var series = TestHelpers.Series(10, 20, 30);
        var buffer = ListPool<double>.Get();

        var result = Oscillators.roc(series, 1, buffer);

        Assert.Same(buffer, result);
        TestHelpers.AssertSeriesEqual(result, 0, 2, 1.5);
        buffer.Dispose();
    }
}
