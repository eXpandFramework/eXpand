using System;
using System.Web.UI;
using DevExpress.ExpressApp;

namespace eXpand.ExpressApp.HintModule.Web
{
    public class WebHintDecorator : HintDecorator
    {
        HintPanel hintPanel;
        static int count;
        private void hintPanel_Unload(object sender, EventArgs e)
        {
            System.Diagnostics.Trace.WriteLine("WebHintManager reference counter:" + (--count));
            hintPanel = null;
            Dispose();
        }
        private void hintPanel_Disposed(object sender, EventArgs e)
        {
            hintPanel = null;
            Dispose();
        }
        protected override void SetText(string text)
        {
            if (hintPanel != null)
            {
                hintPanel.Label.Text = text;
                hintPanel.Visible = !string.IsNullOrEmpty(hintPanel.Label.Text);
            }
        }
        public WebHintDecorator(View view, HintPanel hintPanel)
            : base(view)
        {
            this.hintPanel = hintPanel;
            hintPanel.Unload += hintPanel_Unload;
            hintPanel.Disposed += hintPanel_Disposed;
            UpdateText();
            count++;
        }
        public Control Control
        {
            get { return hintPanel; }
        }
    }
}
