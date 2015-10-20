using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Templates;
using DevExpress.ExpressApp.Win.Core;
using DevExpress.ExpressApp.Win.SystemModule;
using DevExpress.ExpressApp.Win.Templates;
using DevExpress.ExpressApp.Win.Templates.Ribbon;
using Fasterflect;

namespace Xpand.ExpressApp.ModelDifference.Win.Templates {
    public class ModelEditorDetailRibbonFormV2 :DetailRibbonFormV2, ISupportStoreSettings {
        void ISupportStoreSettings.SetSettings(IModelTemplate settings){
            this.SetSettings(() => BaseCall(settings));
        }

        private void BaseCall(IModelTemplate settings){
            IModelTemplateWin templateModel = (IModelTemplateWin)settings;
            TemplatesHelper templatesHelper = new TemplatesHelper(templateModel);
            var viewSiteManager = (ViewSiteManager) this.GetFieldValue("viewSiteManager");
            var formState = viewSiteManager.View != null ? templatesHelper.GetFormStateNode(viewSiteManager.View.Id) : templatesHelper.GetFormStateNode();
            var formStateModelSynchronizer= (FormStateModelSynchronizer) this.GetFieldValue("formStateModelSynchronizer");
            formStateModelSynchronizer.Model = formState;
            templatesHelper.SetRibbonSettings(Ribbon);
        }
    }
}
