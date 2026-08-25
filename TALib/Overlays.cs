using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TALib
{
    static public class Overlays
    {
        static public IList<double> sma(IList<double> series, int period, IList<double> ret = null)
        {
            if (period < 0) { throw new Exception($"Period parameter ({period}) cannot be less than 0"); }
            if (period >= series.Count) { throw new Exception($"Period parameter ({period}) cannot be greater than or equal to length of series ({series.Count})"); }

            if (ret == null) { ret = ListPool<double>.Get(); }
            ret.AdjustLength<double>(series.Count);

            var total = 0.0;
            for (int i = 0; i < series.Count; i++)
            {
                total += series[i];
                if (i > period)
                {
                    total -= series[i - period - 1];
                }
                if (i < period)
                {
                    ret[i] = 0.0;
                }
                else
                {
                    ret[i] = total / (period + 1);
                }
            }

            return ret;
        }
        static public IList<double> ema(IList<double> series, int period, IList<double> ret = null)
        {
            if (period < 0) { throw new Exception($"Period parameter ({period}) cannot be less than 0"); }
            if (period >= series.Count) { throw new Exception($"Period parameter ({period}) cannot be greater than or equal to length of series ({series.Count})"); }

            if (ret == null) { ret = ListPool<double>.Get(); }
            ret.AdjustLength<double>(series.Count);

            // Calculate start 
            int st = 0;
            var total = 0.0;
            while (st <= period)
            {
                total += series[st];
                if (st < period)
                {
                    ret[st] = 0.0;
                }
                else
                {
                    ret[st] = total / (period + 1);
                }
                st++;
            }

            // Go through the main loop
            double mult = 2.0 / (period + 2);
            for (int i = st; i < series.Count; i++)
            {
                ret[i] = ((series[i] - ret[i - 1]) * mult) + ret[i - 1];
            }

            return ret;
        }
        static public IList<double> offset(IList<double> series, int offset, IList<double> ret = null)
        {
            if (Math.Abs(offset) >= series.Count) { throw new Exception($"Abs(offset) with offset value ({offset}) cannot be greater than or equal to length of series ({series.Count})"); }

            if (ret == null) { ret = ListPool<double>.Get(); }
            ret.AdjustLength<double>(series.Count);

            // Go through the main loop
            for (int i = 0; i < series.Count; i++)
            {
                int source = i - offset;
                if (source < 0 || source >= series.Count) { ret[i] = 0.0; continue; }
                ret[i] = series[source];
            }
            return ret;
        }
        static public IList<double> rpt(IList<double> series, IList<double> ret = null)
        {
            if (ret == null) { ret = ListPool<double>.Get(); }
            ret.AdjustLength<double>(series.Count);

            // if zero length, just return
            if (series.Count == 0) { return ret; }

            // for one
            ret[0] = series[0]; 

            // for length of two
            if (series.Count > 1) 
            { 
                if ((series[1] < series[0]))
                {
                    ret[1] = series[0];

                }
                else
                {
                    ret[1] = series[1];
                }
            }

            // set starting point
            double lastResistancePoint = series[1];

            // process the rest if any
            for (int i = 2; i < series.Count; i++)
            {
                ret[i] = lastResistancePoint;

                int right = i;
                int mid = i - 1;
                int left = i - 2;
                if ((series[right] <= series[mid] && series[left] <= series[mid]))
                {
                    lastResistancePoint = series[mid];
                }
            }

            return ret;
        }
        static public IList<double> spt(IList<double> series, IList<double> ret = null)
        {
            if (ret == null) { ret = ListPool<double>.Get(); }
            ret.AdjustLength<double>(series.Count);

            // if zero length, just return
            if (series.Count == 0) { return ret; }

            // for one
            ret[0] = series[0];

            // for length of two
            if (series.Count > 1)
            {
                if ((series[1] > series[0]))
                {
                    ret[1] = series[0];

                }
                else
                {
                    ret[1] = series[1];
                }
            }

            // set starting point
            double lastSupportPoint = series[1];

            // process the rest if any
            for (int i = 2; i < series.Count; i++)
            {
                ret[i] = lastSupportPoint;

                int right = i;
                int mid = i - 1;
                int left = i - 2;
                if ((series[right] >= series[mid] && series[left] >= series[mid]))
                {
                    lastSupportPoint = series[mid];
                }
            }

            return ret;
        }
    }

    static public partial class ListDoubleExtensions
    {
        static public IList<double> sma(this IList<double> series, int period, IList<double> ret = null) {  return Overlays.sma(series, period, ret); }
        static public IList<double> ema(this IList<double> series, int period, IList<double> ret = null) {  return Overlays.ema(series, period, ret); }
        static public IList<double> offset(this IList<double> series, int offset, IList<double> ret = null) {  return Overlays.offset(series, offset, ret); }
        static public IList<double> spt(this IList<double> series, IList<double> ret = null) { return Overlays.spt(series, ret); }
        static public IList<double> rpt(this IList<double> series, IList<double> ret = null) { return Overlays.rpt(series, ret); }
    }
}
