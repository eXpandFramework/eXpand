using System;
using System.Linq;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Win.Controls;
using DevExpress.ExpressApp.Win.Templates;
using DevExpress.XtraBars;
using Xpand.ExpressApp.ModelDifference.DataStore.BaseObjects;
using Xpand.ExpressApp.ModelDifference.Win.PropertyEditors;

namespace Xpand.ExpressApp.ModelDifference.Win.Controllers {
    public class ModelEditorTemplateViewController : ViewController<DetailView> {
        public ModelEditorTemplateViewController() {
            TargetObjectType = typeof(ModelDifferenceObject);
        }

        protected override void OnViewControlsCreated() {
            base.OnViewControlsCreated();
            var template = Frame.Template as XtraFormTemplateBase;
            if (template != null) {
                SetTemplate();
            }
        }

        private void SetTemplate() {
            var modelEditorPropertyEditors = View.GetItems<ModelEditorPropertyEditor>();
            if (modelEditorPropertyEditors.Count > 0) {
                var modelEditorViewController = modelEditorPropertyEditors[0].ModelEditorViewModelEditorViewController;
                var caption = Guid.NewGuid().ToString();
                modelEditorViewController.SaveAction.Caption = caption;
                modelEditorViewController.SetTemplate(Frame.Template);
                var barManagerHolder = ((IBarManagerHolder)Frame.Template);
                barManagerHolder.BarManager.Items.OfType<BarButtonItem>().Single(item => item.Caption.IndexOf(caption, StringComparison.Ordinal) > -1).Visibility = BarItemVisibility.Never;
            }
        }
    }
}
