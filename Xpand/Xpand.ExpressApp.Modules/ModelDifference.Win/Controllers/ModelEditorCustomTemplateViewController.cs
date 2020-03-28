using System;
using System.Reactive.Linq;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Templates;
using DevExpress.ExpressApp.Win;
using DevExpress.ExpressApp.Win.SystemModule;
using DevExpress.ExpressApp.Win.Templates.ActionContainers;
using DevExpress.XtraBars.Ribbon;
using Xpand.ExpressApp.ModelDifference.DataStore.BaseObjects;
using Xpand.ExpressApp.ModelDifference.Win.Templates;
using Xpand.Extensions.Reactive.Transform;
using Xpand.XAF.Modules.Reactive.Services;

namespace Xpand.ExpressApp.ModelDifference.Win.Controllers {
    public class ModelEditorCustomTemplateViewController : WindowController {
        protected override void OnFrameAssigned() {
            base.OnFrameAssigned();
            if (Frame.Context == TemplateContext.ApplicationWindow) {
                Application.WhenDetailViewCreated(typeof(ModelDifferenceObject))
                    .SelectMany(e => Application.WhenCreateCustomTemplate().FirstAsync()
                        .Do(_ => _.e.Template = GetModelEditorDetailViewForm(Application)))
                    .TakeUntil(Frame.WhenDisposingFrame())
                    .Subscribe();
            }
        }

        private static IFrameTemplate GetModelEditorDetailViewForm(XafApplication xafApplication){
            if (((WinApplication)xafApplication).UseOldTemplates || ((IModelOptionsWin)xafApplication.Model.Options).FormStyle != RibbonFormStyle.Ribbon) {
                var template = new ModelEditorDetailViewForm();
                var supportClassicToRibbonTransform = (ISupportClassicToRibbonTransform)template;
                if (xafApplication.Model?.Options is IModelOptionsWin optionsWin) {
                    supportClassicToRibbonTransform.FormStyle = optionsWin.FormStyle;
                }
                return template;
            }
            return new ModelEditorDetailRibbonFormV2();
        }
    }
}
