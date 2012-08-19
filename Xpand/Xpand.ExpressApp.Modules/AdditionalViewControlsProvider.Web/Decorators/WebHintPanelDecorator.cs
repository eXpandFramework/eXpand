using System;
using System.Web.UI;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Editors;
using Xpand.ExpressApp.AdditionalViewControlsProvider.Editors;
using Xpand.ExpressApp.AdditionalViewControlsProvider.Logic;
using Xpand.ExpressApp.AdditionalViewControlsProvider.Web.Controls;

namespace Xpand.ExpressApp.AdditionalViewControlsProvider.Web.Decorators {

    [TypeDecorator(typeof(WarningPanel), typeof(WarningPanel), true, Position = Position.DetailViewItem)]
    [TypeDecorator(typeof(HintPanel), typeof(HintPanel), true)]
    public class WebHintPanelDecorator : AdditionalViewControlsProviderDecorator {
        HintPanelBase _hintPanelBase;

        private void hintPanel_Unload(object sender, EventArgs e) {
            _hintPanelBase = null;
            Dispose();
        }
        private void hintPanel_Disposed(object sender, EventArgs e) {
            _hintPanelBase = null;
            Dispose();
        }
        protected override void SetText(string text) {
            if (_hintPanelBase != null) {
                _hintPanelBase.Label.Text = text;
                var visible = !string.IsNullOrEmpty(_hintPanelBase.Label.Text);
                _hintPanelBase.Visible = visible;
                var supportLayoutManager = _hintPanelBase as ISupportLayoutManager;
                if (supportLayoutManager != null) {
                    ((Control)((ViewItem)(supportLayoutManager).LayoutItem).Control).Visible = visible;
                }
            }
        }
        public WebHintPanelDecorator(View view, HintPanelBase hintPanelBase, IAdditionalViewControlsRule controlsRule)
            : base(view, hintPanelBase, controlsRule) {
            _hintPanelBase = hintPanelBase;
            hintPanelBase.Unload += hintPanel_Unload;
            hintPanelBase.Disposed += hintPanel_Disposed;
            UpdateText();
        }
    }
}