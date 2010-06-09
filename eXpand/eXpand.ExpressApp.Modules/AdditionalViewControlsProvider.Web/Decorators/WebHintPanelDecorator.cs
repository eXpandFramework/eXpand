using System;
using DevExpress.ExpressApp;
using eXpand.ExpressApp.AdditionalViewControlsProvider.Logic;
using eXpand.ExpressApp.AdditionalViewControlsProvider.Web.Controls;

namespace eXpand.ExpressApp.AdditionalViewControlsProvider.Web.Decorators {
    public class WebHintPanelDecorator : AdditionalViewControlsProviderDecorator
    {
        HintPanel hintPanel;
        static int count;
        private void hintPanel_Unload(object sender, EventArgs e)
        {
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
        public WebHintPanelDecorator(View view, HintPanel hintPanel, IAdditionalViewControlsRule controlsRule)
            : base(view, hintPanel, controlsRule)
        {
            this.hintPanel = hintPanel;
            hintPanel.Unload += hintPanel_Unload;
            hintPanel.Disposed += hintPanel_Disposed;
            UpdateText();
            count++;
        }
    }
}