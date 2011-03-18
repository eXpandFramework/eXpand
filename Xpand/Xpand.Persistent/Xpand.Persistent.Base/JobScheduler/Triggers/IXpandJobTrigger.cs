using System;
using System.Collections.Generic;
using Xpand.Persistent.Base.JobScheduler.Calendars;

namespace Xpand.Persistent.Base.JobScheduler.Triggers {
    public interface IJobDetails {
        IList<IXpandJobDetail> JobDetails { get; }

    }

    public interface IXpandJobTrigger : IJobDetails {
        string Name { get; set; }
        string Description { get; set; }

        ITriggerCalendar Calendar { get; set; }

        DateTime? EndTimeUtc { get; set; }

        DateTime StartTimeUtc { get; set; }

        TriggerPriority Priority { get; set; }
        IList<ITriggerListenerTrigger> TriggerListenerTriggers { get; }

    }
    public enum TriggerPriority {
        Default = 5
    }

}