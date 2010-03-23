using System;
using DevExpress.ExpressApp;
using DevExpress.Utils.Frames;
using eXpand.ExpressApp.AdditionalViewControlsProvider.Logic;
using eXpand.ExpressApp.AdditionalViewControlsProvider.Win.Controls;

namespace eXpand.ExpressApp.AdditionalViewControlsProvider.Win.Decorators
{
    public class WinHintPanelDecorator : AdditionalViewControlsProviderDecorator
    {
        private static int count;
        private readonly NotePanel8_1 hintPanel;

        public WinHintPanelDecorator()
        {
        }

        public WinHintPanelDecorator(View view, object hintPanel, IAdditionalViewControlsRule controlsRule): this(view, hintPanel as HintPanel, controlsRule) {
        }
        public WinHintPanelDecorator(View view, HintPanel hintPanel, IAdditionalViewControlsRule controlsRule)
            : base(view, hintPanel, controlsRule)
        {
            this.hintPanel = hintPanel;
            hintPanel.Disposed += hintPanel_Disposed;
            UpdateText();
            count++;
        }


        private void hintPanel_Disposed(object sender, EventArgs e)
        {
            Dispose();
        }

        protected override void SetText(string text)
        {
            if (hintPanel != null)
            {
                hintPanel.Text = text;
                hintPanel.Visible = !string.IsNullOrEmpty(hintPanel.Text);
            }
        }
    }
}