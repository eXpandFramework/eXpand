using DevExpress.ExpressApp;
using DevExpress.Persistent.BaseImpl;
using DevExpress.Xpo;
using eXpand.ExpressApp.AdditionalViewControlsProvider.Logic;

namespace FeatureCenter.Module.HighlightFocusedLayoutItem {
    [AdditionalViewControlsRule(Captions.ViewMessage + " " + Captions.HeaderHighlightFocusedLayoutItem, "1=1", "1=1", Captions.ViewMessageHighlightFocusedLayoutItem, Position.Bottom, ViewType = ViewType.DetailView)]
    [AdditionalViewControlsRule(Captions.Header + " " + Captions.HeaderHighlightFocusedLayoutItem, "1=1", "1=1", Captions.HeaderHighlightFocusedLayoutItem, Position.Top,ViewType = ViewType.DetailView)]
    public class HighlightFocusedLayoutItemObject : BaseObject {
        public HighlightFocusedLayoutItemObject(Session session) : base(session) {
        }

        public string Name {
            get { return GetPropertyValue<string>("Name"); }
            set { SetPropertyValue("Name", value); }
        }

        public string StringProperty {
            get { return GetPropertyValue<string>("StringProperty"); }
            set { SetPropertyValue("StringProperty", value); }
        }

        public int IntegerProperty {
            get { return GetPropertyValue<int>("IntegerProperty"); }
            set { SetPropertyValue("IntegerProperty", value); }
        }

        public bool BooleanProperty {
            get { return GetPropertyValue<bool>("BooleanProperty"); }
            set { SetPropertyValue("BooleanProperty", value); }
        }

        public HighlightFocusedLayoutItemObject ReferencedProperty {
            get { return GetPropertyValue<HighlightFocusedLayoutItemObject>("ReferencedProperty"); }
            set { SetPropertyValue("ReferencedProperty", value); }
        }
    }
}

