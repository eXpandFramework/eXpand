using System;
using System.Collections.Generic;
using System.Linq;

namespace Xpand.Utils.Helpers {
    public class DateTimeUtils {
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
    }
}
