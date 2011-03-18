using System.Collections.Generic;
using Xpand.Persistent.Base.JobScheduler.Triggers;

namespace Xpand.Persistent.Base.JobScheduler {
    public interface IXpandJobDetail {
        string Name { get; set; }
        bool Stateful { get; }
        string Description { get; set; }
        IXpandJob Job { get; set; }
        bool RequestsRecovery { get; set; }
        IList<IXpandJobTrigger> JobTriggers { get; }
        IList<IJobListenerTrigger> JobListenerTriggers { get; }
        IJobSchedulerGroup Group { get; set; }
        IDataMap JobDataMap { get; set; }
    }
}