using System;
using System.Linq;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using eXpand.ExpressApp.Editors;

namespace eXpand.ExpressApp.SystemModule {
    public class ActionButtonDetailItemController:ViewController<DetailView>
    {
        protected override void OnActivated() {
            base.OnActivated();
            foreach (var actionButtonDetailItem in View.GetItems<ActionButtonDetailItem>()){
                actionButtonDetailItem.Executed+=ActionButtonDetailItemOnExecuted;
            }
        }

        void ActionButtonDetailItemOnExecuted(object sender, EventArgs eventArgs) {
            var actionButtonDetailItem = ((ActionButtonDetailItem) sender);
            var simpleActions = Frame.Controllers.Cast<Controller>().SelectMany(controller1 => controller1.Actions).OfType<SimpleAction>();
            var action = simpleActions.Where(@base => @base.Id == actionButtonDetailItem.Info.GetAttributeValue("ActionId")).Single();
            action.DoExecute();
        }
    }
}