using System;

namespace Xpand.Persistent.Base.JobScheduler {
    public enum JobListenerEvent {
        Executing,
        Vetoed,
        Executed
    }

    public interface IJobListenerTrigger:IJobDetails {
        JobListenerEvent Event { get; set; }
        Type JobType { get; set; }
    }
}