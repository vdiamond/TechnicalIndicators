namespace TALib.Tests;

public sealed class ConditionsTests
{
    [Fact]
    public void Filter_AppliesPredicateElementWise()
    {
        var series = TestHelpers.Series(1, 2, 3, 4);

        var result = Conditions.Filter(series, x => x >= 3);

        Assert.Equal(4, result.Count);
        Assert.False(result[0]);
        Assert.False(result[1]);
        Assert.True(result[2]);
        Assert.True(result[3]);
        result.Dispose();
    }

    [Fact]
    public void Condition_OneSeries_AppliesPredicate()
    {
        var series = TestHelpers.Series(1, 5, 2);

        var result = Conditions.Condition(series, x => x > 3);

        Assert.False(result[0]);
        Assert.True(result[1]);
        Assert.False(result[2]);
        result.Dispose();
    }

    [Fact]
    public void Condition_TwoSeries_AppliesBinaryPredicate()
    {
        var a = TestHelpers.Series(1, 5, 3);
        var b = TestHelpers.Series(2, 4, 3);

        var result = Conditions.Condition(a, b, (x, y) => x > y);

        Assert.False(result[0]);
        Assert.True(result[1]);
        Assert.False(result[2]);
        result.Dispose();
    }

    [Fact]
    public void Condition_ThreeSeries_AppliesTernaryPredicate()
    {
        var a = TestHelpers.Series(1, 2, 3);
        var b = TestHelpers.Series(1, 2, 0);
        var c = TestHelpers.Series(1, 0, 3);

        var result = Conditions.Condition(a, b, c, (x, y, z) => x == y && y == z);

        Assert.True(result[0]);
        Assert.False(result[1]);
        Assert.False(result[2]);
        result.Dispose();
    }

    [Fact]
    public void PortionTrue_WithoutFilter_ReturnsOverallFraction()
    {
        var series = new List<bool> { true, false, true, true };

        var portion = Conditions.PortionTrue(series);

        Assert.Equal(0.75, portion);
    }

    [Fact]
    public void PortionTrue_WithFilter_ReturnsFractionWithinFilter()
    {
        var series = new List<bool> { true, false, true, false };
        var filter = new List<bool> { true, true, false, false };

        var portion = Conditions.PortionTrue(series, filter);

        // only first two considered: true, false => 0.5
        Assert.Equal(0.5, portion);
    }

    [Fact]
    public void PortionTrue_EmptySeries_ReturnsZero()
    {
        var series = new List<bool>();

        var portion = Conditions.PortionTrue(series);

        Assert.Equal(0.0, portion);
    }

    [Fact]
    public void Calc_OneSeries_MapsElementWise()
    {
        var series = TestHelpers.Series(1, 2, 3);

        var result = Conditions.Calc(series, x => x * 10);

        TestHelpers.AssertSeriesEqual(result, 10, 20, 30);
        result.Dispose();
    }

    [Fact]
    public void Calc_TwoSeries_MapsPairWise()
    {
        var a = TestHelpers.Series(1, 2, 3);
        var b = TestHelpers.Series(4, 5, 6);

        var result = Conditions.Calc(a, b, (x, y) => x + y);

        TestHelpers.AssertSeriesEqual(result, 5, 7, 9);
        result.Dispose();
    }

    [Fact]
    public void Calc_ThreeSeries_MapsTripleWise()
    {
        var a = TestHelpers.Series(1, 2, 3);
        var b = TestHelpers.Series(4, 5, 6);
        var c = TestHelpers.Series(7, 8, 9);

        var result = Conditions.Calc(a, b, c, (x, y, z) => x + y + z);

        TestHelpers.AssertSeriesEqual(result, 12, 15, 18);
        result.Dispose();
    }

    [Fact]
    public void ExtensionMethods_DelegateToConditions()
    {
        var series = TestHelpers.Series(1, 2, 3, 4);

        var result = series.Filter(x => x % 2 == 0);

        Assert.False(result[0]);
        Assert.True(result[1]);
        Assert.False(result[2]);
        Assert.True(result[3]);
        result.Dispose();
    }
}
