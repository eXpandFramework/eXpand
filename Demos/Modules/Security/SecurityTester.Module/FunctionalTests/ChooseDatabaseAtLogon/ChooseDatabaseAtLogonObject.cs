using DevExpress.Persistent.Base;
using DevExpress.Persistent.BaseImpl;
using DevExpress.Xpo;
using DevExpress.Xpo.DB.Helpers;
using Xpand.Persistent.Base.General;

namespace SecurityTester.Module.FunctionalTests.ChooseDatabaseAtLogon{
    [DefaultClassOptions]
    public class ChooseDatabaseAtLogonObject : BaseObject{

        public ChooseDatabaseAtLogonObject(Session session)
            : base(session){
        }

        public string Database{
            get { return new ConnectionStringParser(ApplicationHelper.Instance.Application.ConnectionString).GetPartByName("Initial Catalog"); }
            
        }
    }
}