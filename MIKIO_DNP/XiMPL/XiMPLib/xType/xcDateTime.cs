using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XiMPLib.xType {
    public class xcDateTime {
        public static DateTime fromTicks(double mSeconds) {
            return new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc).AddMilliseconds(mSeconds);
        }

        public static DateTime fromTicks(string mSeconds) {
            double miliSeconds = (double)xcDecimal.Parse(mSeconds);
            return fromTicks(miliSeconds);
        }

        public static DateTime fromDateString(string date) {
            return DateTime.Parse(date);
        }

        public static long utcNowToLong() {
            return utcDateToLong(DateTime.UtcNow);
        }

        public static long utcDateToLong(DateTime utcDate) {
            return (long)(utcDate - new DateTime(1970, 1, 1)).TotalMilliseconds;
        }

        public static DateTime utcDate(long dateTime) {
            return TimeZone.CurrentTimeZone.ToLocalTime(new DateTime(1970, 1, 1).AddMilliseconds(dateTime));
        }

        public static String toDateString(DateTime utcDate) {
            return utcDate.ToString("yyyyMMdd");
        }
    }
}
