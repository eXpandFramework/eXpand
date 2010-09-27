using System;
using DevExpress.ExpressApp;
using DevExpress.Persistent.BaseImpl;
using DevExpress.Xpo;
using Xpand.ExpressApp.AdditionalViewControlsProvider.Logic;
using FeatureCenter.Base;
using Xpand.ExpressApp.Attributes;

namespace FeatureCenter.Module.WorldCreator.ExistentAssemblyMasterDetail
{
    [AdditionalViewControlsRule(Captions.ViewMessage + " " + Captions.HeaderExistentAssemblyMasterDetail, "1=1", "1=1",
        Captions.ViewMessageExistentAssemblyMasterDetail, Position.Bottom, View = "EAMDCustomer_ListView")]
    [AdditionalViewControlsRule(Captions.Header + " " + Captions.HeaderExistentAssemblyMasterDetail, "1=1", "1=1",
        Captions.HeaderExistentAssemblyMasterDetail, Position.Top, View = "EAMDCustomer_ListView")]
    [XpandNavigationItem("WorldCreator/Existent Assembly/Master Detail", "EAMDCustomer_ListView")]
    [DisplayFeatureModel("EAMDCustomer_ListView", "ExistentAssemblyMasterDetailModelStore")]
    public class EAMDCustomer:BaseObject,ICustomer
    {
        public EAMDCustomer(Session session) : base(session) {
        }


        string ICustomer.Name {
            get { return GetMemberValue("Name") as string; }
            set { SetMemberValue("Name",value); }
        }

        string ICustomer.City {
            get { return GetMemberValue("City") as string; }
            set { SetMemberValue("City",value); }
        }

        string ICustomer.Description {
            get { return GetMemberValue("Description") as string; }
            set { SetMemberValue("Description",value); }
        }
    }
}
