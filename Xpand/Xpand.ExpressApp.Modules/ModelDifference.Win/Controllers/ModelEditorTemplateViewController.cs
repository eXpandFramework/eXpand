using System.Collections.Generic;
using System.Reflection;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.Win.Templates;
using DevExpress.ExpressApp.Win.Templates.ActionContainers;
using DevExpress.XtraBars.Ribbon;
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


        protected override void OnDeactivating()
        {
            HideMainBarActions();
            base.OnDeactivating();
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

        protected override void OnViewControlsCreated()
        {
            base.OnViewControlsCreated();
            var template = Frame.Template as XtraFormTemplateBase;
            if (template != null)
            {
                SetTemplate();
                if (template.FormStyle == RibbonFormStyle.Ribbon)
                {
                    template.RibbonTransformer.Transformed += RibbonTransformer_Transformed;
                }
            }
        }

        private void RibbonTransformer_Transformed(object sender, System.EventArgs e)
        {
            ((ClassicToRibbonTransformer)sender).Transformed -= RibbonTransformer_Transformed;
            SetTemplate();
        }

        private void SetTemplate()
        {

            View.GetItems<ModelEditorPropertyEditor>()[0].ModelEditorViewController.SetTemplate(Frame.Template);
        }
    }
}
