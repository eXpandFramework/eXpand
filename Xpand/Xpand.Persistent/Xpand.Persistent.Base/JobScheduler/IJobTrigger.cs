using System;
using System.Collections.Generic;

namespace Xpand.Persistent.Base.JobScheduler {
    public interface IJobDetails  {
        IList<IJobDetail> JobDetails { get; }
    }

    public interface IJobTrigger:IJobDetails {
        string Name { get; set; }
        string Description { get; set; }

        string CalendarName { get; set; }

        bool Volatile { get; set; }

        DateTime? EndTimeUtc { get; set; }

        DateTime StartTimeUtc { get; set; }

        TriggerPriority Priority { get; set; }
        
    }
    public enum TriggerPriority {
        Default = 5
    }

}