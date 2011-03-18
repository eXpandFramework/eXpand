using System;
using System.Collections.Generic;

namespace Xpand.Persistent.Base.JobScheduler.Triggers {
    public interface ITriggerListenerTrigger {
        Type JobType { get; set; }
        IJobSchedulerGroup Group { get; set; }
        TriggerListenerEvent Event { get; set; }
        IList<IXpandJobTrigger> JobTriggers { get; }
    }
    public enum TriggerListenerEvent {
        Fired,
        Vetoed,
        Complete
    }

}