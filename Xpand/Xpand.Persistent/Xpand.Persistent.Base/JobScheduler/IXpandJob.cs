using System;

namespace Xpand.Persistent.Base.JobScheduler {
    public interface IXpandJob {
        string Name { get; set; }
        Type JobType { get; set; }
        IDataMap DataMap { get; set; }
    }
}