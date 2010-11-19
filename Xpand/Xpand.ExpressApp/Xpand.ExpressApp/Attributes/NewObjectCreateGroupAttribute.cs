using System;

namespace Xpand.ExpressApp.Attributes {
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