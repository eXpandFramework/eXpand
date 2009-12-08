using System;
using System.Web.UI;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Templates;
using eXpand.ExpressApp.AdditionalViewControlsProvider.Controllers;
using eXpand.ExpressApp.Web.Core;

namespace eXpand.ExpressApp.AdditionalViewControlsProvider.Web.Controller
{
    public partial class WebShowAdditionalViewControlsController : AdditionalViewControlsProviderController
    {
        private void Window_TemplateChanged(object sender, EventArgs e)
        {
            
            var template = Window.Template as ISupportViewControlAdding;
            if (template != null)
            {
                template.ViewControlAdding += template_ViewControlAdding;
                ((Control)template).Unload += WebShowAdditionalInfoController_Unload;
            }
            else if (Window.Context == TemplateContext.PopupWindow && Window.Template != null)
                Window.View.ControlsCreated +=
                    (o, args) => AddControls((Window.View.Control));
        }

        public override void Unregister(IAdditionalViewControlsProvider controlProvider){
            base.Unregister(controlProvider);
        }

        public override void Register(IAdditionalViewControlsProvider controlProvider, View view, Frame frame){
            if (view is DetailView || !((ListView)view).IsNested(frame))
                base.Register(controlProvider, view, frame);
        }

        private void WebShowAdditionalInfoController_Unload(object sender, EventArgs e)
        {
            ClearControls();
        }
        private void template_ViewControlAdding(object sender, EventArgs e)
        {
            AddControls();
        }
        protected override void AddControlOnTemplate(object viewSiteControl, object control,AdditionalViewControlsProviderPosition position)
        {
            ControlCollection collection = ((Control)viewSiteControl).Controls;
            if (position == AdditionalViewControlsProviderPosition.Top)
                collection.AddAt(0, (Control) control);
            else
                collection.Add((Control) control);
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