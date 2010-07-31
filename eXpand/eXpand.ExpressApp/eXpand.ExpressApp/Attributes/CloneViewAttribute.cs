using System;

namespace eXpand.ExpressApp.Attributes {
    public enum CloneViewType {
        DetailView,
        ListView,
        LookupListView
    }
    [AttributeUsage(AttributeTargets.Class|AttributeTargets.Method,AllowMultiple = true)]
    public class CloneViewAttribute : Attribute
    {
        readonly CloneViewType _viewType;
        readonly string _viewId;

        public CloneViewAttribute(CloneViewType viewType, string viewId)
        {
            _viewType = viewType;
            _viewId = viewId;
        }


        public string ViewId
        {
            get { return _viewId; }
        }


        public CloneViewType ViewType
        {
            get { return _viewType; }
        }

        public string DetailView { get; set; }
    }
}