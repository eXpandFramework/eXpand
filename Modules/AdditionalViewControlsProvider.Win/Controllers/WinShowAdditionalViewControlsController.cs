using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Win;
using DevExpress.ExpressApp.Win.Templates;
using eXpand.ExpressApp.AdditionalViewControlsProvider.Controllers;
using ListView=System.Windows.Forms.ListView;
using View=DevExpress.ExpressApp.View;
using eXpand.ExpressApp.Win.Core;

namespace eXpand.ExpressApp.AdditionalViewControlsProvider.Win.Controllers
{
    public partial class WinShowAdditionalViewControlsController : AdditionalViewControlsProviderController
    {
        private readonly List<object> infoControls = new List<object>();

        private void SubscribeToCustomizeClientSize()
        {
            var templateForm = Window.Template as PopupForm;
            if (templateForm != null)
            {
                templateForm.CustomizeClientSize += templateForm_CustomizeClientSize;
            }
        }

        public override void Register(IAdditionalViewControlsProvider controlProvider, View view,Frame frame){
            if (view is DetailView || !((DevExpress.ExpressApp.ListView)view).IsNested(frame))
                base.Register(controlProvider, view,frame);
        }

        private void templateForm_CustomizeClientSize(object sender, CustomSizeEventArgs e)
        {
            int newHeight = e.CustomSize.Height;
            foreach (Control control in infoControls)
            {
                newHeight += control.Size.Height;
            }
            e.CustomSize = new Size(e.CustomSize.Width, newHeight);
            e.Handled = true;
        }

        private void Window_TemplateChanged(object sender, EventArgs e)
        {
            SubscribeToCustomizeClientSize();
        }

        private void control_SizeChanged(object sender, EventArgs e)
        {
            var templateForm = Frame.Template as PopupForm;
            if (templateForm != null){
                templateForm.UpdateSize();
            }
        }

        private void Window_TemplateViewChanged(object sender, EventArgs e)
        {
            AddControls();
        }

        protected override void AddControlOnTemplate(object viewSiteControl, object control, AdditionalViewControlsProviderPosition position)
        {
            var value = (Control) control;
            value.Dock = position==AdditionalViewControlsProviderPosition.Bottom ? DockStyle.Bottom : DockStyle.Top;
            Control.ControlCollection collection = ((Control) viewSiteControl).Controls;
            collection.Add(value);
            
            infoControls.Add(control);
            value.SizeChanged += control_SizeChanged;
        }

        protected override void OnActivated()
        {
            base.OnActivated();
            Window.TemplateChanged += Window_TemplateChanged;
            Window.TemplateViewChanged += Window_TemplateViewChanged;
            //SubscribeToCustomizeMinimunSize();
        }

        protected override void OnDeactivating()
        {
            Window.TemplateChanged -= Window_TemplateChanged;
            Window.TemplateViewChanged -= Window_TemplateViewChanged;
            foreach (Control control in infoControls)
            {
                control.SizeChanged -= control_SizeChanged;
            }
            infoControls.Clear();
            base.OnDeactivating();
        }
    }
}