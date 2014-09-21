using System.ComponentModel;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.BaseImpl;
using DevExpress.Xpo;

namespace SystemTester.Module.FunctionalTests.RunTimeMembers{
    [DefaultClassOptions]
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
