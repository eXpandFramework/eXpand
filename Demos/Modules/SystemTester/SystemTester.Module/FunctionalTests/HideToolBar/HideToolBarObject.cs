using DevExpress.Persistent.Base;
using DevExpress.Persistent.BaseImpl;
using DevExpress.Xpo;

namespace SystemTester.Module.FunctionalTests.HideToolBar {
    [DefaultClassOptions]
    public class HideToolBarObject:BaseObject {
        public HideToolBarObject(Session session) : base(session){
        }

        
        [Association("HideNavigationObject-HideToolBarObjectChilds")]
        public XPCollection<HideToolBarObjectChild> Children{
            get {
                return GetCollection<HideToolBarObjectChild>("Children");
            }
        }
    }

    public class HideToolBarObjectChild:BaseObject {
        public HideToolBarObjectChild(Session session) : base(session){
        }

        
        private HideToolBarObject _hideToolBarObject;

        [Association("HideNavigationObject-HideToolBarObjectChilds")]
        public HideToolBarObject HideToolBarObject {
            get {
                return _hideToolBarObject;
            }
            set {
                SetPropertyValue("HideNavigationObject", ref _hideToolBarObject, value);
            }
        }
    }
}
