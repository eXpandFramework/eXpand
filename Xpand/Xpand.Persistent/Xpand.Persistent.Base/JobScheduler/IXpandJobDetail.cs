using System;

namespace Xpand.Persistent.Base.JobScheduler {
    public interface IXpandJobDetail {
        string Name { get; set; }
        bool Stateful { get; }
        string Group { get; set; }
        string Description { get; set; }
        Type JobType { get; set; }
        bool RequestsRecovery { get; set; }
        bool Volatile { get; set; }
        bool Durable { get; set; }
    }
}