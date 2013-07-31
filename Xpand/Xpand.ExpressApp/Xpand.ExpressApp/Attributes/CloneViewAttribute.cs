using System;
using Xpand.Persistent.Base.General;

namespace Xpand.ExpressApp.Attributes {
    public enum CloneViewType {
        DetailView,
        ListView,
        LookupListView
    }
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true,Inherited = false)]
    public sealed class CloneViewAttribute : Attribute, ISupportViewId {
        readonly CloneViewType _viewType;
        readonly string _viewId;
        readonly bool _isDefault;

        public CloneViewAttribute(CloneViewType viewType, string viewId,bool isDefault=false) {
            _viewType = viewType;
            _viewId = viewId;
            _isDefault = isDefault;
        }

        public bool IsDefault {
            get { return _isDefault; }
        }

        public string ViewId {
            get { return _viewId; }
        }


        public CloneViewType ViewType {
            get { return _viewType; }
        }

        public string DetailView { get; set; }
    }
}