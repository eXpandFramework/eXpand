using DevExpress.Persistent.Base;
using DevExpress.Persistent.BaseImpl;
using DevExpress.Xpo;

namespace ModelArtifactStateTester.Module.FunctionalTests.Controllers{
    [DefaultClassOptions]
    public class ControllersObject : BaseObject{
        private ControllersObject _actionsObject;

        public ControllersObject(Session session) : base(session){
        }

        public ControllersObject Controllers {
            get { return _actionsObject; }
            set { SetPropertyValue("Views", ref _actionsObject, value); }
        }
    }
}