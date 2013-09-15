using System;
using System.Linq;
using DevExpress.ExpressApp;
using Quartz;
using Quartz.Impl.Calendar;
using Xpand.Persistent.Base.JobScheduler.Calendars;
using Xpand.Persistent.Base.JobScheduler.Triggers;
using Fasterflect;

namespace Xpand.ExpressApp.JobScheduler {
    internal static class CalendarBuilder {
        public static void Build(IXpandJobTrigger trigger, IScheduler scheduler) {
            if (trigger.Calendar != null) {
                var calendar = (ICalendar)XafTypesInfo.Instance.FindTypeInfo(trigger.Calendar.CalendarTypeFullName).Type.CreateInstance();
                Initialize(calendar, trigger);
                scheduler.AddCalendar(trigger.Calendar.Name, calendar, true, true);
            }
        }

        static void Initialize(ICalendar calendar, IXpandJobTrigger trigger) {
            if (calendar is AnnualCalendar) {
                InitializeAnnual(calendar as AnnualCalendar, trigger.Calendar as IAnnualCalendar);
            } else if (calendar is HolidayCalendar) {
                InitializeHoliday(calendar as HolidayCalendar, trigger.Calendar as IHolidayCalendar);
            } else if (calendar is WeeklyCalendar) {
                InitializeWeekly(calendar as WeeklyCalendar, trigger.Calendar as IWeeklyCalendar);
            } else if (calendar is MonthlyCalendar) {
                InitializeMonthly(calendar as MonthlyCalendar, trigger.Calendar as IMonthlyCalendar);
            } else if (calendar is DailyCalendar) {
                InitializeDaily(calendar as DailyCalendar, trigger.Calendar as IDailyCalendar);
            } else if (calendar is CronCalendar) {
                InitializeCron(calendar as CronCalendar, trigger.Calendar as ICronCalendar);
            }
        }

        static void InitializeCron(CronCalendar cronCalendar, ICronCalendar calendar) {
            cronCalendar.TimeZone = TimeZoneInfo.FindSystemTimeZoneById(Persistent.Base.General.RegistryTimeZoneProvider.GetRegistryKeyNameByTimeZoneId(calendar.TimeZone));
            cronCalendar.CronExpression = new CronExpression(calendar.CronExpression);
        }

        static void InitializeDaily(DailyCalendar dailyCalendar, IDailyCalendar calendar) {
            dailyCalendar.TimeZone = TimeZoneInfo.FindSystemTimeZoneById(Persistent.Base.General.RegistryTimeZoneProvider.GetRegistryKeyNameByTimeZoneId(calendar.TimeZone));
            calendar.DateRanges.ToList().ForEach(range => dailyCalendar.SetTimeRange(range.StartPoint, range.EndPoint));
        }

        static void InitializeMonthly(MonthlyCalendar monthlyCalendar, IMonthlyCalendar calendar) {
            monthlyCalendar.TimeZone = TimeZoneInfo.FindSystemTimeZoneById(Persistent.Base.General.RegistryTimeZoneProvider.GetRegistryKeyNameByTimeZoneId(calendar.TimeZone));
            calendar.DaysExcluded.ForEach(i => monthlyCalendar.SetDayExcluded(i, true));
            calendar.DaysIncluded.ForEach(i => monthlyCalendar.SetDayExcluded(i, false));
        }

        static void InitializeWeekly(WeeklyCalendar weeklyCalendar, IWeeklyCalendar calendar) {
            weeklyCalendar.TimeZone = TimeZoneInfo.FindSystemTimeZoneById(Persistent.Base.General.RegistryTimeZoneProvider.GetRegistryKeyNameByTimeZoneId(calendar.TimeZone));
            calendar.DaysOfWeekExcluded.ForEach(week => weeklyCalendar.SetDayExcluded(week, true));
            calendar.DaysOfWeekIncluded.ForEach(week => weeklyCalendar.SetDayExcluded(week, false));
        }

        static void InitializeHoliday(HolidayCalendar holidayCalendar, IHolidayCalendar calendar) {
            holidayCalendar.TimeZone = TimeZoneInfo.FindSystemTimeZoneById(Persistent.Base.General.RegistryTimeZoneProvider.GetRegistryKeyNameByTimeZoneId(calendar.TimeZone));
            calendar.DatesExcluded.ForEach(holidayCalendar.AddExcludedDate);
        }

        static void InitializeAnnual(AnnualCalendar annualCalendar, IAnnualCalendar calendar) {
            annualCalendar.TimeZone = TimeZoneInfo.FindSystemTimeZoneById(Persistent.Base.General.RegistryTimeZoneProvider.GetRegistryKeyNameByTimeZoneId(calendar.TimeZone));
            calendar.DatesExcluded.ForEach(time => annualCalendar.SetDayExcluded(time, true));
            calendar.DatesIncluded.ForEach(time => annualCalendar.SetDayExcluded(time, false));
        }
    }
}