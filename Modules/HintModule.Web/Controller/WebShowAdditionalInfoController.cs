using System;
using System.Web.UI;
using DevExpress.ExpressApp.Templates;
using eXpand.ExpressApp.HintModule.Controllers;

namespace eXpand.ExpressApp.HintModule.Web.Controller
{
    public partial class WebShowAdditionalInfoController : AdditionalInfoController
    {
        private void Window_TemplateChanged(object sender, EventArgs e)
        {
            var template = Window.Template as ISupportViewControlAdding;
            if (template != null)
            {
                template.ViewControlAdding += template_ViewControlAdding;
                ((Control)template).Unload += WebShowAdditionalInfoController_Unload;
            }
        }
        private void WebShowAdditionalInfoController_Unload(object sender, EventArgs e)
        {
            ClearControls();
        }
        private void template_ViewControlAdding(object sender, EventArgs e)
        {
            AddInfoControls();
        }
        protected override void AddInfoControlOnTemplate(object viewSiteControl, object control)
        {
            ((Control)viewSiteControl).Controls.AddAt(0, (Control)control);
        }
        protected override void OnActivated()
        {
            base.OnActivated();
            Window.TemplateChanged += Window_TemplateChanged;
        }
        protected override void OnDeactivating()
        {
            Window.TemplateChanged -= Window_TemplateChanged;
            base.OnDeactivating();
        }
    }
}