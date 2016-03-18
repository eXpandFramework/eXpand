using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using DevExpress.Persistent.Base;

namespace EmailTester.Module.FunctionalTests.EmailFromAction {
    public class EmailFromAction:ViewController<DetailView> {
        public EmailFromAction(){
            var simpleAction = new SimpleAction(this,"SendEmail",PredefinedCategory.View);
            simpleAction.Execute+=SimpleActionOnExecute;
        }

        private void SimpleActionOnExecute(object sender, SimpleActionExecuteEventArgs simpleActionExecuteEventArgs){
            
        }
    }
}
