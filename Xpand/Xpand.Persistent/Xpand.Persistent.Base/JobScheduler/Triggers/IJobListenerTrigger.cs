using System;

namespace Xpand.Persistent.Base.JobScheduler.Triggers {
    public enum JobListenerEvent {
        Executing,
        Vetoed,
        Executed
    }

    public interface IJobListenerTrigger : IJobDetails {
        JobListenerEvent Event { get; set; }
        Type JobType { get; set; }
        IJobSchedulerGroup Group { get; set; }
    }
}