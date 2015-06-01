using DevExpress.Persistent.Base;
using DevExpress.Persistent.BaseImpl;
using DevExpress.Xpo;

namespace ModelArtifactStateTester.Module.FunctionalTests.Actions{
    [DefaultClassOptions]
    public class ActionsObject : BaseObject{
        private ActionsObject _actionsObject;

        public ActionsObject(Session session) : base(session){
        }

        public ActionsObject ActiObject{
            get { return _actionsObject; }
            set { SetPropertyValue("Controllers", ref _actionsObject, value); }
        }
    }
}