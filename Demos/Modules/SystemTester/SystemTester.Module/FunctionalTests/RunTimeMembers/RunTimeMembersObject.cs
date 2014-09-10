using System.ComponentModel;
using DevExpress.ExpressApp.DC;
using DevExpress.Persistent.BaseImpl;
using DevExpress.Xpo;
using Xpand.ExpressApp.Attributes;
using Xpand.Persistent.Base.General.Model;

namespace SystemTester.Module.FunctionalTests.RunTimeMembers{
    [XpandNavigationItem("RuntimeMembers/Calculated", "RuntimeMembersCalculated_ListView")]
    [CloneView(CloneViewType.DetailView, "RuntimeMembersCalculated_DetailView")]
    [CloneView(CloneViewType.ListView, "RuntimeMembersCalculated_ListView",DetailView = "RuntimeMembersCalculated_DetailView")]

    [XpandNavigationItem("RuntimeMembers/Persistent", "RuntimeMembersPersistent_ListView")]
    [CloneView(CloneViewType.DetailView, "RuntimeMembersPersistent_DetailView")]
    [CloneView(CloneViewType.ListView, "RuntimeMembersPersistent_ListView", DetailView = "RuntimeMembersPersistent_DetailView")]
    public class RunTimeMembersObject : BaseObject{
        public RunTimeMembersObject(Session session)
            : base(session){
        }

        public string FirstName { get; set; }
        public string LastName { get; set; }

        [Browsable(false)]
        public Address HiddenAddress { get; set; }
    }

    [NonPersistent]
    public class RunTimeMembersObjectConfig {
         
    }
}
