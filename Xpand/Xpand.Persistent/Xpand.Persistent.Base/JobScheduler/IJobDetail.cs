using System.Collections.Generic;

namespace Xpand.Persistent.Base.JobScheduler {
    public interface IJobDetail  {
        string Name { get; set; }
        bool Stateful { get; }
        string Description { get; set; }
        IXpandJob Job { get; set; }
        bool RequestsRecovery { get; set; }
        bool Volatile { get; set; }
        IList<IJobTrigger> JobTriggers { get; }
        IList<IJobListenerTrigger> JobListenerTriggers { get; }
    }
}