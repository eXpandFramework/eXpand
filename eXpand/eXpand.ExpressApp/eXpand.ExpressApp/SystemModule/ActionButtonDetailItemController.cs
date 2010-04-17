using System;
using System.Linq;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.Model;
using DevExpress.Persistent.Base;
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

        private void ActionButtonDetailItemOnExecuted(object sender, EventArgs eventArgs) {
            var actionButtonDetailItem = ((ActionButtonDetailItem) sender);
            var simpleActions = Frame.Controllers.Cast<Controller>().SelectMany(controller1 => controller1.Actions).OfType<SimpleAction>();
            var action = simpleActions.Where(@base => @base.Id == ((IModelActionButtonDetailItem)actionButtonDetailItem.Model).ActionId.Id).Single();
            action.DoExecute();
        }
    }
}