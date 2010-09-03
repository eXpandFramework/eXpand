using DevExpress.ExpressApp;
using DevExpress.Persistent.BaseImpl;
using DevExpress.Xpo;
using eXpand.ExpressApp.AdditionalViewControlsProvider.Logic;
using eXpand.ExpressApp.Attributes;
using FeatureCenter.Base;

namespace FeatureCenter.Module.Security.MemberLevel {
    [AdditionalViewControlsRule(Captions.ViewMessage + " " + Captions.HeaderMemberLevelSecurity, "1=1", "1=1",
        Captions.ViewMessageMemberLevelSecurity, Position.Bottom, ViewType = ViewType.ListView,
        View = "MLSCustomer_ListView")]
    [AdditionalViewControlsRule(Captions.Header + " " + Captions.HeaderMemberLevelSecurity, "1=1", "1=1",
        Captions.HeaderMemberLevelSecurity, Position.Top, ViewType = ViewType.ListView, View = "MLSCustomer_ListView")]
    [XpandNavigationItem(Captions.Security + Captions.MemberLevelSecurity, "MLSCustomer_ListView")]
//    [DisplayFeatureModel("MLSCustomer_ListView", "MemberLevel")]
    public class MLSCustomer : BaseObject, ICustomer {
        string _city;
        string _description;
        string _name;

        public MLSCustomer(Session session) : base(session) {
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