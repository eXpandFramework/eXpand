using System;

namespace Xpand.Persistent.Base.JobScheduler {
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
    public class NonDataMapMember : Attribute {
    }
}