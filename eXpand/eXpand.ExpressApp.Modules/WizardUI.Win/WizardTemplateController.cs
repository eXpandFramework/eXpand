using DevExpress.ExpressApp;
using DevExpress.ExpressApp.SystemModule;
using DevExpress.ExpressApp.Actions;

namespace eXpand.ExpressApp.WizardUI.Win
{
    public class WizardTemplateController : ViewController
    {
        protected override void OnActivated()
        {
            base.OnActivated();

            Frame.GetController<NewObjectViewController>().NewObjectAction.Executed += Action_Executed;
            Frame.GetController<NewObjectViewController>().ObjectCreating += NewObjectCreating;
            Frame.GetController<ListViewProcessCurrentObjectController>().ProcessCurrentObjectAction.Executed += Action_Executed;
        }

        protected override void OnDeactivating()
        {
            Frame.GetController<NewObjectViewController>().NewObjectAction.Executed -= Action_Executed;
            Frame.GetController<NewObjectViewController>().ObjectCreating -= NewObjectCreating;
            Frame.GetController<ListViewProcessCurrentObjectController>().ProcessCurrentObjectAction.Executed -= Action_Executed;

            base.OnDeactivating();
        }

        private void Action_Executed(object sender, ActionBaseEventArgs e)
        {
            if (e.ShowViewParameters.CreatedView is DetailView)
            {
                var modelWizard = (IModelDetailViewWizard)(e.ShowViewParameters.CreatedView as DetailView).Model;
                if (modelWizard != null && modelWizard.Wizard.Count > 0 && modelWizard.Wizard.ShowInWizard)
                {
                    e.ShowViewParameters.TargetWindow = TargetWindow.NewModalWindow;
                    e.ShowViewParameters.Context = "WizardDetailViewForm";
                }
            }
        }

        private void NewObjectCreating(object sender, ObjectCreatingEventArgs e)
        {
            e.ShowDetailView = true;
        }
    }
}
