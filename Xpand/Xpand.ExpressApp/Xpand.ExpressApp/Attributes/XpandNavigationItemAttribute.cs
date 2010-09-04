using System;
using Xpand.Persistent.Base.General;

namespace Xpand.ExpressApp.Attributes {
    [AttributeUsage(AttributeTargets.Class,AllowMultiple = true)]
    public class XpandNavigationItemAttribute:Attribute, ISupportViewId {
        readonly string path;
        readonly string viewId;

        public XpandNavigationItemAttribute(string path, string viewId) {
            this.path = path;
            this.viewId = viewId;
        }

        public XpandNavigationItemAttribute(string path, string viewId, string objectKey) {
            this.path = path;
            this.viewId = viewId;
            ObjectKey = objectKey;
        }

        public string Path {
            get { return path; }
        }

        public string ViewId {
            get { return viewId; }
        }

        public string ObjectKey { get; set; }
    }
}