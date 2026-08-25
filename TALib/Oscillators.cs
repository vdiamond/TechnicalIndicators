using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace TALib
{
    static public class Oscillators
    {
        static public IList<double> roc(IList<double> series, int period, IList<double> ret = null)
        {
            if (period < 0) { throw new Exception($"Period parameter ({period}) cannot be less than 0"); }
            if (period >= series.Count) { throw new Exception($"Period parameter ({period}) cannot be greater than or equal to length of series ({series.Count})"); }

            if (ret == null) { ret = ListPool<double>.Get(); }
            ret.AdjustLength<double>(series.Count);

            // Calculate start 
            int st = 0;
            while (st <= period)
            {
                if (st < period)
                {
                    ret[st] = 0.0;
                }
                else
                {
                    if (series[st - period] == 0.0) { ret[st] = 0.0; }
                    else { ret[st] = series[st] / series[st - period]; }
                }
                st++;
            }

            // Go through the main loop
            for (int i = st; i < series.Count; i++)
            {
                if (series[i - period] == 0.0) { ret[i] = 0.0; continue; }
                ret[i] = series[i] / series[i - period];
            }

            return ret;
        }
        static public IList<double> mom(IList<double> series, int period, IList<double> ret = null)
        {
            if (period < 0) { throw new Exception($"Period parameter ({period}) cannot be less than 0"); }
            if (period >= series.Count) { throw new Exception($"Period parameter ({period}) cannot be greater than or equal to length of series ({series.Count})"); }

            if (ret == null) { ret = ListPool<double>.Get(); }
            ret.AdjustLength<double>(series.Count);

            // Calculate start 
            int st = 0;
            while (st <= period)
            {
                if (st < period)
                {
                    ret[st] = 0.0;
                }
                else
                {
                    ret[st] = series[st] - series[st - period];
                }
                st++;
            }

            // Go through the main loop
            for (int i = st; i < series.Count; i++)
            {
                if (series[i - period] == 0.0) { ret[i] = 0.0; continue; }
                ret[i] = series[i] - series[i - period];
            }

            return ret;
        }
        static public IList<double> variance(IList<double> series, int period, IList<double> ret = null)
        {
            if (period < 0) { throw new Exception($"Period parameter ({period}) cannot be less than 0"); }
            if (period >= series.Count) { throw new Exception($"Period parameter ({period}) cannot be greater than or equal to length of series ({series.Count})"); }

            if (ret == null) { ret = ListPool<double>.Get(); }
            ret.AdjustLength<double>(series.Count);

            var mean = series.sma(period);

            for (int i = 0; i < period; i++) { ret[i] = 0.0; }

            for (int i = period; i < series.Count; i++)
            {
                var sum = 0.0;
                for (int j = i - period; j <= i; j++)
                {
                    sum += Math.Pow(series[j] - mean[i], 2.0);
                }
                ret[i] = sum / (period + 1.0);
            }
            mean.Dispose();
            return ret;
        }
        static public IList<double> stddev(IList<double> series, int period, IList<double> ret = null)
        {
            if (period < 0) { throw new Exception($"Period parameter ({period}) cannot be less than 0"); }
            if (period >= series.Count) { throw new Exception($"Period parameter ({period}) cannot be greater than or equal to length of series ({series.Count})"); }

            if (ret == null) { ret = ListPool<double>.Get(); }
            ret.AdjustLength<double>(series.Count);

            var variance = series.variance(period);
            ret = variance.pow(0.5, ret);

            variance.Dispose();

            return ret;
        }
        static public IList<double> normalize(IList<double> series, int period, IList<double> ret = null)
        {
            if (period < 0) { throw new Exception($"Period parameter ({period}) cannot be less than 0"); }
            if (period >= series.Count) { throw new Exception($"Period parameter ({period}) cannot be greater than or equal to length of series ({series.Count})"); }

            if (ret == null) { ret = ListPool<double>.Get(); }
            ret.AdjustLength<double>(series.Count);

            var std = series.stddev(period);
            var sma = series.sma(period);
            ret = series.sub(sma).div(std, ret);

            std.Dispose();
            sma.Dispose();

            return ret;
        }
        static public IList<double> volatility(IList<double> series, int period, IList<double> ret = null)
        {
            if (period < 0) { throw new Exception($"Period parameter ({period}) cannot be less than 0"); }
            if (period >= series.Count) { throw new Exception($"Period parameter ({period}) cannot be greater than or equal to length of series ({series.Count})"); }

            if (ret == null) { ret = ListPool<double>.Get(); }
            ret.AdjustLength<double>(series.Count);

            var roc1 = series.roc(1);
            var log = roc1.log(roc1);
            ret = log.stddev(period, ret);

            roc1.Dispose();
            log.Dispose();

            return ret;
        }
        static public IList<double> obv(IList<double> series, IList<double> prices, IList<double> ret = null)
        {
            if (series.Count != prices.Count) { throw new Exception($"Volume list length({series.Count}) must be the same as Price list length ({prices.Count})"); }

            if (ret == null) { ret = ListPool<double>.Get(); }
            ret.AdjustLength<double>(series.Count);

            // Calculate start 
            ret[0] = 0;

            // Go through the main loop
            for (int i = 1; i < series.Count; i++)
            {
                if (prices[i] > prices[i - 1])
                {
                    ret[i] += ret[i - 1] + series[i];
                    continue;
                }
                ret[i] += ret[i - 1] - series[i];
            }

            return ret;
        }
        static public IList<double> psl(IList<double> series, int period, IList<double> ret = null)
        {
            if (period < 0) { throw new Exception($"Period parameter ({period}) cannot be less than 0"); }
            if (period >= series.Count) { throw new Exception($"Period parameter ({period}) cannot be greater than or equal to length of series ({series.Count})"); }

            if (ret == null) { ret = ListPool<double>.Get(); }
            ret.AdjustLength<double>(series.Count);

            // Calculate start 
            int countUp = 0;
            int st = 1;
            while (st <= period)
            {
                if (series[st] > series[st - 1]) { countUp++; }
                if (st < period)
                {
                    ret[st] = 0.0;
                }
                else
                {
                    ret[st] = 0.0;
                    if (period > 0) { ret[st] = (double)((double)countUp / (double)period); }
                }
                st++;
            }


            // Go through the main loop
            for (int i = st; i < series.Count; i++)
            {
                if (period == 0) { ret[i] = 0.0; continue; }
                // take out if there was an increase "periods" ago
                if (series[i - period] > series[i - period - 1]) { countUp--; }
                // include if new one is an increase
                if (series[i] > series[i - 1]) { countUp++; }

                ret[i] = (double)((double)countUp / (double)period);
            }

            return ret;
        }
        static public IList<double> slope(IList<double> series, int period, IList<double> ret = null)
        {
            if (period < 1) { throw new Exception($"Period parameter ({period}) cannot be less than 1"); }
            if (period >= series.Count) { throw new Exception($"Period parameter ({period}) cannot be greater than or equal to length of series ({series.Count})"); }

            if (ret == null) { ret = ListPool<double>.Get(); }
            ret.AdjustLength<double>(series.Count);

            for (int i = 0; i < series.Count; i++)
            {
                if (i >= period)
                {
                    double n = (double)period;
                    var coeff1 = 12.0 / (n * (n + 1.0) * (n + 2.0));
                    var coeff2 = (n + 2.0) / 2.0;

                    double siy = 0.0;
                    double sy = 0.0;
                    var idx = i - period;
                    for (int p = 0; p <= period; p++)
                    {
                        siy += ((double)p + 1.0) * series[idx];
                        sy += series[idx];
                        idx++;
                    }

                    ret[i] = coeff1 * (siy - coeff2 * sy);
                }
                else
                {
                    ret[i] = 0.0;
                }
            }

            return ret;
        }
        static public IList<double> wilders_rsi(IList<double> series, int period, IList<double> ret = null)
        {
            if (period < 1) { throw new Exception($"Period parameter ({period}) cannot be less than 1"); }
            if (period >= series.Count) { throw new Exception($"Period parameter ({period}) cannot be greater than or equal to length of series ({series.Count})"); }

            if (ret == null) { ret = ListPool<double>.Get(); }
            ret.AdjustLength<double>(series.Count);

            double avgGain = 0.0;
            double avgLoss = 0.0;

            // 1. Seed the first 'period' periods (simple average)
            for (int i = 1; i <= period; i++)
            {
                double change = series[i] - series[i - 1];
                if (change > 0) { avgGain += change; }
                else { avgLoss += -change; }
            }

            avgGain /= period;
            avgLoss /= period;

            // First valid RSI at index = period
            if (avgLoss == 0) { ret[period] = 100.0; }
            else { ret[period] = 100.0 - (100.0 / (1.0 + (avgGain / avgLoss))); }

            // 2. Wilder smoothing for remaining periods
            for (int i = period + 1; i < series.Count; i++)
            {
                double change = series[i] - series[i - 1];
                double currentGain = change > 0 ? change : 0;
                double currentLoss = change < 0 ? -change : 0;

                // Wilder’s smoothing
                avgGain = (avgGain * (period - 1) + currentGain) / period;
                avgLoss = (avgLoss * (period - 1) + currentLoss) / period;

                if (avgLoss == 0) { ret[i] = 100.0; }
                else { ret[i] = 100.0 - (100.0 / (1.0 + (avgGain / avgLoss))); }
            }

            return ret;
        }
        static public IList<double> macd(IList<double> series, int periodLong = 26, int periodShort = 12, IList<double> ret = null)
        {
            if (periodLong <= periodShort) { throw new Exception($"PeriodLong parameter ({periodLong}) cannot be less than PeriodShort parameter ({periodShort})"); }
            if (periodShort < 0) { throw new Exception($"PeriodShort parameter ({periodShort}) cannot be less than 0"); }
            if (periodLong >= series.Count) { throw new Exception($"PeriodLong parameter ({periodLong}) cannot be greater than or equal to length of series ({series.Count})"); }

            if (ret == null) { ret = ListPool<double>.Get(); }
            ret.AdjustLength<double>(series.Count);

            var shortEma = series.ema(periodShort);
            var longEma = series.ema(periodLong);

            shortEma.sub(longEma, ret);

            shortEma.Dispose();
            longEma.Dispose();

            return ret;
        }
        static public IList<double> macd_signal(IList<double> series, int periodLong = 26, int periodShort = 12, int periodSignal = 9, IList<double> ret = null)
        {
            if (periodSignal < 0) { throw new Exception($"PeriodSignal parameter ({periodSignal}) cannot be less than 0"); }
            if (periodSignal >= series.Count) { throw new Exception($"PeriodSignal parameter ({periodSignal}) cannot be greater than or equal to length of series ({series.Count})"); }

            if (ret == null) { ret = ListPool<double>.Get(); }
            ret.AdjustLength<double>(series.Count);

            var macdline = series.macd(periodLong, periodShort);
            macdline.ema(periodSignal, ret);

            macdline.Dispose();

            return ret;
        }
        static public IList<double> macd_hist(IList<double> series, int periodLong = 26, int periodShort = 12, int periodSignal = 9, IList<double> ret = null)
        {
            if (periodSignal < 0) { throw new Exception($"PeriodSignal parameter ({periodSignal}) cannot be less than 0"); }
            if (periodSignal >= series.Count) { throw new Exception($"PeriodSignal parameter ({periodSignal}) cannot be greater than or equal to length of series ({series.Count})"); }

            if (ret == null) { ret = ListPool<double>.Get(); }
            ret.AdjustLength<double>(series.Count);

            var macdline = series.macd(periodLong, periodShort);
            var macdsignal = macdline.ema(periodSignal);

            macdline.sub(macdsignal, ret);

            macdline.Dispose();
            macdsignal.Dispose();

            return ret;
        }
        static public IList<double> yield(IList<double> series, int period, IList<double> ret = null)
        {
            if (period < 0) { throw new Exception($"Period parameter ({period}) cannot be less than 0"); }
            if (period >= series.Count) { throw new Exception($"Period parameter ({period}) cannot be greater than or equal to length of series ({series.Count})"); }

            if (ret == null) { ret = ListPool<double>.Get(); }
            ret.AdjustLength<double>(series.Count);

            for (int i = 0; i < series.Count; i++)
            {
                if (i < period)
                {
                    ret[i] = 0.0;
                }
                else
                {
                    var diff = series[i] - series[i - period];
                    if (diff == 0.0) { ret[i] = 0.0; }
                    else { ret[i] = diff / series[i - period]; }
                }
            }
            return ret;
        }
        static public IList<double> wilders_atr(IList<double> close, IList<double> low, IList<double> high, int period, IList<double> ret = null)
        {
            if (high == null || low == null || close == null)
            {
                throw new ArgumentNullException("High, Low, and Close lists cannot be null.");
            }

            int count = high.Count;

            if (period < 1 || period >= count)
            {
                throw new ArgumentException($"Period must be between 1 and {count - 1}");
            }

            if (ret == null) { ret = ListPool<double>.Get(); }
            ret.AdjustLength<double>(count);

            // Calculate True Range for all periods
            var trueRange = ListPool<double>.Get();
            trueRange.AdjustLength(count);

            trueRange[0] = high[0] - low[0];   // First TR uses High - Low only

            for (int i = 1; i < count; i++)
            {
                double hl = high[i] - low[i];
                double hc = Math.Abs(high[i] - close[i - 1]);
                double lc = Math.Abs(low[i] - close[i - 1]);

                trueRange[i] = Math.Max(hl, Math.Max(hc, lc));
            }

            // === Seed the first ATR (Simple Average of first 'period' True Ranges) ===
            double atr = 0.0;
            for (int i = 1; i <= period; i++)          // Start from 1 because TR[0] is usually skipped in many implementations
            {
                atr += trueRange[i];
            }
            atr /= period;

            ret[period] = atr;   // First valid ATR at index = period

            // === Apply Wilder's Smoothing for remaining periods ===
            for (int i = period + 1; i < count; i++)
            {
                atr = (atr * (period - 1) + trueRange[i]) / period;
                ret[i] = atr;
            }

            trueRange.Dispose();

            return ret;
        }
        static public IList<double> bollinger_band_width(IList<double> series, int period, double stddevs, IList<double> ret = null)
        {
            int count = series.Count;
            if (period < 1 || period >= count)
            {
                throw new ArgumentException($"Period must be between 1 and {count - 1}");
            }
            if (stddevs <= 0)
            {
                throw new ArgumentException($"Stddevs must be between > 0");
            }

            if (ret == null) { ret = ListPool<double>.Get(); }
            ret.AdjustLength<double>(count);

            var sma = series.sma(period);
            var stddev = series.stddev(period);

            for (int i = 0; i < count; i++)
            {
                if (i < period)
                {
                    ret[i] = 0.0;
                }
                else
                {
                    double upperBand = sma[i] + stddevs * stddev[i];
                    double lowerBand = sma[i] - stddevs * stddev[i];
                    double bandWidth = upperBand - lowerBand;
                    ret[i] = bandWidth / sma[i];
                }
            }

            sma.Dispose();
            stddev.Dispose();

            return ret;
        }
        static public IList<double> bollinger_band_pctb(IList<double> series, int period, double stddevs, IList<double> ret = null)
        {
            int count = series.Count;
            if (period < 1 || period >= count)
            {
                throw new ArgumentException($"Period must be between 1 and {count - 1}");
            }
            if (stddevs <= 0)
            {
                throw new ArgumentException($"Stddevs must be between > 0");
            }

            if (ret == null) { ret = ListPool<double>.Get(); }
            ret.AdjustLength<double>(count);

            var sma = series.sma(period);
            var stddev = series.stddev(period);

            for (int i = 0; i < count; i++)
            {
                if (i < period)
                {
                    ret[i] = 0.0;
                }
                else
                {
                    double upperBand = sma[i] + stddevs * stddev[i];
                    double lowerBand = sma[i] - stddevs * stddev[i];
                    ret[i] = (series[i] - lowerBand) / (upperBand - lowerBand);
                }
            }

            sma.Dispose();
            stddev.Dispose();

            return ret;
        }
        static public IList<double> wilders_adx(IList<double> close, IList<double> low, IList<double> high, int period, IList<double> ret = null)
        {
            if (high == null || low == null || close == null)
            {
                throw new ArgumentNullException("High, Low, and Close cannot be null.");
            }

            int count = high.Count;

            if (period < 2 || period >= count)
            {
                throw new ArgumentException($"Period must be between 2 and {count - 1}");
            }

            if (ret == null) { ret = ListPool<double>.Get(); }
            ret.AdjustLength<double>(count);

            if (count < period * 2)
            {
                return ret; // Not enough data
            }

            // === 1. Calculate True Range (TR) ===
            var tr = ListPool<double>.Get();
            tr.AdjustLength<double>(count);

            tr[0] = high[0] - low[0];

            for (int i = 1; i < count; i++)
            {
                double hl = high[i] - low[i];
                double hc = Math.Abs(high[i] - close[i - 1]);
                double lc = Math.Abs(low[i] - close[i - 1]);
                tr[i] = Math.Max(hl, Math.Max(hc, lc));
            }

            // === 2. Calculate +DM and -DM ===
            var plusDM = ListPool<double>.Get();
            plusDM.AdjustLength<double>(count);
            var minusDM = ListPool<double>.Get();
            minusDM.AdjustLength<double>(count);

            for (int i = 1; i < count; i++)
            {
                double upMove = high[i] - high[i - 1];
                double downMove = low[i - 1] - low[i];

                if (upMove > downMove && upMove > 0)
                {
                    plusDM[i] = upMove;
                }
                else
                {
                    if (downMove > upMove && downMove > 0)
                    {
                        minusDM[i] = downMove;
                    }
                }
                // else both remain 0
            }

            // === 3. Wilder’s Smoothing (TR14, +DM14, -DM14) ===
            double tr14 = 0.0;
            double plusDM14 = 0.0;
            double minusDM14 = 0.0;

            // Seed with simple average (first 'period' values)
            for (int i = 1; i <= period; i++)
            {
                tr14 += tr[i];
                plusDM14 += plusDM[i];
                minusDM14 += minusDM[i];
            }

            tr14 /= period;
            plusDM14 /= period;
            minusDM14 /= period;

            // === 4. Calculate +DI14, -DI14, DX, and ADX ===
            double adx = 0.0;
            int adxStartIndex = period * 2 - 1; // 27 when period = 14

            // First DX at index = period
            double plusDI = 100.0 * (plusDM14 / tr14);
            double minusDI = 100.0 * (minusDM14 / tr14);
            double dx = 100.0 * Math.Abs(plusDI - minusDI) / (plusDI + minusDI);

            // Seed ADX with average of first 14 DX values (from index period to period+13)
            double dxSum = dx;
            for (int i = period + 1; i < period + period; i++)
            {
                // Recalculate DI and DX for each subsequent bar
                tr14 = (tr14 * (period - 1) + tr[i]) / period;
                plusDM14 = (plusDM14 * (period - 1) + plusDM[i]) / period;
                minusDM14 = (minusDM14 * (period - 1) + minusDM[i]) / period;

                plusDI = 100.0 * (plusDM14 / tr14);
                minusDI = 100.0 * (minusDM14 / tr14);
                dx = 100.0 * Math.Abs(plusDI - minusDI) / (plusDI + minusDI);

                dxSum += dx;
            }

            adx = dxSum / period;
            ret[adxStartIndex] = adx;   // First valid ADX at index 27 (for period 14)

            // === Continue Wilder smoothing for ADX ===
            for (int i = adxStartIndex + 1; i < count; i++)
            {
                // Update smoothed TR, +DM, -DM
                tr14 = (tr14 * (period - 1) + tr[i]) / period;
                plusDM14 = (plusDM14 * (period - 1) + plusDM[i]) / period;
                minusDM14 = (minusDM14 * (period - 1) + minusDM[i]) / period;

                plusDI = 100.0 * (plusDM14 / tr14);
                minusDI = 100.0 * (minusDM14 / tr14);

                dx = 100.0 * Math.Abs(plusDI - minusDI) / (plusDI + minusDI);

                // Wilder smoothing for ADX
                adx = (adx * (period - 1) + dx) / period;
                ret[i] = adx;
            }

            tr.Dispose();
            plusDM.Dispose();
            minusDM.Dispose();

            return ret;
        }

    }
    static public partial class ListDoubleExtensions
    {
        static public IList<double> roc(this IList<double> series, int period, IList<double> ret = null) { return Oscillators.roc(series, period, ret); }
        static public IList<double> mom(this IList<double> series, int period, IList<double> ret = null) { return Oscillators.mom(series, period, ret); }
        static public IList<double> variance(this IList<double> series, int period, IList<double> ret = null) { return Oscillators.variance(series, period, ret); }
        static public IList<double> stddev(this IList<double> series, int period, IList<double> ret = null) { return Oscillators.stddev(series, period, ret); }
        static public IList<double> normalize(this IList<double> series, int period, IList<double> ret = null) { return Oscillators.normalize(series, period, ret); }
        static public IList<double> volatility(this IList<double> series, int period, IList<double> ret = null) { return Oscillators.volatility(series, period, ret); }
        static public IList<double> obv(this IList<double> series, IList<double> prices, IList<double> ret = null) { return Oscillators.obv(series, prices, ret); }
        static public IList<double> psl(this IList<double> series, int period, IList<double> ret = null) { return Oscillators.psl(series, period, ret); }
        static public IList<double> slope(this IList<double> series, int period, IList<double> ret = null) { return Oscillators.slope(series, period, ret); }
        static public IList<double> wilders_rsi(this IList<double> series, int period, IList<double> ret = null) { return Oscillators.wilders_rsi(series, period, ret); }
        static public IList<double> macd(this IList<double> series, int periodLong = 26, int periodShort = 12, IList<double> ret = null) { return Oscillators.macd(series, periodLong, periodShort, ret); }
        static public IList<double> macd_signal(this IList<double> series, int periodLong = 26, int periodShort = 12, int periodSignal = 9, IList<double> ret = null) { return Oscillators.macd_signal(series, periodLong, periodShort, periodSignal, ret); }
        static public IList<double> macd_hist(this IList<double> series, int periodLong = 26, int periodShort = 12, int periodSignal = 9, IList<double> ret = null) { return Oscillators.macd_hist(series, periodLong, periodShort, periodSignal, ret); }
        static public IList<double> yield(this IList<double> series, int period, IList<double> ret = null) { return Oscillators.yield(series, period, ret); }
        static public IList<double> wilders_atr(this IList<double> close, IList<double> low, IList<double> high, int period, IList<double> ret = null) { return Oscillators.wilders_atr(close, low, high, period, ret); }
        static public IList<double> bollinger_bands_width(this IList<double> series, int period, double stddevs, IList<double> ret = null) { return Oscillators.bollinger_band_width(series, period, stddevs, ret); }
        static public IList<double> bollinger_bands_pctb(this IList<double> series, int period, double stddevs, IList<double> ret = null) { return Oscillators.bollinger_band_pctb(series, period, stddevs, ret); }
        static public IList<double> wilders_adx(this IList<double> close, IList<double> low, IList<double> high, int period, IList<double> ret = null) { return Oscillators.wilders_adx(close, low, high, period, ret); }
    }
}
