using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using DevExpress.ExpressApp.Win;
using DevExpress.ExpressApp.Win.Templates;
using eXpand.ExpressApp.SystemModule;

namespace eXpand.ExpressApp.Win.SystemModule
{
    public partial class WinShowAdditionalInfoController : AdditionalInfoController
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
            if (templateForm != null)
            {
                templateForm.UpdateSize();
            }
        }

        private void Window_TemplateViewChanged(object sender, EventArgs e)
        {
            AddInfoControls();
        }

        protected override void AddInfoControlOnTemplate(object viewSiteControl, object control)
        {
            ((Control) viewSiteControl).Controls.Add((Control) control);
            infoControls.Add(control);
            ((Control) control).SizeChanged += control_SizeChanged;
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