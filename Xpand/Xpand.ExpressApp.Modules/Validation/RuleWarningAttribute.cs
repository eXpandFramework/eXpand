using System;

namespace Xpand.ExpressApp.Validation {
    public class RuleWarningAttribute : Attribute {
        readonly string _id;

        public RuleWarningAttribute(string id) {
            _id = id;
        }

        public string Id {
            get { return _id; }
        }
    }
}
