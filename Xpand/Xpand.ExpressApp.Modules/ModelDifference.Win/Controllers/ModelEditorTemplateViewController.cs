using System;
using System.Collections.Generic;
using System.Reflection;
using System.Windows.Forms;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using Xpand.ExpressApp.ModelDifference.DataStore.BaseObjects;
using Xpand.ExpressApp.ModelDifference.Win.PropertyEditors;
using System.Linq;

namespace Xpand.ExpressApp.ModelDifference.Win.Controllers
{
    public class ModelEditorTemplateViewController : ViewController<DetailView>
    {

        public ModelEditorTemplateViewController()
        {
            TargetObjectType = typeof (ModelDifferenceObject);
        }
        protected override void OnActivated()
        {
            base.OnActivated();
            ((Form) Frame.Template).Shown+=OnShown;
        }

        void OnShown(object sender, EventArgs eventArgs) {
            View.GetItems<ModelEditorPropertyEditor>()[0].ModelEditorViewController.SetTemplate(Frame.Template);
        }

        protected override void OnDeactivating()
        {
            HideMainBarActions();
            base.OnDeactivating();
            ((Form)Frame.Template).Shown -= OnShown;

        }

        void HideMainBarActions()
        {
            var modelEditorViewController = View.GetItems<ModelEditorPropertyEditor>()[0].ModelEditorViewController;
            var actions = (List<ActionBase>)modelEditorViewController.GetType().GetField("mainBarActions",
                                                                                        BindingFlags.Instance | BindingFlags.NonPublic).GetValue(
                                                                                            modelEditorViewController);
            foreach (var actionBase in Frame.Template.DefaultContainer.Actions.Where(actions.Contains))
            {
                actionBase.Active["Is Not ModelDiffs view"] = false;
            }
        }

    }
}
