using System;
using System.Collections.Generic;
using System.Windows.Forms;
using DevExpress.ExpressApp.Templates;
using DevExpress.XtraBars.Docking;
using DevExpress.ExpressApp.Utils;
using DevExpress.XtraBars;
using DevExpress.ExpressApp.Win.Templates;
using DevExpress.XtraTabbedMdi;

namespace eXpand.ExpressApp.Win.Templates {
	public partial class MDIMainForm : XtraFormTemplateBase {
		private const string NavigationVisibilityAttributeName = "NavigationVisibility";
		private const string NavigationWidthAttributeName = "NavigationWidth";
		private const string NavigationDockAttributeName = "NavigationDock";
		private const string FrameTemplatesMainFormLocalizationPath = @"FrameTemplates\MainForm";
		#region IFrameTemplate Members
		public override IActionContainer DefaultContainer {
			get { return cView; }
		}
		#endregion
		private DockVisibility navigationPaneVisibility = DockVisibility.Visible;
		private void dockPanelNavigation_ClosingPanel(object sender, DevExpress.XtraBars.Docking.DockPanelCancelEventArgs e) {
			barCheckItemNavigationPaneVisibility.Down = false;
		}
		private void barCheckItemNavigationPaneVisibility_CheckedChanged(object sender, DevExpress.XtraBars.ItemClickEventArgs e) {
			if(barCheckItemNavigationPaneVisibility.Down) {
				dockPanelNavigation.Visibility = navigationPaneVisibility;
			}
			else {
				navigationPaneVisibility = dockPanelNavigation.Visibility;
				dockPanelNavigation.Visibility = DockVisibility.Hidden;
			}
		}
		private void xtraTabbedMdiManager1_PageAdded(object sender, DevExpress.XtraTabbedMdi.MdiTabPageEventArgs e) {
			DevExpress.ExpressApp.View view = ((XtraFormTemplateBase)e.Page.MdiChild).View;
			if(view != null) {
				e.Page.Image = ImageLoader.Instance.GetImageInfo(view.Info.GetAttributeValue(DevExpress.ExpressApp.NodeWrappers.VisualItemInfoNodeWrapper.ImageNameAttribute)).Image;
			}
			((MDIChildForm)e.Page.MdiChild).ResetSettings();
		}
		private void xtraTabbedMdiManager1_SelectedPageChanged(object sender, System.EventArgs e) {
			if(xtraTabbedMdiManager1.SelectedPage != null) {
				this.Text = xtraTabbedMdiManager1.SelectedPage.MdiChild.Text;
			}
			else {
				this.Text = this.TemplateInfo.GetRootNode().GetAttributeValue("Title");
			}
		}
		private void barButtonItemCloseAll_ItemClick(object sender, ItemClickEventArgs e) {
			foreach(Form child in this.MdiChildren) {
				child.Close();
				if(child.Visible) {
					break;
				}
			}
		}
		private void barManager_Merge(object sender, DevExpress.XtraBars.BarManagerMergeEventArgs e) {
			MergeChildBars(e.ChildManager);
		}

		private void barManager_UnMerge(object sender, DevExpress.XtraBars.BarManagerMergeEventArgs e) {
			UnMergeChildBars(e.ChildManager);
		}
		protected void MergeChildBars(BarManager childBarManager) {
			foreach(Bar childBar in childBarManager.Bars) {
				if(childBar == childBarManager.MainMenu || childBar == childBarManager.StatusBar) {
					continue;
				}
				childBar.Visible = false;
			}
		}
		protected void UnMergeChildBars(BarManager childBarManager) {
		}
		public override void ReloadSettings() {
			base.ReloadSettings();
			if(TemplateInfo != null) {
				dockPanelNavigation.Width = TemplateInfo.GetAttributeIntValue(NavigationWidthAttributeName, dockPanelNavigation.Width);
				if(!string.IsNullOrEmpty(TemplateInfo.GetAttributeValue(NavigationDockAttributeName))) {
					dockPanelNavigation.Dock = (DockingStyle)Enum.Parse(typeof(DockingStyle), TemplateInfo.GetAttributeValue(NavigationDockAttributeName));
				}
				if(!string.IsNullOrEmpty(TemplateInfo.GetAttributeValue(NavigationVisibilityAttributeName))) {
					navigationPaneVisibility = (DockVisibility)Enum.Parse(typeof(DockVisibility), TemplateInfo.GetAttributeValue(NavigationVisibilityAttributeName), true);
				}
				barCheckItemNavigationPaneVisibility.Down = (navigationPaneVisibility == DockVisibility.AutoHide || navigationPaneVisibility == DockVisibility.Visible);
			}
		}
		public override void SaveSettings() {
			base.SaveSettings();
			if(TemplateInfo != null) {
				TemplateInfo.SetAttribute(NavigationWidthAttributeName, dockPanelNavigation.Width);
				TemplateInfo.SetAttribute(NavigationDockAttributeName, dockPanelNavigation.Dock.ToString());
				TemplateInfo.SetAttribute(NavigationVisibilityAttributeName, dockPanelNavigation.Visibility.ToString());
			}
		}

