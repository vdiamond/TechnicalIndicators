using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TALib
{
    static public class Conditions
    {
        static public IList<bool> Filter<T>(IList<T> series, Func<T, bool> func, IList<bool> ret = null)
        {
            if (ret == null) { ret = ListPool<bool>.Get(); }
            ret.AdjustLength<bool>(series.Count);

            for (int i = 0; i < series.Count; i++)
            {
                ret[i] = func(series[i]);
            }

            return ret;
        }
        static public double PortionTrue(IList<bool> series, IList<bool> filter = null)
        {
            int filterCount = (filter == null) ? series.Count : filter.Count; 

            if (series.Count == 0) { return 0.0; }
            if (series.Count != filterCount)
            { 
                throw new Exception($"Filter length ({filterCount}) must be equal to series length ({series.Count})"); 
            }

            int set = 0;
            int selected = 0;
            for (int i = 0; i < series.Count; i++)
            {
                if (filter != null)
                {
                    if (filter[i])
                    {
                        set++;
                        if (series[i]) { selected++; }
                    }
                    continue;
                }

                set++;
                if (series[i]) { selected++; }
            }

            if (set == 0) { return 0.0; }
            return ((double) selected) / ((double) set);
        }
        static public IList<bool> Condition<T>(IList<T> series, Func<T, bool> func, IList<bool> ret = null)
        {
            if (ret == null) { ret = ListPool<bool>.Get(); }
            ret.AdjustLength<bool>(series.Count);

            for (int i = 0; i < series.Count; i++)
            {
                ret[i] = func(series[i]);
            }   

            return ret;
        }
        static public IList<bool> Condition<T, U>(IList<T> series, IList<U> series2, Func<T, U, bool> func, IList<bool> ret = null)
        {
            if (ret == null) { ret = ListPool<bool>.Get(); }
            ret.AdjustLength<bool>(series.Count);

            for (int i = 0; i < series.Count; i++)
            {
                ret[i] = func(series[i], series2[i]);
            }

            return ret;
        }
        static public IList<bool> Condition<T, U, V>(IList<T> series, IList<U> series2, IList<V> series3, Func<T, U, V, bool> func, IList<bool> ret = null)
        {
            if (ret == null) { ret = ListPool<bool>.Get(); }
            ret.AdjustLength<bool>(series.Count);

            for (int i = 0; i < series.Count; i++)
            {
                ret[i] = func(series[i], series2[i], series3[i]);
            }

            return ret;
        }
        static public IList<R> Calc<P, R>(IList<P> series, Func<P, R> func, IList<R> ret = null)
        {
            if (ret == null) { ret = ListPool<R>.Get(); }
            ret.AdjustLength<R>(series.Count);

            for (int i = 0; i < series.Count; i++)
            {
                ret[i] = func(series[i]);
            }

            return ret;
        }
        static public IList<R> Calc<P, P1, R>(IList<P> series, IList<P1> series1, Func<P, P1, R> func, IList<R> ret = null)
        {
            if (ret == null) { ret = ListPool<R>.Get(); }
            ret.AdjustLength<R>(series.Count);

            for (int i = 0; i < series.Count; i++)
            {
                ret[i] = func(series[i], series1[i]);
            }

            return ret;
        }
        static public IList<R> Calc<P, P1, P2, R>(IList<P> series, IList<P1> series1, IList<P2> series2, Func<P, P1, P2, R> func, IList<R> ret = null)
        {
            if (ret == null) { ret = ListPool<R>.Get(); }
            ret.AdjustLength<R>(series.Count);

            for (int i = 0; i < series.Count; i++)
            {
                ret[i] = func(series[i], series1[i], series2[i]);
            }
            return ret;
        }

    }

    static public partial class ListDoubleExtensions
    {
        static public IList<bool> Filter<T>(this IList<T> series, Func<T, bool> func, IList<bool> ret = null) { return Conditions.Filter<T>(series, func, ret); }
        static public double PortionTrue(this IList<bool> series, IList<bool> filter = null) { return Conditions.PortionTrue(series, filter); }
        static public IList<bool> Condition<T>(this IList<T> series, Func<T, bool> func, IList<bool> ret = null) { return Conditions.Condition<T>(series, func, ret); }
        static public IList<bool> Condition<T, U>(this IList<T> series, IList<U> series2, Func<T, U, bool> func, IList<bool> ret = null) { return Conditions.Condition<T, U>(series, series2, func, ret); }
        static public IList<bool> Condition<T, U, V>(this IList<T> series, IList<U> series2, IList<V> series3, Func<T, U, V, bool> func, IList<bool> ret = null) { return Conditions.Condition<T, U, V>(series, series2, series3, func, ret); }
        static public IList<R> Calc<P, R>(this IList<P> series, Func<P, R> func, IList<R> ret = null) { return Conditions.Calc<P, R>(series, func, ret); }
        static public IList<R> Calc<P, P1, R>(this IList<P> series, IList<P1> series1, Func<P, P1, R> func, IList<R> ret = null) { return Conditions.Calc<P, P1, R>(series, series1, func, ret); }
        static public IList<R> Calc<P, P1, P2, R>(this IList<P> series, IList<P1> series1, IList<P2> series2, Func<P, P1, P2, R> func, IList<R> ret = null) { return Conditions.Calc<P, P1, P2, R>(series, series1, series2, func, ret); }
    }
}
