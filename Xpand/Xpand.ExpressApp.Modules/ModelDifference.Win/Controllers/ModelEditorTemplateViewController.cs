using System.Collections.Generic;
using System.Reflection;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.SystemModule;
using Xpand.ExpressApp.ModelDifference.DataStore.BaseObjects;
using Xpand.ExpressApp.ModelDifference.Win.PropertyEditors;
using System.Linq;

namespace Xpand.ExpressApp.ModelDifference.Win.Controllers
{
    public class ModelEditorTemplateViewController : ViewController<XpandDetailView>
    {
        public ModelEditorTemplateViewController()
        {
            TargetObjectType = typeof(ModelDifferenceObject);
        }

        protected override void OnFrameAssigned()
        {
            base.OnFrameAssigned();
            Frame.GetController<ListViewProcessCurrentObjectController>().ProcessCurrentObjectAction.ExecuteCompleted += ProcessCurrentObjectActionOnExecuted;
        }
        void ProcessCurrentObjectActionOnExecuted(object sender, ActionBaseEventArgs actionBaseEventArgs) {
            ((DetailView) actionBaseEventArgs.ShowViewParameters.CreatedView).GetItems<ModelEditorPropertyEditor>()[0].ModelEditorViewController.SetTemplate(Frame.Template);
        }

        protected override void OnDeactivating() {
            HideMainBarActions();
            base.OnDeactivating();
            Frame.GetController<ListViewProcessCurrentObjectController>().ProcessCurrentObjectAction.Executed -= ProcessCurrentObjectActionOnExecuted;
        }

        void HideMainBarActions() {
            var modelEditorViewController = View.GetItems<ModelEditorPropertyEditor>()[0].ModelEditorViewController;
            var actions =(List<ActionBase>)modelEditorViewController.GetType().GetField("mainBarActions",
                                                                                        BindingFlags.Instance | BindingFlags.NonPublic).GetValue(
                                                                                            modelEditorViewController);
            foreach (var actionBase in Frame.Template.DefaultContainer.Actions.Where(actions.Contains)) {
                actionBase.Active["Is Not ModelDiffs view"] = false;
            }
        }



    }
}
