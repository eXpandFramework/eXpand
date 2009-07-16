using System;
using System.Diagnostics;
using System.Windows.Forms;
using DevExpress.Utils.Frames;
using View=DevExpress.ExpressApp.View;

namespace eXpand.ExpressApp.HintModule.Win
{
    public class WinHintDecorator : HintDecorator
    {
        private static int count;
        private readonly NotePanel8_1 hintPanel;

        public WinHintDecorator(View view, NotePanel8_1 hintPanel)
            : base(view)
        {
            this.hintPanel = hintPanel;
            hintPanel.Disposed += hintPanel_Disposed;
            UpdateText();
            count++;
        }

        public Control Control
        {
            get { return hintPanel; }
        }

        private void hintPanel_Disposed(object sender, EventArgs e)
        {
            Trace.WriteLine("WebHintManager reference counter:" + (--count));
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