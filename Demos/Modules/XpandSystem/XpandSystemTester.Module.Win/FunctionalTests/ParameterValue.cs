using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using DevExpress.Persistent.Base;

namespace XpandSystemTester.Module.Win.FunctionalTests {
    public class ParameterValue:WindowController {
        private readonly ParametrizedAction _parameterAction;

        public ParameterValue(){
            _parameterAction = new ParametrizedAction(this,GetType().Name,PredefinedCategory.ObjectsCreation,typeof(string));
            _parameterAction.Execute+=SimpleActionOnExecute;
        }

        private void SimpleActionOnExecute(object sender, ParametrizedActionExecuteEventArgs parametrizedActionExecuteEventArgs){
            
        }

        public ParametrizedAction ParameterAction{
            get { return _parameterAction; }
        }

        public string Value{
            get { return _parameterAction.Value as string; }
        }
    }
}
