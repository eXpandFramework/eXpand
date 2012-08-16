using DevExpress.ExpressApp;
using DevExpress.Persistent.BaseImpl;
using DevExpress.Xpo;
using Xpand.ExpressApp.AdditionalViewControlsProvider.Logic;
using FeatureCenter.Base;
using Xpand.ExpressApp.Attributes;

namespace FeatureCenter.Module.Win.Security.MemberLevel {
    [AdditionalViewControlsRule(Module.Captions.ViewMessage + " " + Module.Captions.HeaderMemberLevelSecurity, "1=1", "1=1",
        Module.Captions.ViewMessageMemberLevelSecurity, Position.Bottom, ViewType = ViewType.ListView,
        View = "MLSCustomer_ListView")]
    [AdditionalViewControlsRule(Module.Captions.Header + " " + Module.Captions.HeaderMemberLevelSecurity, "1=1", "1=1",
        Module.Captions.HeaderMemberLevelSecurity, Position.Top, ViewType = ViewType.ListView, View = "MLSCustomer_ListView")]
    [XpandNavigationItem(Module.Captions.Security + Module.Captions.MemberLevelSecurity, "MLSCustomer_ListView")]
    //    [DisplayFeatureModel("MLSCustomer_ListView", "MemberLevel")]
    public class MLSCustomer : BaseObject, ICustomer {
        string _city;
        string _description;
        string _name;

        public MLSCustomer(Session session)
            : base(session) {
        }
        #region ICustomer Members
        public string Name {
            get { return _name; }
            set { SetPropertyValue("Name", ref _name, value); }
        }

        [Size(SizeAttribute.Unlimited)]
        public string Description {
            get { return _description; }
            set { SetPropertyValue("Description", ref _description, value); }
        }

        public string City {
            get { return _city; }
            set { SetPropertyValue("City", ref _city, value); }
        }
        #endregion
    }
}