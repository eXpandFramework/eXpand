using System;
using Xpand.Persistent.Base.General;

namespace Xpand.ExpressApp.Attributes {
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface, AllowMultiple = true)]
    public class XpandNavigationItemAttribute : Attribute, ISupportViewId {
        readonly int _index = -1;
        readonly string _path;
        readonly string _viewId;

        public XpandNavigationItemAttribute(string path, string viewId, int index = -1) {
            _path = path;
            _viewId = viewId;
            _index = index;
        }

        public XpandNavigationItemAttribute(string path, string viewId, string objectKey, int index = -1) {
            _path = path;
            _viewId = viewId;
            _index = index;
            ObjectKey = objectKey;
        }

        public int Index {
            get { return _index; }
        }

        public string Path {
            get { return _path; }
        }

        public string ObjectKey { get; set; }

        public string ViewId {
            get { return _viewId; }
        }
    }
}