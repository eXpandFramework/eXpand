using System;
using System.Diagnostics;
using DevExpress.Utils.Frames;
using eXpand.ExpressApp.AdditionalViewControlsProvider.Win.Controls;
using View=DevExpress.ExpressApp.View;

namespace eXpand.ExpressApp.AdditionalViewControlsProvider.Win.Decorators
{
    public class WinHintPanelDecorator : AdditionalViewControlsProviderDecorator
    {
        private static int count;
        private readonly NotePanel8_1 hintPanel;

        public WinHintPanelDecorator()
        {
        }

        public WinHintPanelDecorator(View view, HintPanel hintPanel)
            : base(view,hintPanel)
        {
            this.hintPanel = hintPanel;
            hintPanel.Disposed += hintPanel_Disposed;
            UpdateText();
            count++;
        }


        private void hintPanel_Disposed(object sender, EventArgs e)
        {
            Trace.WriteLine("WinHintManager reference counter:" + (--count));
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