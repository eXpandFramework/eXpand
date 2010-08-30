using System;
using eXpand.Persistent.Base.General;

namespace eXpand.ExpressApp.Attributes {
    [AttributeUsage(AttributeTargets.Class,AllowMultiple = true)]
    public class NavigationItemAttribute:Attribute, ISupportViewId {
        readonly string path;
        readonly string viewId;

        public NavigationItemAttribute(string path, string viewId) {
            this.path = path;
            this.viewId = viewId;
        }

        public NavigationItemAttribute(string path, string viewId, string objectKey) {
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