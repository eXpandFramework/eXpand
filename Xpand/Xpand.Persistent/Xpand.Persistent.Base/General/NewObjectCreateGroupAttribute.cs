using System;

namespace Xpand.Persistent.Base.General {
    public class NewObjectCreateGroupAttribute : Attribute {
        public NewObjectCreateGroupAttribute(string groupPath) {
            GroupPath = groupPath;
        }

        public string GroupPath { get; }
    }
}