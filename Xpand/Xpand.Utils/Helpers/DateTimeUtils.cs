using System;
using System.Collections.Generic;
using System.Linq;

namespace Xpand.Utils.Helpers {
    public static class DateTimeUtils {

        public static IEnumerable<DateTime> GetDates() {
            return GetDates(DateTime.Today.Year);
        }

        public static IEnumerable<DateTime> GetDates(int year) {
            return Enumerable.Range(1, 12).SelectMany(month => GetDates(year, month)).ToList();
        }

        public static IEnumerable<DateTime> GetDates(int year, int month) {
            return Enumerable.Range(1, DateTime.DaysInMonth(year, month))
                             .Select(day => new DateTime(year, month, day));
        }
        public static int GetYears(this TimeSpan timespan) {
            return (int)(timespan.Days / 365.2425);
        }
        public static int GetMonths(this TimeSpan timespan) {
            return (int)(timespan.Days / 30.436875);
        }

        public static DateTime UnixTimeStampToDateTime(this long unixTimeStamp) {
            var dtDateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0);
            return dtDateTime.AddSeconds(unixTimeStamp).ToLocalTime();
        }

        public static string RelativeDate(this DateTime theDate) {
            var ts = new TimeSpan(DateTime.UtcNow.Ticks - theDate.Ticks);
            double delta = Math.Abs(ts.TotalSeconds);

            if (delta < 60) {
                return ts.Seconds == 1
                           ? "one second ago"
                           : ts.Seconds
                             + " seconds ago";
            }
            if (delta < 120) {
                return "a minute ago";
            }
            if (delta < 2700) // 45 * 60
            {
                return ts.Minutes + " minutes ago";
            }
            if (delta < 5400) // 90 * 60
            {
                return "an hour ago";
            }
            if (delta < 86400) {
                // 24 * 60 * 60
                return ts.Hours + " hours ago";
            }
            if (delta < 172800) {
                // 48 * 60 * 60
                return "yesterday";
            }
            if (delta < 2592000) {
                // 30 * 24 * 60 * 60
                return ts.Days + " days ago";
            }
            if (delta < 31104000) {
                // 12 * 30 * 24 * 60 * 60
                int months = System.Convert.ToInt32(Math.Floor((double)ts.Days / 30));
                return months <= 1 ? "one month ago" : months + " months ago";
            }
            int years = System.Convert.ToInt32(Math.Floor((double)ts.Days / 365));
            return years <= 1 ? "one year ago" : years + " years ago";
        }
    }
}