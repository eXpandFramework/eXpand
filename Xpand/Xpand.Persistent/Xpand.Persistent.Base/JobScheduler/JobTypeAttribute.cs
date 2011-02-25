using System;

namespace Xpand.Persistent.Base.JobScheduler {
    [AttributeUsage(AttributeTargets.Class,AllowMultiple = false)]
    public class JobTypeAttribute : Attribute {
        readonly Type _type;

        public JobTypeAttribute(Type type) {
            _type = type;
        }

        public Type Type {
            get {
                return _type;
            }
        }
    }
}