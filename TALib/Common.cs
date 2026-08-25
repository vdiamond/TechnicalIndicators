using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TALib
{
// -------------------------------------------- Approach for sliding Window approach --------------------------------------------------
//
// Meaning of sliding window: function(double[] series, int period)
// - Period of 0 means there is no period lookback, i,e, we use the same value for current and lookback. e.g. gfunc(series[x], series[x])
// - Period of n means we lookback n periods. Example, if period=3, we would consider series[9] and series[6], i.e. series[x] amd series [x-3]
    public static class VolatilityConverter
    {
        public static double DailyToAnnual365 { get { return Math.Sqrt(365); } }
        public static double DailyToAnnual { get { return Math.Sqrt(252); } }
        public static double DailyToMonthly365 { get { return Math.Sqrt(30); } }
        public static double DailyToMonthly { get { return Math.Sqrt(22); } }
        public static double DailyToWeekly365 { get { return Math.Sqrt(7); } }
        public static double DailyToWeekly { get { return Math.Sqrt(5); } }
        public static double WeeklyToAnnual { get { return Math.Sqrt(52); } }
        public static double MonthlyToAnnual { get { return Math.Sqrt(12); } }
    }

}
