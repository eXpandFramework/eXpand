using DevExpress.ExpressApp;
using DevExpress.ExpressApp.SystemModule;
using DevExpress.ExpressApp.Templates;
using DevExpress.ExpressApp.Win;
using DevExpress.ExpressApp.Win.SystemModule;
using DevExpress.ExpressApp.Win.Templates.ActionContainers;
using DevExpress.XtraBars.Ribbon;
using Xpand.ExpressApp.ModelDifference.DataStore.BaseObjects;
using Xpand.ExpressApp.ModelDifference.Win.Templates;

namespace Xpand.ExpressApp.ModelDifference.Win.Controllers {
    public class ModelEditorCustomTemplateViewController : ViewController<ListView> {
        public ModelEditorCustomTemplateViewController() {
            TargetObjectType = typeof(ModelDifferenceObject);
        }

        protected override void OnActivated() {
            base.OnActivated();
            Frame.GetController<ListViewProcessCurrentObjectController>().CustomProcessSelectedItem += CustomProcessSelectedItem;
        }

        protected override void OnDeactivated() {
            Frame.GetController<ListViewProcessCurrentObjectController>().CustomProcessSelectedItem -= CustomProcessSelectedItem;

            base.OnDeactivated();
        }

        void CustomProcessSelectedItem(object sender, CustomProcessListViewSelectedItemEventArgs e) {
            Application.CreateCustomTemplate += CreateCustomTemplate;
        }

        void CreateCustomTemplate(object sender, CreateCustomTemplateEventArgs e) {
            var xafApplication = (XafApplication)sender;
            xafApplication.CreateCustomTemplate -= CreateCustomTemplate;
            var template = GetModelEditorDetailViewForm(xafApplication);
            e.Template = template;
        }

        private static IFrameTemplate GetModelEditorDetailViewForm(XafApplication xafApplication){
//            return new ModelEditorDetailViewForm();
            if (((WinApplication)xafApplication).UseOldTemplates || ((IModelOptionsWin)xafApplication.Model.Options).FormStyle != RibbonFormStyle.Ribbon) {
                var template = new ModelEditorDetailViewForm();
                var supportClassicToRibbonTransform = (ISupportClassicToRibbonTransform)template;
                if (xafApplication.Model != null && xafApplication.Model.Options is IModelOptionsWin) {
                    supportClassicToRibbonTransform.FormStyle = ((IModelOptionsWin)xafApplication.Model.Options).FormStyle;
                }
                return template;
            }
            return new ModelEditorDetailRibbonFormV2();
        }
    }
}
