namespace TALib.Tests;

public sealed class BasicTests
{
    [Fact]
    public void Add_SeriesAndSeries_AddsElementWise()
    {
        var a = TestHelpers.Series(1, 2, 3);
        var b = TestHelpers.Series(10, 20, 30);

        var result = Basic.add(a, b);

        TestHelpers.AssertSeriesEqual(result, 11, 22, 33);
        result.Dispose();
    }

    [Fact]
    public void Add_SeriesAndScalar_AddsScalarToEachElement()
    {
        var series = TestHelpers.Series(1, 2, 3);

        var result = Basic.add(series, 5);

        TestHelpers.AssertSeriesEqual(result, 6, 7, 8);
        result.Dispose();
    }

    [Fact]
    public void Sub_SeriesAndSeries_SubtractsElementWise()
    {
        var a = TestHelpers.Series(10, 20, 30);
        var b = TestHelpers.Series(1, 2, 3);

        var result = Basic.sub(a, b);

        TestHelpers.AssertSeriesEqual(result, 9, 18, 27);
        result.Dispose();
    }

    [Fact]
    public void Sub_SeriesAndScalar_SubtractsScalarFromEachElement()
    {
        var series = TestHelpers.Series(10, 20, 30);

        var result = Basic.sub(series, 5);

        TestHelpers.AssertSeriesEqual(result, 5, 15, 25);
        result.Dispose();
    }

    [Fact]
    public void Mul_SeriesAndSeries_MultipliesElementWise()
    {
        var a = TestHelpers.Series(2, 3, 4);
        var b = TestHelpers.Series(5, 6, 7);

        var result = Basic.mul(a, b);

        TestHelpers.AssertSeriesEqual(result, 10, 18, 28);
        result.Dispose();
    }

    [Fact]
    public void Mul_SeriesAndScalar_MultipliesEachElement()
    {
        var series = TestHelpers.Series(2, 3, 4);

        var result = Basic.mul(series, 10);

        TestHelpers.AssertSeriesEqual(result, 20, 30, 40);
        result.Dispose();
    }

    [Fact]
    public void Div_SeriesAndSeries_DividesElementWise()
    {
        var a = TestHelpers.Series(10, 20, 30);
        var b = TestHelpers.Series(2, 5, 10);

        var result = Basic.div(a, b);

        TestHelpers.AssertSeriesEqual(result, 5, 4, 3);
        result.Dispose();
    }

    [Fact]
    public void Div_ByZero_ReturnsZero()
    {
        var series = TestHelpers.Series(10, 20);
        var zeros = TestHelpers.Series(0, 5);

        var result = Basic.div(series, zeros);

        TestHelpers.AssertSeriesEqual(result, 0, 4);
        result.Dispose();
    }

    [Fact]
    public void Div_SeriesAndScalar_DividesEachElement()
    {
        var series = TestHelpers.Series(10, 20, 30);

        var result = Basic.div(series, 10);

        TestHelpers.AssertSeriesEqual(result, 1, 2, 3);
        result.Dispose();
    }

    [Fact]
    public void Abs_ReturnsAbsoluteValues()
    {
        var series = TestHelpers.Series(-2, 0, 3.5);

        var result = Basic.abs(series);

        TestHelpers.AssertSeriesEqual(result, 2, 0, 3.5);
        result.Dispose();
    }

    [Fact]
    public void Acos_MatchesMathAcos()
    {
        var series = TestHelpers.Series(1, 0, -1);

        var result = Basic.acos(series);

        TestHelpers.AssertSeriesEqual(result, Math.Acos(1), Math.Acos(0), Math.Acos(-1));
        result.Dispose();
    }

    [Fact]
    public void Acosh_MatchesMathAcosh()
    {
        var series = TestHelpers.Series(1, 2, 10);

        var result = Basic.acosh(series);

        TestHelpers.AssertSeriesEqual(result, Math.Acosh(1), Math.Acosh(2), Math.Acosh(10));
        result.Dispose();
    }

