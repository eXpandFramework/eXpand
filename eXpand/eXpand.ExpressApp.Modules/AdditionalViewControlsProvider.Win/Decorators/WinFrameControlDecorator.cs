using System;
using DevExpress.ExpressApp;
using DevExpress.Utils.Frames;
using eXpand.ExpressApp.AdditionalViewControlsProvider.Logic;
using eXpand.ExpressApp.AdditionalViewControlsProvider.Win.Controls;

namespace eXpand.ExpressApp.AdditionalViewControlsProvider.Win.Decorators
{
    [TypeDecorator(typeof(FrameControl), typeof(HintPanel),true)]
    public class WinFrameControlDecorator : AdditionalViewControlsProviderDecorator
    {
        private static int count;
        private readonly FrameControl _frameControl;

        public WinFrameControlDecorator()
        {
        }

        public WinFrameControlDecorator(View view, object hintPanel, IAdditionalViewControlsRule controlsRule): this(view, hintPanel as FrameControl, controlsRule) {
        }
        public WinFrameControlDecorator(View view, FrameControl frameControl, IAdditionalViewControlsRule controlsRule)
            : base(view, frameControl, controlsRule)
        {
            _frameControl = frameControl;
            frameControl.Disposed += hintPanel_Disposed;
            UpdateText();
            count++;
        }


        private void hintPanel_Disposed(object sender, EventArgs e)
        {
            Dispose();
        }

        protected override void SetText(string text)
        {
            if (_frameControl != null)
            {
                _frameControl.Text = text;
                _frameControl.Visible = !string.IsNullOrEmpty(_frameControl.Text);
            }
        }
    }
}