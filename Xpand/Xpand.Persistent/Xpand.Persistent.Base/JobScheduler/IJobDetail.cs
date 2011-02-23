using System;
using System.Collections.Generic;

namespace Xpand.Persistent.Base.JobScheduler {
    public interface IJobDetail {
        string Name { get; set; }
        bool Stateful { get; }
        string Group { get; set; }
        string Description { get; set; }
        Type JobType { get; set; }
        bool RequestsRecovery { get; set; }
        bool Volatile { get; set; }
        bool Durable { get; set; }
        IList<IJobTrigger> JobTriggers { get; }
    }
}