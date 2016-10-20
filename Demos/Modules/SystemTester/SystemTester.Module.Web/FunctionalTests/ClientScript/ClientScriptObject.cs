using DevExpress.Persistent.Base;
using DevExpress.Persistent.BaseImpl;
using DevExpress.Xpo;

namespace SystemTester.Module.Web.FunctionalTests.ClientScript {
    [DefaultClassOptions]
    public class ClientScriptObject:BaseObject {
        public ClientScriptObject(Session session) : base(session){
        }

        bool _confirmed;

        public bool Confirmed{
            get { return _confirmed; }
            set { SetPropertyValue(nameof(Confirmed), ref _confirmed, value); }
        }

        string _action;

        public string Action{
            get { return _action; }
            set { SetPropertyValue(nameof(Action), ref _action, value); }
        }

    }
}
