namespace TALib.Tests;

public sealed class CommonTests
{
    [Fact]
    public void VolatilityConverter_DailyToAnnual_IsSqrt252()
    {
        Assert.Equal(Math.Sqrt(252), VolatilityConverter.DailyToAnnual, 12);
    }

    [Fact]
    public void VolatilityConverter_DailyToAnnual365_IsSqrt365()
    {
        Assert.Equal(Math.Sqrt(365), VolatilityConverter.DailyToAnnual365, 12);
    }

    [Fact]
    public void VolatilityConverter_DailyToMonthly_IsSqrt22()
    {
        Assert.Equal(Math.Sqrt(22), VolatilityConverter.DailyToMonthly, 12);
    }

    [Fact]
    public void VolatilityConverter_DailyToMonthly365_IsSqrt30()
    {
        Assert.Equal(Math.Sqrt(30), VolatilityConverter.DailyToMonthly365, 12);
    }

    [Fact]
    public void VolatilityConverter_DailyToWeekly_IsSqrt5()
    {
        Assert.Equal(Math.Sqrt(5), VolatilityConverter.DailyToWeekly, 12);
    }

    [Fact]
    public void VolatilityConverter_DailyToWeekly365_IsSqrt7()
    {
        Assert.Equal(Math.Sqrt(7), VolatilityConverter.DailyToWeekly365, 12);
    }

    [Fact]
    public void VolatilityConverter_WeeklyToAnnual_IsSqrt52()
    {
        Assert.Equal(Math.Sqrt(52), VolatilityConverter.WeeklyToAnnual, 12);
    }

    [Fact]
    public void VolatilityConverter_MonthlyToAnnual_IsSqrt12()
    {
        Assert.Equal(Math.Sqrt(12), VolatilityConverter.MonthlyToAnnual, 12);
    }
}
