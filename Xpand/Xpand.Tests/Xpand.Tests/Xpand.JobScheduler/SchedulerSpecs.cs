using System;
using System.Threading;
using DevExpress.ExpressApp;
using Machine.Specifications;
using Quartz;
using Quartz.Impl.Calendar;
using Xpand.ExpressApp.JobScheduler.QuartzExtensions;
using Xpand.Persistent.BaseImpl.JobScheduler;
using Xpand.Persistent.BaseImpl.JobScheduler.Calendars;
using Xpand.Persistent.BaseImpl.JobScheduler.Triggers;

namespace Xpand.Tests.Xpand.JobScheduler {
    public class When_trigger_time_pass : With_Scheduler {

        Establish context = () => {

            var objectSpace = ObjectSpaceInMemory.CreateNew();
            var xpandSimpleTrigger = objectSpace.CreateObject<XpandSimpleTrigger>();
            xpandSimpleTrigger.Name = "tr";
            var simpleTrigger = xpandSimpleTrigger.CreateTrigger("jb", typeof(DummyJob), null);

            simpleTrigger.StartTimeUtc = DateTime.UtcNow;

            Scheduler.ScheduleJob(simpleTrigger);
            Scheduler.Start();
        };

        Because of = () => Thread.Sleep(5000);

        It should_execute_the_job = () => JobExecutedCount.ShouldEqual(1);

        It should_shutdown_the_scheduler = () => Scheduler.Shutdown(false);
    }
    public class When_Job_with_Annual_calendar_scheduled : With_Scheduler {
        static AnnualCalendar _calendar;
        static XpandSimpleTrigger _simpleTrigger;
        static XpandAnnualCalendar _annualCalendar;
        static XpandJobDetail _jobDetail;

        Establish context = () => {
            var objectSpace = ObjectSpaceInMemory.CreateNew();
            _simpleTrigger = objectSpace.CreateObject<XpandSimpleTrigger>();
            _annualCalendar = objectSpace.CreateObject<XpandAnnualCalendar>();
            _annualCalendar.Name = "annualCalendarName";
            _simpleTrigger.Calendar = _annualCalendar;
            _simpleTrigger.Name = "tr";
            _annualCalendar.DatesExcluded.Add(DateTime.Today.AddDays(1));
            _annualCalendar.DatesIncluded.Add(DateTime.Today.AddDays(2));
            _jobDetail = objectSpace.CreateObject<XpandJobDetail>();
            _jobDetail.Name = "jb";
            var xpandJob = objectSpace.CreateObject<XpandJob>();
            _jobDetail.Job = xpandJob;
            xpandJob.JobType = typeof(DummyJob);
            xpandJob.Name = "DJ";

        };

        Because of = () => Scheduler.ScheduleJob(_simpleTrigger, _jobDetail, null);

        It should_add_an_annualCalendar_to_the_scheduler =
            () => {
                _calendar = Scheduler.GetCalendar(_annualCalendar.Name) as AnnualCalendar;
                _calendar.ShouldNotBeNull();
            };

        It should_add_the_excluded_dates_to_the_scheduler_calendar =
            () => _calendar.IsDayExcluded(DateTime.Today.AddDays(1)).ShouldBeTrue();
        It should_add_the_included_dates_to_the_scheduler_calendar =
            () => _calendar.IsDayExcluded(DateTime.Today.AddDays(2)).ShouldBeFalse();
        It should_shutdown_the_scheduler = () => Scheduler.Shutdown(false);
    }
    public class When_Job_with_Holiday_calendar_scheduled : With_Scheduler {
        static HolidayCalendar _calendar;
        static XpandSimpleTrigger _simpleTrigger;
        static XpandHolidayCalendar _xpandHolidayCalendar;
        static XpandJobDetail _jobDetail;

