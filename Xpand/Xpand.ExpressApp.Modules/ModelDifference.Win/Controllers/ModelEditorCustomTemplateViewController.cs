using DevExpress.ExpressApp;
using DevExpress.ExpressApp.SystemModule;
using DevExpress.ExpressApp.Win.SystemModule;
using DevExpress.ExpressApp.Win.Templates.ActionContainers;
using Xpand.ExpressApp.ModelDifference.DataStore.BaseObjects;
using Xpand.ExpressApp.ModelDifference.Win.Templates;

namespace Xpand.ExpressApp.ModelDifference.Win.Controllers
{
    public class ModelEditorCustomTemplateViewController : ViewController<XpandListView>
    {
        public ModelEditorCustomTemplateViewController()
        {
            TargetObjectType = typeof(ModelDifferenceObject);
        }

        protected override void OnActivated()
        {
            base.OnActivated();
            Frame.GetController<ListViewProcessCurrentObjectController>().CustomProcessSelectedItem += this.CustomProcessSelectedItem;
        }

        protected override void OnDeactivating()
        {
            Frame.GetController<ListViewProcessCurrentObjectController>().CustomProcessSelectedItem -= this.CustomProcessSelectedItem;

            base.OnDeactivating();
        }

        void CustomProcessSelectedItem(object sender, CustomProcessListViewSelectedItemEventArgs e)
        {
            this.Application.CreateCustomTemplate += this.CreateCustomTemplate;
        }

        void CreateCustomTemplate(object sender, CreateCustomTemplateEventArgs e)
        {
            this.Application.CreateCustomTemplate -= this.CreateCustomTemplate;
            var template = new ModelEditorDetailViewForm();
            ISupportClassicToRibbonTransform supportClassicToRibbonTransform = template as ISupportClassicToRibbonTransform;
            if (supportClassicToRibbonTransform != null && Application.Model != null && Application.Model.Options is IModelOptionsWin)
            {
                supportClassicToRibbonTransform.FormStyle = ((IModelOptionsWin)Application.Model.Options).FormStyle;
            }

            e.Template = template;
        }
    }
}
