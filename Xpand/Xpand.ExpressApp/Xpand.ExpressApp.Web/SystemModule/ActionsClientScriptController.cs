using System;
using System.ComponentModel;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Web;
using DevExpress.ExpressApp.Web.Templates;
using DevExpress.ExpressApp.Web.Templates.ActionContainers;
using DevExpress.ExpressApp.Web.Templates.ActionContainers.Menu;
using Xpand.Persistent.Base.General.Model;

namespace Xpand.Persistent.Base.General.Web {
    [ModelAbstractClass]
    public interface IModelActionClientScript:IModelAction{
        [Category(AttributeCategoryNameProvider.Xpand)]
        string ClientScript { get; set; }

        
        [Category(AttributeCategoryNameProvider.Xpand)]
        [Localizable(true)]
        string ConfirmationMsg { get; set; }
        [Browsable(false)]
        string Script { get; set; }
    }

    public class ActionsClientConfirmationController:ViewController,IModelExtender,IXafCallbackHandler{
        public event EventHandler<ActionClientConfirmationArgs> ClientConfirmation;
        public void ExtendModelInterfaces(ModelInterfaceExtenders extenders){
            extenders.Add<IModelAction,IModelActionClientScript>();
        }

        protected override void OnFrameAssigned(){
            base.OnFrameAssigned();
            foreach (var action in Frame.Actions<SimpleAction>()){
                var modelActionClientScript = ((IModelActionClientScript) action.Model());
                if (!string.IsNullOrEmpty(modelActionClientScript.ClientScript))
                    modelActionClientScript.Script= modelActionClientScript.ClientScript;
            }
        }

        public void ProcessAction(string parameter) {
            var strings = parameter.Split(';');
            var action = Application.Model.ActionDesign.Actions[strings[1]].ToAction(Frame);
            OnClientConfirmation(new ActionClientConfirmationArgs(bool.Parse(strings[0]), action));
        }

        protected virtual void OnClientConfirmation(ActionClientConfirmationArgs e) {
            ClientConfirmation?.Invoke(this, e);
        }
        protected override void OnViewControlsCreated(){
            base.OnViewControlsCreated();
            XafCallbackManager callbackManager = ((ICallbackManagerHolder)WebWindow.CurrentRequestPage).CallbackManager;
            callbackManager.RegisterHandler(GetType().FullName, this);
            foreach (var action in Frame.Actions<SimpleAction>()) {
                var modelActionClientScript = ((IModelActionClientScript)action.Model());
                if (!string.IsNullOrEmpty(modelActionClientScript?.ConfirmationMsg)){
                    var clientScript = callbackManager.GetScript(GetType().FullName, "ret +';" +action.Id+ "'");
                    modelActionClientScript.Script = "var ret= confirm('" +modelActionClientScript.ConfirmationMsg+ "'); " + clientScript;
                }
            }
        }
    }

    public class ActionsClientScriptController:WindowController {
        protected override void OnDeactivated() {
            Frame.ProcessActionContainer -= Frame_ProcessActionContainer;
            base.OnDeactivated();
        }

        protected override void OnActivated() {
            base.OnActivated();
            Frame.ProcessActionContainer += Frame_ProcessActionContainer;
        }

        void Frame_ProcessActionContainer(object sender, ProcessActionContainerEventArgs e) {
            var webActionContainer = e.ActionContainer as WebActionContainer;
            if (webActionContainer != null) {
                webActionContainer.Owner.CreateCustomMenuActionItem+=OwnerOnCreateCustomMenuActionItem;
            }
        }

        public class SimpleActionMenuActionItem : DevExpress.ExpressApp.Web.Templates.ActionContainers.Menu.SimpleActionMenuActionItem {
            public SimpleActionMenuActionItem(SimpleAction action) : base(action){
            }

            public override string GetScript(XafCallbackManager callbackManager, string controlID, string indexPath){
                return ClientClickScript;
            }
        }
        private void OwnerOnCreateCustomMenuActionItem(object sender, CreateCustomMenuActionItemEventArgs e){
            var modelAction = Application.Model.ActionDesign.Actions[e.Action.Id];
            var script = ((IModelActionClientScript) modelAction)?.Script;
            if (!string.IsNullOrEmpty(script)){
                var action = modelAction.ToAction(Frame);
                var actionItem = new SimpleActionMenuActionItem((SimpleAction) action) {
                    ClientClickScript = script
                };
                e.ActionItem = actionItem;
            }
        }
    }

    public class ActionClientConfirmationArgs : EventArgs{
        public ActionClientConfirmationArgs(bool confirmed, ActionBase action){
            Confirmed = confirmed;
            Action = action;
        }

        public bool Confirmed { get; }

        public ActionBase Action { get; }
    }
}
