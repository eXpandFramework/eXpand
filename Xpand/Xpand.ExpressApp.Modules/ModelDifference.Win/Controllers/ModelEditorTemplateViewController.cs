using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.Win.Templates;
using DevExpress.ExpressApp.Win.Templates.ActionContainers;
using DevExpress.XtraBars.Ribbon;
using Xpand.ExpressApp.ModelDifference.DataStore.BaseObjects;
using Xpand.ExpressApp.ModelDifference.Win.PropertyEditors;

namespace Xpand.ExpressApp.ModelDifference.Win.Controllers
{
    public class ModelEditorTemplateViewController : ViewController<DetailView>
    {
        public ModelEditorTemplateViewController()
        {
            TargetObjectType = typeof(ModelDifferenceObject);
        }

        protected override void OnViewChanging(View view)
        {
            base.OnViewChanging(view);

            if (View != null)
            {
                View.Closing -= View_Closing;
            }

            if (view is DetailView && typeof(ModelDifferenceObject).IsAssignableFrom(view.ObjectTypeInfo.Type))
            {
                view.Closing += View_Closing;
            }
        }

        protected override void Dispose(bool disposing)
        {
            try
            {
                if (View != null)
                {
                    View.Closing -= View_Closing;
                }
            }
            finally
            {
                base.Dispose(disposing);
            }
        }

        void View_Closing(object sender, System.EventArgs e)
        {
            HideMainBarActions((DetailView) sender);
        }

        void HideMainBarActions(DetailView xpandDetailView)
        {
            var modelEditorViewController = xpandDetailView.GetItems<ModelEditorPropertyEditor>()[0].ModelEditorViewController;
            FieldInfo fieldInfo = modelEditorViewController.GetType().GetField("mainBarActions",
                                                                               BindingFlags.Instance | BindingFlags.NonPublic);
            if (fieldInfo != null) {
                var actions = (List<ActionBase>)fieldInfo.GetValue(modelEditorViewController);
                foreach (var actionBase in Frame.Template.DefaultContainer.Actions.Where(actions.Contains))
                {
                    actionBase.Active["Is Not ModelDiffs view"] = false;
                }
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