    [Fact]
    public void Asin_MatchesMathAsin()
    {
        var series = TestHelpers.Series(0, 0.5, 1);

        var result = Basic.asin(series);

        TestHelpers.AssertSeriesEqual(result, Math.Asin(0), Math.Asin(0.5), Math.Asin(1));
        result.Dispose();
    }

    [Fact]
    public void Asinh_MatchesMathAsinh()
    {
        var series = TestHelpers.Series(0, 1, 2);

        var result = Basic.asinh(series);

        TestHelpers.AssertSeriesEqual(result, Math.Asinh(0), Math.Asinh(1), Math.Asinh(2));
        result.Dispose();
    }

    [Fact]
    public void Atan_MatchesMathAtan()
    {
        var series = TestHelpers.Series(0, 1, -1);

        var result = Basic.atan(series);

        TestHelpers.AssertSeriesEqual(result, Math.Atan(0), Math.Atan(1), Math.Atan(-1));
        result.Dispose();
    }

    [Fact]
    public void Atanh_MatchesMathAtanh()
    {
        var series = TestHelpers.Series(0, 0.5, -0.5);

        var result = Basic.atanh(series);

        TestHelpers.AssertSeriesEqual(result, Math.Atanh(0), Math.Atanh(0.5), Math.Atanh(-0.5));
        result.Dispose();
    }

    [Fact]
    public void Cbrt_MatchesMathCbrt()
    {
        var series = TestHelpers.Series(8, 27, -8);

        var result = Basic.cbrt(series);

        TestHelpers.AssertSeriesEqual(result, Math.Cbrt(8), Math.Cbrt(27), Math.Cbrt(-8));
        result.Dispose();
    }

    [Fact]
    public void Cos_MatchesMathCos()
    {
        var series = TestHelpers.Series(0, Math.PI / 2, Math.PI);

        var result = Basic.cos(series);

        TestHelpers.AssertSeriesEqual(result, Math.Cos(0), Math.Cos(Math.PI / 2), Math.Cos(Math.PI));
        result.Dispose();
    }

    [Fact]
    public void Cosh_MatchesMathCosh()
    {
        var series = TestHelpers.Series(0, 1, 2);

        var result = Basic.cosh(series);

        TestHelpers.AssertSeriesEqual(result, Math.Cosh(0), Math.Cosh(1), Math.Cosh(2));
        result.Dispose();
    }

    [Fact]
    public void Exp_MatchesMathExp()
    {
        var series = TestHelpers.Series(0, 1, 2);

        var result = Basic.exp(series);

        TestHelpers.AssertSeriesEqual(result, Math.Exp(0), Math.Exp(1), Math.Exp(2));
        result.Dispose();
    }

    [Fact]
    public void Ilogb_MatchesMathILogB()
    {
        var series = TestHelpers.Series(1, 2, 8);

        var result = Basic.ilogb(series);

        TestHelpers.AssertSeriesEqual(result, Math.ILogB(1), Math.ILogB(2), Math.ILogB(8));
        result.Dispose();
    }

    [Fact]
    public void Log_MatchesMathLog_AndZeroReturnsZero()
    {
        var series = TestHelpers.Series(1, Math.E, 0);

        var result = Basic.log(series);

        TestHelpers.AssertSeriesEqual(result, Math.Log(1), Math.Log(Math.E), 0);
        result.Dispose();
    }

    [Fact]
    public void Log10_MatchesMathLog10()
    {
        var series = TestHelpers.Series(1, 10, 100);

        var result = Basic.log10(series);

        TestHelpers.AssertSeriesEqual(result, 0, 1, 2);
        result.Dispose();
    }

    [Fact]
    public void Log2_MatchesMathLog2()
    {
        var series = TestHelpers.Series(1, 2, 8);

        var result = Basic.log2(series);

        TestHelpers.AssertSeriesEqual(result, 0, 1, 3);
        result.Dispose();
    }

