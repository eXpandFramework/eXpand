using System;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.SystemModule;

namespace Xpand.ExpressApp.WizardUI.Win {
    public class WizardTemplateController : ViewController {
        protected override void OnActivated() {
            base.OnActivated();

            Frame.GetController<NewObjectViewController>().NewObjectAction.Executed += Action_Executed;
            Frame.GetController<NewObjectViewController>().ObjectCreated += ObjectCreated;
            Frame.GetController<ListViewProcessCurrentObjectController>().ProcessCurrentObjectAction.Executed += Action_Executed;
            Frame.GetController<ShowNavigationItemController>().ShowNavigationItemAction.Executed += Action_Executed;
        }

        protected override void OnDeactivated() {
            Frame.GetController<NewObjectViewController>().NewObjectAction.Executed -= Action_Executed;
            Frame.GetController<NewObjectViewController>().ObjectCreated -= ObjectCreated;
            Frame.GetController<ListViewProcessCurrentObjectController>().ProcessCurrentObjectAction.Executed -= Action_Executed;
            Frame.GetController<ShowNavigationItemController>().ShowNavigationItemAction.Executed -= Action_Executed;
            base.OnDeactivated();
        }


        IObjectSpace _objectSpace;
        object _newObject;

        private void ObjectCreated(object sender, ObjectCreatedEventArgs e) {
            _objectSpace = e.ObjectSpace;
            _newObject = e.CreatedObject;
        }

        private void Action_Executed(object sender, ActionBaseEventArgs e) {
            var newObject = _newObject;
            var sourceView = View;
            e.CreateWizardView(_objectSpace, newObject, sourceView);

            _objectSpace = null;
            _newObject = null;
        }

        
    }
}
