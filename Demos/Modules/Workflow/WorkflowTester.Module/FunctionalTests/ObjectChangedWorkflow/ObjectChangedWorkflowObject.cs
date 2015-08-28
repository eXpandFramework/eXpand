using DevExpress.ExpressApp.Workflow;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.BaseImpl;
using DevExpress.Xpo;

namespace WorkflowTester.Module.FunctionalTests.ObjectChangedWorkflow{
    [DefaultClassOptions]
    public class ObjectChangedWorkflowObject : BaseObject{
        private string _propertyName;

        public ObjectChangedWorkflowObject(Session session) : base(session){
        }

        public string PropertyName{
            get { return _propertyName; }
            set { SetPropertyValue("PropertyName", ref _propertyName, value); }
        }
    }
}