	    public XtraTabbedMdiManager XtraTabbedMdiManager {
	        get { return xtraTabbedMdiManager1; }
	    }

	    public MDIMainForm() {
			InitializeComponent();

			MainMenuBar.Text = CaptionHelper.GetLocalizedText(FrameTemplatesMainFormLocalizationPath, "MainMenu", "MainMenu");
			barSubItemFile.Caption = CaptionHelper.GetLocalizedText(FrameTemplatesMainFormLocalizationPath, "FileSubMenu", "FileSubMenu");
			cObjectsCreation.Caption = CaptionHelper.GetLocalizedText(FrameTemplatesMainFormLocalizationPath, "ObjectsCreation", "ObjectsCreation");
			cFile.Caption = CaptionHelper.GetLocalizedText(FrameTemplatesMainFormLocalizationPath, "File", "File");
			cPrint.Caption = CaptionHelper.GetLocalizedText(FrameTemplatesMainFormLocalizationPath, "Print", "Print");
			cExport.Caption = CaptionHelper.GetLocalizedText(FrameTemplatesMainFormLocalizationPath, "Export", "Export");
			cExit.Caption = CaptionHelper.GetLocalizedText(FrameTemplatesMainFormLocalizationPath, "Exit", "Exit");
			barSubItemEdit.Caption = CaptionHelper.GetLocalizedText(FrameTemplatesMainFormLocalizationPath, "EditSubMenu","EditSubMenu");
			cRecordEdit.Caption = CaptionHelper.GetLocalizedText(FrameTemplatesMainFormLocalizationPath, "RecordEdit", "RecordEdit");
			barSubItemView.Caption = CaptionHelper.GetLocalizedText(FrameTemplatesMainFormLocalizationPath, "ViewSubMenu", "ViewSubMenu");
			barCheckItemNavigationPaneVisibility.Caption = CaptionHelper.GetLocalizedText(FrameTemplatesMainFormLocalizationPath, "NavigationBar", "NavigationBar");
			cRecordsNavigation.Caption = CaptionHelper.GetLocalizedText(FrameTemplatesMainFormLocalizationPath, "RecordsNavigation", "RecordsNavigation");
			cView.Caption = CaptionHelper.GetLocalizedText(FrameTemplatesMainFormLocalizationPath, "View", "View");
			barSubItemTools.Caption = CaptionHelper.GetLocalizedText(FrameTemplatesMainFormLocalizationPath, "ToolsSubMenu", "ToolsSubMenu");
			cTools.Caption = CaptionHelper.GetLocalizedText(FrameTemplatesMainFormLocalizationPath, "Tools", "Tools");
			barSubItemWindow.Caption = CaptionHelper.GetLocalizedText(FrameTemplatesMainFormLocalizationPath, "Window", "Window");
			barButtonItemCloseAll.Caption = CaptionHelper.GetLocalizedText(FrameTemplatesMainFormLocalizationPath, "CloseAllWindows", "Close &All Windows");
			barMdiChildrenListItem.Caption = CaptionHelper.GetLocalizedText(FrameTemplatesMainFormLocalizationPath, "Window List", "Window List");
			cDiagnostic.Caption = CaptionHelper.GetLocalizedText(FrameTemplatesMainFormLocalizationPath, "Diagnostic", "Diagnostic");
			cOptions.Caption = CaptionHelper.GetLocalizedText(FrameTemplatesMainFormLocalizationPath, "Options", "Options");
			barSubItemHelp.Caption = CaptionHelper.GetLocalizedText(FrameTemplatesMainFormLocalizationPath, "HelpSubMenu", "HelpSubMenu");
			cAbout.Caption = CaptionHelper.GetLocalizedText(FrameTemplatesMainFormLocalizationPath, "About", "About");
			StandardToolBar.Text = CaptionHelper.GetLocalizedText(FrameTemplatesMainFormLocalizationPath, "MainToolbar", "MainToolbar");
			cFiltersSearch.Caption = CaptionHelper.GetLocalizedText(FrameTemplatesMainFormLocalizationPath, "Search", "Search");
			cFilters.Caption = CaptionHelper.GetLocalizedText(FrameTemplatesMainFormLocalizationPath, "Filters", "Filters");
			StatusBar.Text = CaptionHelper.GetLocalizedText(FrameTemplatesMainFormLocalizationPath, "StatusBar", "StatusBar");

			//this.templateInfo = templateInfo;
			List<IActionContainer> containers = new List<IActionContainer>();
			containers.AddRange(new IActionContainer[] {
				cAbout, cTools, cFile, cObjectsCreation, cPrint, cExport, cExit, cRecordEdit, 
				cRecordsNavigation, cFiltersSearch, cFilters,
				cView, cOptions, navigation, cDiagnostic});
			Initialize(barManager, containers,
				new IActionContainer[] { cObjectsCreation, cRecordEdit, cView, cPrint, cExport },
				null, navigation);
		}
	}
}
