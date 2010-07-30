using System;

namespace eXpand.ExpressApp.Attributes {
    [AttributeUsage(AttributeTargets.Class,AllowMultiple = true)]
    public class NavigationItemAttribute:Attribute
    {
        readonly string path;
        readonly string viewId;

        public NavigationItemAttribute(string path, string viewId) {
            this.path = path;
            this.viewId = viewId;
        }

        public string Path {
            get { return path; }
        }

        public string ViewId {
            get { return viewId; }
        }
    }
}