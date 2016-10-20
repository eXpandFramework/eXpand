using System;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using DevExpress.Persistent.Base;

namespace SystemTester.Module.Web.FunctionalTests.ClientScript {
    public class ClientScriptController:ObjectViewController<ObjectView,ClientScriptObject>{
        public ClientScriptController(){
            var simpleAction = new SimpleAction(this,"ClientScript",PredefinedCategory.View);
            simpleAction.Execute+=SimpleActionOnExecute;
        }

        private void SimpleActionOnExecute(object sender, SimpleActionExecuteEventArgs simpleActionExecuteEventArgs){
            throw new NotImplementedException();
        }
    }
}
