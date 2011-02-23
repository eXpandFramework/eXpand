using System;

namespace Xpand.Persistent.Base.JobScheduler {
    public interface IXpandTrigger {
        string Name { get; set; }
        string Group { get; set; }


        string JobName { get; set; }

        string JobGroup { get; set; }

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