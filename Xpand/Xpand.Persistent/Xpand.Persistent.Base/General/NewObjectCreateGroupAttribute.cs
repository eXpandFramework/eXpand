using System;

namespace Xpand.Persistent.Base.General {
    public class NewObjectCreateGroupAttribute : Attribute {
        readonly string _groupPath;

        public NewObjectCreateGroupAttribute(string groupPath) {
            _groupPath = groupPath;
        }

        public string GroupPath {
            get {
                return _groupPath;
            }
        }
    }
}