using System;
using System.Linq;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using Xpand.ExpressApp.Editors;

namespace Xpand.ExpressApp.SystemModule {
    public class ActionButtonDetailItemController : ViewController<DetailView> {
        protected override void OnActivated() {
            base.OnActivated();
            foreach (var actionButtonDetailItem in View.GetItems<ActionButtonDetailItem>()) {
                actionButtonDetailItem.Executed += ActionButtonDetailItemOnExecuted;
                var modelActionButton = ((IModelActionButton)actionButtonDetailItem.Model);
                var id = modelActionButton.ActionId.Id;
                var actionBase = Frame.Template.GetContainers().Select(container => container.Actions).SelectMany(bases => bases).Where(@base => @base.Id == id).FirstOrDefault();
                if (actionBase != null)
                    actionBase.Active["ShowInContainer"] = modelActionButton.ShowInContainer;
            }
        }

        private void ActionButtonDetailItemOnExecuted(object sender, EventArgs eventArgs) {
            var actionButtonDetailItem = ((ActionButtonDetailItem)sender);
            var simpleActions = Frame.Controllers.Cast<Controller>().SelectMany(controller1 => controller1.Actions).OfType<SimpleAction>();
            var action = simpleActions.Where(@base => @base.Id == ((IModelActionButton)actionButtonDetailItem.Model).ActionId.Id).Single();
            var b = action.Active["ShowInContainer"];
            action.Active["ShowInContainer"] = true;
            action.DoExecute();
            action.Active["ShowInContainer"] = b;
        }
    }
}