using System.ComponentModel;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Web.Templates;
using DevExpress.ExpressApp.Web.Templates.ActionContainers;
using DevExpress.ExpressApp.Web.Templates.ActionContainers.Menu;
using Xpand.Persistent.Base.General.Model;

namespace Xpand.Persistent.Base.General.Controllers.Actions {
    public interface IModelActionClientScript{
        [Category(AttributeCategoryNameProvider.Xpand)]
        string ClientScript { get; set; }
    }

    public class ActionsClientScriptController:WindowController,IModelExtender {
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
            var clientScript = ((IModelActionClientScript)modelAction).ClientScript;
            if (!string.IsNullOrEmpty(clientScript)){
                var action = modelAction.ToAction(Frame);
                var actionItem = new SimpleActionMenuActionItem((SimpleAction) action) {
                    ClientClickScript = ((IModelActionClientScript) modelAction).ClientScript
                };
                e.ActionItem = actionItem;
            }

        }

        public void ExtendModelInterfaces(ModelInterfaceExtenders extenders){
            extenders.Add<IModelAction,IModelActionClientScript>();
        }
    }
}
