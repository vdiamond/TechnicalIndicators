
namespace TALib
{

    static public class Basic
    {
        static public IList<double> add(IList<double> series, IList<double> source, IList<double> ret = null)
        {
            if (source.Count != series.Count) { throw new Exception($"Lists need to be the same length to add ({series.Count},{source.Count})"); }
            if (ret == null) { ret = ListPool<double>.Get(); }
            ret.AdjustLength<double>(series.Count);
            for (int i = 0; i < series.Count; i++) { ret[i] = series[i] + source[i]; }
            return ret;
        }
        static public IList<double> add(IList<double> series, double value, IList<double> ret = null)
        {
            if (ret == null) { ret = ListPool<double>.Get(); }
            ret.AdjustLength<double>(series.Count);
            for (int i = 0; i < series.Count; i++) { ret[i] = series[i] + value; }
            return ret;
        }
        static public IList<double> sub(IList<double> series, IList<double> source, IList<double> ret = null)
        {
            if (source.Count != series.Count) { throw new Exception($"Lists need to be the same length to subtract ({series.Count},{source.Count})"); }
            if (ret == null) { ret = ListPool<double>.Get(); }
            ret.AdjustLength<double>(series.Count);
            for (int i = 0; i < series.Count; i++) { ret[i] = series[i] - source[i]; }
            return ret;
        }
        static public IList<double> sub(IList<double> series, double value, IList<double> ret = null)
        {
            if (ret == null) { ret = ListPool<double>.Get(); }
            ret.AdjustLength<double>(series.Count);
            for (int i = 0; i < series.Count; i++) { ret[i] = series[i] - value; }
            return ret;
        }
        static public IList<double> mul(IList<double> series, IList<double> source, IList<double> ret = null)
        {
            if (source.Count != series.Count) { throw new Exception($"Lists need to be the same length to multiply ({series.Count},{source.Count})"); }
            if (ret == null) { ret = ListPool<double>.Get(); }
            ret.AdjustLength<double>(series.Count);
            for (int i = 0; i < series.Count; i++) { ret[i] = series[i] * source[i]; }
            return ret;
        }
        static public IList<double> mul(IList<double> series, double value, IList<double> ret = null)
        {
            if (ret == null) { ret = ListPool<double>.Get(); }
            ret.AdjustLength<double>(series.Count);
            for (int i = 0; i < series.Count; i++) { ret[i] = series[i] * value; }
            return ret;
        }
        static public IList<double> div(IList<double> series, IList<double> source, IList<double> ret = null)
        {
            if (source.Count != series.Count) { throw new Exception($"Lists need to be the same length to divide ({series.Count},{source.Count})"); }
            if (ret == null) { ret = ListPool<double>.Get(); }
            ret.AdjustLength<double>(series.Count);
            for (int i = 0; i < series.Count; i++) { if (source[i] != 0.0) { ret[i] = series[i] / source[i]; } else { ret[i] = 0.0; } }
            return ret;
        }
        static public IList<double> div(IList<double> series, double value, IList<double> ret = null)
        {
            if (ret == null) { ret = ListPool<double>.Get(); }
            ret.AdjustLength<double>(series.Count);
            for (int i = 0; i < series.Count; i++) { if (value != 0.0) { ret[i] = series[i] / value; } else { ret[i] = 0.0; } }
            return ret;
        }
        static public IList<double> abs(IList<double> series, IList<double> ret = null)
        {
            if (ret == null) { ret = ListPool<double>.Get(); }
            ret.AdjustLength<double>(series.Count);
            for (int i = 0; i < series.Count; i++) { ret[i] = Math.Abs(series[i]); }
            return ret;
        }
        static public IList<double> acos(IList<double> series, IList<double> ret = null)
        {
            if (ret == null) { ret = ListPool<double>.Get(); }
            ret.AdjustLength<double>(series.Count);
            for (int i = 0; i < series.Count; i++) { ret[i] = Math.Acos(series[i]); }
            return ret;
        }
        static public IList<double> acosh(IList<double> series, IList<double> ret = null)
        {
            if (ret == null) { ret = ListPool<double>.Get(); }
            ret.AdjustLength<double>(series.Count);
            for (int i = 0; i < series.Count; i++) { ret[i] = Math.Acosh(series[i]); }
            return ret;
        }
        static public IList<double> asin(IList<double> series, IList<double> ret = null)
        {
            if (ret == null) { ret = ListPool<double>.Get(); }
            ret.AdjustLength<double>(series.Count);
            for (int i = 0; i < series.Count; i++) { ret[i] = Math.Asin(series[i]); }
            return ret;
        }
        static public IList<double> asinh(IList<double> series, IList<double> ret = null)
        {
            if (ret == null) { ret = ListPool<double>.Get(); }
            ret.AdjustLength<double>(series.Count);
            for (int i = 0; i < series.Count; i++) { ret[i] = Math.Asinh(series[i]); }
            return ret;
        }
        static public IList<double> atan(IList<double> series, IList<double> ret = null)
        {
            if (ret == null) { ret = ListPool<double>.Get(); }
            ret.AdjustLength<double>(series.Count);
            for (int i = 0; i < series.Count; i++) { ret[i] = Math.Atan(series[i]); }
            return ret;
        }
        static public IList<double> atanh(IList<double> series, IList<double> ret = null)
        {
            if (ret == null) { ret = ListPool<double>.Get(); }
            ret.AdjustLength<double>(series.Count);
            for (int i = 0; i < series.Count; i++) { ret[i] = Math.Atanh(series[i]); }
            return ret;
        }
        static public IList<double> cbrt(IList<double> series, IList<double> ret = null)
        {
            if (ret == null) { ret = ListPool<double>.Get(); }
            ret.AdjustLength<double>(series.Count);
            for (int i = 0; i < series.Count; i++) { ret[i] = Math.Cbrt(series[i]); }
            return ret;
        }
        static public IList<double> cos(IList<double> series, IList<double> ret = null)
        {
            if (ret == null) { ret = ListPool<double>.Get(); }
            ret.AdjustLength<double>(series.Count);
            for (int i = 0; i < series.Count; i++) { ret[i] = Math.Cos(series[i]); }
            return ret;
        }
        static public IList<double> cosh(IList<double> series, IList<double> ret = null)
        {
            if (ret == null) { ret = ListPool<double>.Get(); }
            ret.AdjustLength<double>(series.Count);
            for (int i = 0; i < series.Count; i++) { ret[i] = Math.Cosh(series[i]); }
            return ret;
        }
        static public IList<double> exp(IList<double> series, IList<double> ret = null)
        {
            if (ret == null) { ret = ListPool<double>.Get(); }
            ret.AdjustLength<double>(series.Count);
            for (int i = 0; i < series.Count; i++) { ret[i] = Math.Exp(series[i]); }
            return ret;
        }
        static public IList<double> ilogb(IList<double> series, IList<double> ret = null)
        {
            if (ret == null) { ret = ListPool<double>.Get(); }
            ret.AdjustLength<double>(series.Count);
            for (int i = 0; i < series.Count; i++) { ret[i] = Math.ILogB(series[i]); }
            return ret;
        }
        static public IList<double> log(IList<double> series, IList<double> ret = null)
        {
            if (ret == null) { ret = ListPool<double>.Get(); }
            ret.AdjustLength<double>(series.Count);
            for (int i = 0; i < series.Count; i++) { if (series[i] != 0.0) { ret[i] = Math.Log(series[i]); } else { ret[i] = 0.0; } }
            return ret;
        }
        static public IList<double> log10(IList<double> series, IList<double> ret = null)
        {
            if (ret == null) { ret = ListPool<double>.Get(); }
            ret.AdjustLength<double>(series.Count);
            for (int i = 0; i < series.Count; i++) { ret[i] = Math.Log10(series[i]); }
            return ret;
        }
        static public IList<double> log2(IList<double> series, IList<double> ret = null)
        {
            if (ret == null) { ret = ListPool<double>.Get(); }
            ret.AdjustLength<double>(series.Count);
            for (int i = 0; i < series.Count; i++) { ret[i] = Math.Log2(series[i]); }
            return ret;
        }
        static public IList<double> sin(IList<double> series, IList<double> ret = null)
        {
            if (ret == null) { ret = ListPool<double>.Get(); }
            ret.AdjustLength<double>(series.Count);
            for (int i = 0; i < series.Count; i++) { ret[i] = Math.Sin(series[i]); }
            return ret;
        }
        static public IList<double> sinh(IList<double> series, IList<double> ret = null)
        {
            if (ret == null) { ret = ListPool<double>.Get(); }
            ret.AdjustLength<double>(series.Count);
            for (int i = 0; i < series.Count; i++) { ret[i] = Math.Sinh(series[i]); }
            return ret;
        }
        static public IList<double> sqrt(IList<double> series, IList<double> ret = null)
        {
            if (ret == null) { ret = ListPool<double>.Get(); }
            ret.AdjustLength<double>(series.Count);
            for (int i = 0; i < series.Count; i++) { ret[i] = Math.Sqrt(series[i]); }
            return ret;
        }
        static public IList<double> tan(IList<double> series, IList<double> ret = null)
        {
            if (ret == null) { ret = ListPool<double>.Get(); }
            ret.AdjustLength<double>(series.Count);
            for (int i = 0; i < series.Count; i++) { ret[i] = Math.Tan(series[i]); }
            return ret;
        }
        static public IList<double> tanh(IList<double> series, IList<double> ret = null)
        {
            if (ret == null) { ret = ListPool<double>.Get(); }
            ret.AdjustLength<double>(series.Count);
            for (int i = 0; i < series.Count; i++) { ret[i] = Math.Tanh(series[i]); }
            return ret;
        }
        static public IList<double> pow(IList<double> series, IList<double> source, IList<double> ret = null)
        {
            if (source.Count != series.Count) { throw new Exception($"Lists need to be the same length to add ({series.Count},{source.Count})"); }
            if (ret == null) { ret = ListPool<double>.Get(); }
            ret.AdjustLength<double>(series.Count);
            for (int i = 0; i < series.Count; i++) { ret[i] = Math.Pow(series[i], source[i]); }
            return ret;
        }
        static public IList<double> pow(IList<double> series, double value, IList<double> ret = null)
        {
            if (ret == null) { ret = ListPool<double>.Get(); }
            ret.AdjustLength<double>(series.Count);
            for (int i = 0; i < series.Count; i++) { ret[i] = Math.Pow(series[i], value); }
            return ret;
        }
        static public IList<double> sigma(IList<double> series, double k, IList<double> ret = null)
        {
            if (ret == null) { ret = ListPool<double>.Get(); }
            ret.AdjustLength<double>(series.Count);
            for (int i = 0; i < series.Count; i++) { ret[i] = 1.0 / (1.0 + Math.Exp(series[i] * k * -1.0)); }
            return ret;
        }
        static public IList<double> nop(IList<double> series, IList<double> ret = null)
        {
            if (ret == null) { ret = ListPool<double>.Get(); }
            ret.AdjustLength<double>(series.Count);
            for (int i = 0; i < series.Count; i++) { ret[i] = series[i]; }
            return ret;
        }

    }
    static public partial class ListDoubleExtensions
    {
        static public IList<double> add(this IList<double> series, IList<double> source, IList<double> ret = null) { return Basic.add(series, source, ret); }
        static public IList<double> add(this IList<double> series, double value, IList<double> ret = null) { return Basic.add(series, value, ret); }
        static public IList<double> sub(this IList<double> series, IList<double> source, IList<double> ret = null) {  return Basic.sub(series, source, ret); }
        static public IList<double> sub(this IList<double> series, double value, IList<double> ret = null) {  return Basic.sub(series, value, ret); }
        static public IList<double> mul(this IList<double> series, IList<double> source, IList<double> ret = null) { return Basic.mul(series, source, ret); }
        static public IList<double> mul(this IList<double> series, double value, IList<double> ret = null) { return Basic.mul(series, value, ret); }
        static public IList<double> div(this IList<double> series, IList<double> source, IList<double> ret = null) {  return Basic.div(series, source, ret); }
        static public IList<double> div(this IList<double> series, double value, IList<double> ret = null) {  return Basic.div(series, value, ret ); }
        static public IList<double> abs(this IList<double> series, IList<double> ret = null) {  return Basic.abs(series, ret); }
        static public IList<double> acos(this IList<double> series, IList<double> ret = null) {  return Basic.acos(series, ret); }
        static public IList<double> acosh(this IList<double> series, IList<double> ret = null) {  return Basic.acosh(series, ret); }
        static public IList<double> asin(this IList<double> series, IList<double> ret = null) {  return Basic.asin(series, ret); }
        static public IList<double> asinh(this IList<double> series, IList<double> ret = null) {  return Basic.asinh(series, ret); }
        static public IList<double> atan(this IList<double> series, IList<double> ret = null) {  return Basic.atan(series, ret); }
        static public IList<double> atanh(this IList<double> series, IList<double> ret = null) {  return Basic.atanh(series, ret); }    
        static public IList<double> cbrt(this IList<double> series, IList<double> ret = null) { return Basic.cbrt(series, ret); }
        static public IList<double> cos(this IList<double> series, IList<double> ret = null) {  return Basic.cos(series, ret); }    
        static public IList<double> cosh(this IList<double> series, IList<double> ret = null) {  return Basic.cosh(series, ret); }
        static public IList<double> exp(this IList<double> series, IList<double> ret = null) {  return Basic.exp(series, ret); }
        static public IList<double> ilogb(this IList<double> series, IList<double> ret = null) {  return Basic.ilogb(series, ret); }
        static public IList<double> log(this IList<double> series, IList<double> ret = null) {  return Basic.log(series, ret); }
        static public IList<double> log10(this IList<double> series, IList<double> ret = null) {  return Basic.log10(series, ret); }    
        static public IList<double> log2(this IList<double> series, IList<double> ret = null) {  return Basic.log2(series, ret); }  
        static public IList<double> sin(this IList<double> series, IList<double> ret = null) {  return Basic.sin(series, ret); }    
        static public IList<double> sinh(this IList<double> series, IList<double> ret = null) {  return Basic.sinh(series, ret); }
        static public IList<double> sqrt(this IList<double> series, IList<double> ret = null) {  return Basic.sqrt(series, ret); }
        static public IList<double> tan(this IList<double> series, IList<double> ret = null) {  return Basic.tan(series, ret); } 
        static public IList<double> tanh(this IList<double> series, IList<double> ret = null) {  return Basic.tanh(series, ret); }
        static public IList<double> pow(this IList<double> series, List<double> source, IList<double> ret = null) {  return Basic.pow(series, source, ret); }
        static public IList<double> pow(this IList<double> series, double value, IList<double> ret = null) { return Basic.pow(series, value, ret); }
        static public IList<double> sigma(this IList<double> series, double k, IList<double> ret = null) { return Basic.sigma(series, k, ret); }
        static public IList<double> nop(this IList<double> series, IList<double> ret = null) { return Basic.nop(series, ret); }

    }
}