    [Fact]
    public void Sin_MatchesMathSin()
    {
        var series = TestHelpers.Series(0, Math.PI / 2, Math.PI);

        var result = Basic.sin(series);

        TestHelpers.AssertSeriesEqual(result, Math.Sin(0), Math.Sin(Math.PI / 2), Math.Sin(Math.PI));
        result.Dispose();
    }

    [Fact]
    public void Sinh_MatchesMathSinh()
    {
        var series = TestHelpers.Series(0, 1, 2);

        var result = Basic.sinh(series);

        TestHelpers.AssertSeriesEqual(result, Math.Sinh(0), Math.Sinh(1), Math.Sinh(2));
        result.Dispose();
    }

    [Fact]
    public void Sqrt_MatchesMathSqrt()
    {
        var series = TestHelpers.Series(0, 4, 9);

        var result = Basic.sqrt(series);

        TestHelpers.AssertSeriesEqual(result, 0, 2, 3);
        result.Dispose();
    }

    [Fact]
    public void Tan_MatchesMathTan()
    {
        var series = TestHelpers.Series(0, Math.PI / 4);

        var result = Basic.tan(series);

        TestHelpers.AssertSeriesEqual(result, Math.Tan(0), Math.Tan(Math.PI / 4));
        result.Dispose();
    }

    [Fact]
    public void Tanh_MatchesMathTanh()
    {
        var series = TestHelpers.Series(0, 1, -1);

        var result = Basic.tanh(series);

        TestHelpers.AssertSeriesEqual(result, Math.Tanh(0), Math.Tanh(1), Math.Tanh(-1));
        result.Dispose();
    }

    [Fact]
    public void Pow_SeriesAndSeries_RaisesElementWise()
    {
        var bases = TestHelpers.Series(2, 3, 4);
        var exponents = TestHelpers.Series(3, 2, 0.5);

        var result = Basic.pow(bases, exponents);

        TestHelpers.AssertSeriesEqual(result, 8, 9, 2);
        result.Dispose();
    }

    [Fact]
    public void Pow_SeriesAndScalar_RaisesEachElement()
    {
        var series = TestHelpers.Series(2, 3, 4);

        var result = Basic.pow(series, 2);

        TestHelpers.AssertSeriesEqual(result, 4, 9, 16);
        result.Dispose();
    }

    [Fact]
    public void Sigma_AppliesLogisticTransform()
    {
        var series = TestHelpers.Series(0, 1, -1);
        double k = 1.0;

        var result = Basic.sigma(series, k);

        // 1 / (1 + exp(-k * x))
        TestHelpers.AssertSeriesEqual(
            result,
            1.0 / (1.0 + Math.Exp(0)),
            1.0 / (1.0 + Math.Exp(-1)),
            1.0 / (1.0 + Math.Exp(1)));
        result.Dispose();
    }

    [Fact]
    public void Nop_CopiesSeries()
    {
        var series = TestHelpers.Series(1.5, -2, 0);

        var result = Basic.nop(series);

        TestHelpers.AssertSeriesEqual(result, 1.5, -2, 0);
        Assert.NotSame(series, result);
        result.Dispose();
    }

    [Fact]
    public void Add_WithRetBuffer_ReusesCallerBuffer()
    {
        var series = TestHelpers.Series(1, 2, 3);
        var buffer = ListPool<double>.Get();

        var result = Basic.add(series, 1, buffer);

        Assert.Same(buffer, result);
        TestHelpers.AssertSeriesEqual(result, 2, 3, 4);
        buffer.Dispose();
    }

    [Fact]
    public void ExtensionMethods_DelegateToBasic()
    {
        var series = TestHelpers.Series(4, 9, 16);

        var result = series.sqrt();

        TestHelpers.AssertSeriesEqual(result, 2, 3, 4);
        result.Dispose();
    }
}
