using DevExpress.Persistent.Base;
using DevExpress.Persistent.BaseImpl;
using DevExpress.Xpo;

namespace WorkflowTester.Module.FunctionalTests.WorldCreatorWorkflow{
    [DefaultClassOptions]
    public class WorldCreatorWorkflowObject : BaseObject{
        private string _propertyName;

        public WorldCreatorWorkflowObject(Session session) : base(session){
        }

        public string PropertyName{
            get { return _propertyName; }
            set { SetPropertyValue("PropertyName", ref _propertyName, value); }
        }
    }
}