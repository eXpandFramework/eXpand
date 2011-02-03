using System;
using DevExpress.ExpressApp;
using DevExpress.Utils.Frames;
using DevExpress.XtraLayout;
using DevExpress.XtraLayout.Utils;
using Xpand.ExpressApp.AdditionalViewControlsProvider.Editors;
using Xpand.ExpressApp.AdditionalViewControlsProvider.Logic;
using Xpand.ExpressApp.AdditionalViewControlsProvider.Win.Controls;

namespace Xpand.ExpressApp.AdditionalViewControlsProvider.Win.Decorators {
    [TypeDecorator(typeof(FrameControl), typeof(WarningPanel), true, Position = Position.DetailViewItem)]
    [TypeDecorator(typeof(FrameControl), typeof(HintPanel), true)]
    public class WinFrameControlDecorator : AdditionalViewControlsProviderDecorator {

        private readonly FrameControl _frameControl;

        public WinFrameControlDecorator() {
        }

        public WinFrameControlDecorator(View view, FrameControl frameControl, IAdditionalViewControlsRule controlsRule)
            : base(view, frameControl, controlsRule) {
            _frameControl = frameControl;
            frameControl.Disposed += hintPanel_Disposed;
            UpdateText();
        }


        private void hintPanel_Disposed(object sender, EventArgs e) {
            Dispose();
        }

        protected override void SetText(string text) {
            if (_frameControl != null) {
                _frameControl.Text = text;
                bool visible = !string.IsNullOrEmpty(_frameControl.Text);
                _frameControl.Visible = visible;
                if (_frameControl is ISupportLayoutManager) {
                    ((BaseLayoutItem)((ISupportLayoutManager)_frameControl).LayoutItem).Visibility = visible ? LayoutVisibility.Always : LayoutVisibility.Never;
                }
            }
        }
    }
}