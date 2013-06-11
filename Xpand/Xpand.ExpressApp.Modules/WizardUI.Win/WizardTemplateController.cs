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
            IModelDetailViewWizard modelWizard = null;
            if (e.ShowViewParameters.CreatedView != null) {
                modelWizard = e.ShowViewParameters.CreatedView.Model as IModelDetailViewWizard;
            } else if (e.ShowViewParameters.CreatedView == null && e.Action.Controller is NewObjectViewController) {
                var viewID = Application.GetDetailViewId(((SingleChoiceActionExecuteEventArgs)e).SelectedChoiceActionItem.Data as Type);
                modelWizard = Application.Model.Views[viewID] as IModelDetailViewWizard;
            }

            if (CanCreateView(e, modelWizard)){
                e.ShowViewParameters.TargetWindow = TargetWindow.NewModalWindow;
                e.ShowViewParameters.Context = "WizardDetailViewForm";
                if (e.ShowViewParameters.CreatedView == null)
                    e.ShowViewParameters.CreatedView = Application.CreateDetailView(_objectSpace, _newObject, View);
            }

            _objectSpace = null;
            _newObject = null;
        }

        bool CanCreateView(ActionBaseEventArgs e, IModelDetailViewWizard modelWizard) {
            var canCreate = modelWizard != null && modelWizard.Wizard.Count > 0 && modelWizard.Wizard.ShowInWizard;
            return canCreate && (!(e.Action.Controller is NewObjectViewController) || modelWizard.Wizard.NewObjectsOnly);
        }
    }
}
