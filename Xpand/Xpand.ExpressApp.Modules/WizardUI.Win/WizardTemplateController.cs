using DevExpress.ExpressApp;
using DevExpress.ExpressApp.SystemModule;
using DevExpress.ExpressApp.Actions;

namespace Xpand.ExpressApp.WizardUI.Win
{
    public class WizardTemplateController : ViewController
    {
        protected override void OnActivated()
        {
            base.OnActivated();

            Frame.GetController<NewObjectViewController>().NewObjectAction.Executed += Action_Executed;
            Frame.GetController<NewObjectViewController>().ObjectCreated += ObjectCreated;
            Frame.GetController<ListViewProcessCurrentObjectController>().ProcessCurrentObjectAction.Executed += Action_Executed;
        }

        protected override void OnDeactivating()
        {
            Frame.GetController<NewObjectViewController>().NewObjectAction.Executed -= Action_Executed;
            Frame.GetController<NewObjectViewController>().ObjectCreated -= ObjectCreated;
            Frame.GetController<ListViewProcessCurrentObjectController>().ProcessCurrentObjectAction.Executed -= Action_Executed;

            base.OnDeactivating();
        }

        ObjectSpace objectSpace;
        object newObject;

        private void ObjectCreated(object sender, ObjectCreatedEventArgs e)
        {
            objectSpace = e.ObjectSpace;
            newObject = e.CreatedObject;
        }

        private void Action_Executed(object sender, ActionBaseEventArgs e)
        {
            IModelDetailViewWizard modelWizard = null;
            NewObjectViewController controller = null;

            if (e.ShowViewParameters.CreatedView != null)
            {
                modelWizard = e.ShowViewParameters.CreatedView.Model as IModelDetailViewWizard;
            }
            else if (e.ShowViewParameters.CreatedView == null && e.Action.Controller is NewObjectViewController)
            {
                controller = e.Action.Controller as NewObjectViewController;
                var viewID = this.Application.GetDetailViewId(controller.NewObjectAction.SelectedItem.Data as System.Type);
                modelWizard = this.Application.Model.Views[viewID] as IModelDetailViewWizard;
            }

            if (modelWizard != null && modelWizard.Wizard.Count > 0 && modelWizard.Wizard.ShowInWizard)
            {
                e.ShowViewParameters.TargetWindow = TargetWindow.NewModalWindow;
                e.ShowViewParameters.Context = "WizardDetailViewForm";
                if (e.ShowViewParameters.CreatedView == null)
                {
                    e.ShowViewParameters.CreatedView = this.Application.CreateDetailView(objectSpace, newObject, controller.View);
                }
            }

            objectSpace = null;
            newObject = null;
        }
    }
}
