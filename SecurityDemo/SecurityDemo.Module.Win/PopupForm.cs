using System;
using System.Collections.Generic;
using System.Drawing;
using DevExpress.ExpressApp.Templates;
using DevExpress.ExpressApp.Win.Templates;
using DevExpress.ExpressApp.Win.Templates.ActionContainers;
using DevExpress.ExpressApp.Win.Utils;  

namespace FeatureCenter.Module.Win {
    public partial class PopupForm : PopupFormBase, DevExpress.ExpressApp.Demos.IHintTemplate {
        protected override void OnHandleCreated(EventArgs e) {
            base.OnHandleCreated(e);
            if(AutoShrink) {
                UpdateSize();
            }
        }
        protected override void OnVisibleChanged(EventArgs e) {
            base.OnVisibleChanged(e);
            if(Visible && AutoShrink) {
                UpdateSize();
            }
        }
        public PopupForm() {
            InitializeComponent();
            viewSitePanel.BringToFront();
            NativeMethods.SetExecutingApplicationIcon(this);
            AutoShrink = true;
            ShowInTaskbar = true;
            KeyPreview = true;
            IsSizeable = true;
            UpdateSize();
        }
        protected override Size OnCustomizeMinimumSize(Size calculatedMinumumSize) {
            Size result = new Size(calculatedMinumumSize.Width, calculatedMinumumSize.Height + hintPanel.Height + 5);
            return base.OnCustomizeMinimumSize(result);
        }
        private void hintPanel_SizeChanged(object sender, EventArgs e) {
            UpdateSize();
        }
        public override ICollection<IActionContainer> GetContainers() {
            return actionContainersManager1.GetContainers();
        }
        public override IActionContainer DefaultContainer {
            get { return actionContainersManager1.DefaultContainer; }
        }
        public ButtonsContainer ButtonsContainer {
            get { return buttonsContainer1; }
        }
        public override object ViewSiteControl {
            get { return viewSitePanel; }
        }
        public override DevExpress.XtraBars.BarManager BarManager {
            get { return xafBarManager1; }
        }
        #region IHintTemplate Members
        public string Hint {
            get {
                return hintPanel.Text;
            }
            set {
                hintPanel.Text = value;
                hintPanel.Visible = !string.IsNullOrEmpty(value);
            }
        }
        #endregion
        protected override DevExpress.ExpressApp.Win.Layout.XafLayoutControl BottomPanel {
            get { return bottomPanel; }
        }
        protected override ViewSiteManager ViewSiteManager {
            get { return viewSiteManager; }
        }
        protected override DevExpress.XtraEditors.PanelControl ViewSitePanel {
            get { return viewSitePanel; }
        }
        protected override DevExpress.ExpressApp.Win.Core.FormStateModelSynchronizer FormStateModelSynchronizer {
            get { return formStateModelSynchronizer; }
        }
    }
    [System.ComponentModel.DisplayName("FeatureCenter PopupForm Template")]
    public class FeatureCenterPopupFormTemplateLocalizer : FrameTemplateLocalizer<PopupForm> { }
}
