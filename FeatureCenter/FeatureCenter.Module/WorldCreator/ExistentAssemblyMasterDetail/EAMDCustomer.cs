using DevExpress.Persistent.BaseImpl;
using DevExpress.Xpo;
using FeatureCenter.Base;
using Xpand.ExpressApp.AdditionalViewControlsProvider.Logic;
using Xpand.ExpressApp.Attributes;

namespace FeatureCenter.Module.WorldCreator.ExistentAssemblyMasterDetail {
    [AdditionalViewControlsRule(Captions.ViewMessage + " " + Captions.HeaderExistentAssemblyMasterDetail, "1=1", "1=1",
        Captions.ViewMessageExistentAssemblyMasterDetail, Position.Bottom, View = "EAMDCustomer_ListView")]
    [AdditionalViewControlsRule(Captions.Header + " " + Captions.HeaderExistentAssemblyMasterDetail, "1=1", "1=1",
        Captions.HeaderExistentAssemblyMasterDetail, Position.Top, View = "EAMDCustomer_ListView")]
    [XpandNavigationItem("WorldCreator/Existent Assembly/Master Detail", "EAMDCustomer_ListView")]
    [DisplayFeatureModel("EAMDCustomer_ListView", "ExistentAssemblyMasterDetailModelStore")]
    public class EAMDCustomer : BaseObject, ICustomer {
        string _city;
        string _description;
        string _name;

        public EAMDCustomer(Session session)
            : base(session) {
        }
        #region ICustomer Members
        public string Name {
            get { return _name; }
            set { SetPropertyValue("Name", ref _name, value); }
        }

        public string City {
            get { return _city; }
            set { SetPropertyValue("City", ref _city, value); }
        }

        public string Description {
            get { return _description; }
            set { SetPropertyValue("Description", ref _description, value); }
        }
        #endregion
    }
}