using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Win.Templates;
using DevExpress.ExpressApp.Win.Templates.ActionContainers;
using DevExpress.XtraBars.Ribbon;
using eXpand.ExpressApp.ModelDifference.DataStore.BaseObjects;
using eXpand.ExpressApp.ModelDifference.Win.PropertyEditors;

namespace eXpand.ExpressApp.ModelDifference.Win.Controllers
{
    public class ModelEditorTemplateViewController : ViewController<DetailView>
    {
        public ModelEditorTemplateViewController()
        {
            this.TargetObjectType = typeof(ModelDifferenceObject);
        }

        protected override void OnViewControlsCreated()
        {
            base.OnViewControlsCreated();
            var template = this.Frame.Template as XtraFormTemplateBase;
            if (template != null)
            {
                if (template.FormStyle == RibbonFormStyle.Ribbon)
                {
                    template.RibbonTransformer.Transformed += this.RibbonTransformer_Transformed;
                }
                else
                {
                    this.SetTemplate();
                }
            }
        }

        private void RibbonTransformer_Transformed(object sender, System.EventArgs e)
        {
            (sender as ClassicToRibbonTransformer).Transformed -= this.RibbonTransformer_Transformed;
            this.SetTemplate();
        }

        private void SetTemplate()
        {
            this.View.GetItems<ModelEditorPropertyEditor>()[0].ModelEditorViewController.SetTemplate(
                this.Frame.Template);
        }
    }
}