        Establish context = () => {
            var objectSpace = ObjectSpaceInMemory.CreateNew();
            _simpleTrigger = objectSpace.CreateObject<XpandSimpleTrigger>();
            _xpandHolidayCalendar = objectSpace.CreateObject<XpandHolidayCalendar>();
            _xpandHolidayCalendar.Name = "annualCalendarName";
            _simpleTrigger.Calendar = _xpandHolidayCalendar;
            _simpleTrigger.Name = "tr";
            _xpandHolidayCalendar.DatesExcluded.Add(DateTime.Today.AddDays(1));
            _jobDetail = objectSpace.CreateObject<XpandJobDetail>();
            _jobDetail.Name = "jb";
            var xpandJob = objectSpace.CreateObject<XpandJob>();
            _jobDetail.Job = xpandJob;
            xpandJob.JobType = typeof(DummyJob);
            xpandJob.Name = "DJ";

        };

        Because of = () => Scheduler.ScheduleJob(_simpleTrigger, _jobDetail, null);

        It should_add_an_holidayCalendar_to_the_scheduler =
            () => {
                _calendar = Scheduler.GetCalendar(_xpandHolidayCalendar.Name) as HolidayCalendar;
                _calendar.ShouldNotBeNull();
            };

        It should_add_the_excluded_dates_to_the_scheduler_calendar =
            () => _calendar.ExcludedDates.Contains(DateTime.Today.AddDays(1));
        It should_shutdown_the_scheduler = () => Scheduler.Shutdown(false);
    }
    [Ignore]
    public class When_Job_with_Weekcly_calendar_scheduled : With_Scheduler {
        static WeeklyCalendar _calendar;
        static XpandSimpleTrigger _simpleTrigger;
        static XpandWeeklyCalendar _xpandWeeklyCalendar;
        static XpandJobDetail _jobDetail;

        Establish context = () => {
            var objectSpace = ObjectSpaceInMemory.CreateNew();
            _simpleTrigger = objectSpace.CreateObject<XpandSimpleTrigger>();
            _xpandWeeklyCalendar = objectSpace.CreateObject<XpandWeeklyCalendar>();
            _xpandWeeklyCalendar.Name = "annualCalendarName";
            _simpleTrigger.Calendar = _xpandWeeklyCalendar;
            _simpleTrigger.Name = "tr";
            _xpandWeeklyCalendar.DaysOfWeekExcluded.Add(DateTime.Today.AddDays(2).DayOfWeek);
            _xpandWeeklyCalendar.DaysOfWeekIncluded.Add(DateTime.Today.DayOfWeek);
            _jobDetail = objectSpace.CreateObject<XpandJobDetail>();
            _jobDetail.Name = "jb";
            var xpandJob = objectSpace.CreateObject<XpandJob>();
            _jobDetail.Job = xpandJob;
            xpandJob.JobType = typeof(DummyJob);
            xpandJob.Name = "DJ";

        };

        Because of = () => Scheduler.StoreTrigger(_simpleTrigger, _jobDetail, null);

        It should_add_an_weekcly_Calendar_to_the_scheduler =
            () => {
                _calendar = Scheduler.GetCalendar(_xpandWeeklyCalendar.Name) as WeeklyCalendar;
                _calendar.ShouldNotBeNull();
            };

        It should_add_the_excluded_dates_to_the_scheduler_calendar =
            () => _calendar.IsDayExcluded(DateTime.Today.AddDays(1).DayOfWeek).ShouldBeTrue();
        It should_add_the_included_dates_to_the_scheduler_calendar =
            () => _calendar.IsDayExcluded(DateTime.Today.DayOfWeek).ShouldBeFalse();
        It should_shutdown_the_scheduler = () => Scheduler.Shutdown(false);
    }

    public class DummyStateFullJob : IJob {
        int _state;


        public void Execute(IJobExecutionContext context) {
            var data = context.JobDetail.JobDataMap;
            data.Put("excount", data.GetInt("excount") + 1);
            if (data.GetInt("excount") == 3)
                SameInstance = true;
        }

        public static bool SameInstance { get; set; }
    }
}
