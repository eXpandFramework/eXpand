using System;
using System.Collections.Generic;
using System.ComponentModel;

using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Templates;
using DevExpress.XtraBars.Docking;
using DevExpress.ExpressApp.Utils;
using DevExpress.XtraBars;
using System.Windows.Forms;
using DevExpress.XtraTabbedMdi;
using DevExpress.ExpressApp.Win.Templates.ActionContainers;
using DevExpress.XtraBars.Ribbon;
using DevExpress.ExpressApp.Core;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Model.Core;
using DevExpress.ExpressApp.Win.SystemModule;
using DevExpress.ExpressApp.Win.Templates;

namespace FeatureCenter.Module.Win {
    public partial class MainForm : MainFormTemplateBase, IDockManagerHolder, ISupportMdiContainer, ISupportClassicToRibbonTransform, DevExpress.ExpressApp.Demos.IHintTemplate, IInfoPanelTemplate, ICaptionPanelHolder {
        public override void SetSettings(IModelTemplate modelTemplate) {
            base.SetSettings(modelTemplate);
            navigation.Model = TemplatesHelper.GetNavBarCustomizationNode();
            formStateModelSynchronizerComponent.Model = GetFormStateNode();
            modelSynchronizationManager.ModelSynchronizableComponents.Add(new NavigationModelSynchronizer(dockPanelNavigation, (IModelTemplateWin)modelTemplate));
        }
        protected virtual void InitializeImages() {
            barMdiChildrenListItem.Glyph = ImageLoader.Instance.GetImageInfo("Action_WindowList").Image;
            barMdiChildrenListItem.LargeGlyph = ImageLoader.Instance.GetLargeImageInfo("Action_WindowList").Image;
            barSubItemPanels.Glyph = ImageLoader.Instance.GetImageInfo("Action_Navigation").Image;
            barSubItemPanels.LargeGlyph = ImageLoader.Instance.GetLargeImageInfo("Action_Navigation").Image;
        }
        public MainForm() {
            InitializeComponent();
            InitializeImages();
            modelSynchronizationManager.ModelSynchronizableComponents.Add(navigation);

            viewSiteManager.ViewSiteControl = viewSitePanel;
            BarManager = mainBarManager;
            UpdateMdiModeDependentProperties();
        }
        public Bar ClassicStatusBar {
            get { return _statusBar; }
        }
        public DockPanel DockPanelNavigation {
            get { return dockPanelNavigation; }
        }
        public DockManager DockManager {
            get { return mainDockManager; }
        }
        protected override void UpdateMdiModeDependentProperties() {
            base.UpdateMdiModeDependentProperties();
            bool isMdi = UIMode == UIMode.Mdi;
            viewSiteControlPanel.Visible = !isMdi;
            barSubItemWindow.Visibility = isMdi ? BarItemVisibility.Always : BarItemVisibility.Never;
            barMdiChildrenListItem.Visibility = isMdi ? BarItemVisibility.Always : BarItemVisibility.Never;
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

        #region IInfoPanelTemplate Members
        DevExpress.XtraEditors.SplitContainerControl IInfoPanelTemplate.SplitContainer {
            get { return splitContainerControl; }
        }
        #endregion

        #region ICaptionPanelHolder Members

        public DevExpress.Utils.Frames.ApplicationCaption8_1 CaptionPanel {
            get { return captionPanel; }
        }

        #endregion
    }
    public interface IInfoPanelTemplate : IFrameTemplate {
        DevExpress.XtraEditors.SplitContainerControl SplitContainer { get; }
    }
    public interface ICaptionPanelHolder {
        DevExpress.Utils.Frames.ApplicationCaption8_1 CaptionPanel { get; }
    }
    [System.ComponentModel.DisplayName("FeatureCenter MainForm Template")]
    public class FeatureCenterMainFormTemplateLocalizer : FrameTemplateLocalizer<MainForm> { }
}
