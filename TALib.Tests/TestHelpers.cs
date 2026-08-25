using System.Collections.Generic;

namespace TALib.Tests;

internal static class TestHelpers
{
    public static List<double> Series(params double[] values)
    {
        return new List<double>(values);
    }

    public static void AssertSeriesEqual(IList<double> actual, IList<double> expected, int precision = 10)
    {
        Assert.Equal(expected.Count, actual.Count);
        for (int i = 0; i < expected.Count; i++)
        {
            Assert.Equal(expected[i], actual[i], precision);
        }
    }

    public static void AssertSeriesEqual(IList<double> actual, params double[] expected)
    {
        AssertSeriesEqual(actual, expected, 10);
    }

    public static void AssertFinite(IList<double> series)
    {
        for (int i = 0; i < series.Count; i++)
        {
            Assert.False(double.IsNaN(series[i]), $"NaN at index {i}");
            Assert.False(double.IsInfinity(series[i]), $"Infinity at index {i}");
        }
    }
}
