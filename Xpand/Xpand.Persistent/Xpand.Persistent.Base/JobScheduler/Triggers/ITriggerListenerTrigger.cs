using System;
using System.Collections.Generic;

namespace Xpand.Persistent.Base.JobScheduler.Triggers {
    public interface ITriggerListenerTrigger {
        Type JobType { get; set; }

        TriggerListenerEvent Event { get; set; }
        IList<IJobTrigger> JobTriggers { get; }
    }
    public enum TriggerListenerEvent {
        Fired,
        Vetoed,
        Missfire,
        Complete
    }

}