using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using DevExpress.Persistent.Base;
using Xpand.Persistent.Base.General.Web;

namespace SystemTester.Module.Web.FunctionalTests.ClientScript {
    public class ClientScriptController:ObjectViewController<ObjectView,ClientScriptObject>{
        public ClientScriptController(){
            new SimpleAction(this,"ClientScript",PredefinedCategory.View);
            
            new SimpleAction(this,"ClientConfirmation",PredefinedCategory.View);
        }


        protected override void OnActivated(){
            base.OnActivated();
            Frame.GetController<ActionsClientConfirmationController>().ClientConfirmation+=OnClientConfirmation;
        }

        protected override void OnDeactivated(){
            base.OnDeactivated();
            Frame.GetController<ActionsClientConfirmationController>().ClientConfirmation -= OnClientConfirmation;
        }

        private void OnClientConfirmation(object sender, ActionClientConfirmationArgs e){
            var clientScriptObject = ((ClientScriptObject) View.CurrentObject);
            clientScriptObject.Confirmed = e.Confirmed;
            clientScriptObject.Action = e.Action.Id;
        }

    }
}
