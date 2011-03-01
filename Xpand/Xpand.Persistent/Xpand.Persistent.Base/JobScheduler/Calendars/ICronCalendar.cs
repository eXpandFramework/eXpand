
namespace Xpand.Persistent.Base.JobScheduler.Calendars {
    public interface ICronCalendar : ITriggerCalendar {
        string CronExpression { get; set; }
    }
}