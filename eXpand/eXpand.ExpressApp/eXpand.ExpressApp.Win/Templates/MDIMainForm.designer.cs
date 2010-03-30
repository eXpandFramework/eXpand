using DevExpress.XtraBars;
using DevExpress.ExpressApp.Utils;
namespace MDIDemo.Win {
	partial class MDIMainForm {		
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing) {
			if(disposing && (components != null)) {
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		#region Windows Form Designer generated code

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent() {
			this.components = new System.ComponentModel.Container();
			this.barManager = new DevExpress.XtraBars.BarManager(this.components);
			this.barManager.AllowMergeInvisibleLinks = true;
			this.MainMenuBar = new DevExpress.XtraBars.Bar();
			this.barSubItemFile = new DevExpress.ExpressApp.Win.Templates.MainMenuItem();
			this.cObjectsCreation = new DevExpress.ExpressApp.Win.Templates.ActionContainers.ActionContainerBarItem();
			this.cFile = new DevExpress.ExpressApp.Win.Templates.ActionContainers.ActionContainerBarItem();
			this.cPrint = new DevExpress.ExpressApp.Win.Templates.ActionContainers.ActionContainerBarItem();
			this.cExport = new DevExpress.ExpressApp.Win.Templates.ActionContainers.ActionContainerBarItem();
			this.cExit = new DevExpress.ExpressApp.Win.Templates.ActionContainers.ActionContainerBarItem();
			this.barSubItemEdit = new DevExpress.ExpressApp.Win.Templates.MainMenuItem();
			this.cRecordEdit = new DevExpress.ExpressApp.Win.Templates.ActionContainers.ActionContainerBarItem();
			this.barSubItemView = new DevExpress.ExpressApp.Win.Templates.MainMenuItem();
			this.barCheckItemNavigationPaneVisibility = new DevExpress.XtraBars.BarCheckItem();
			this.cRecordsNavigation = new DevExpress.ExpressApp.Win.Templates.ActionContainers.ActionContainerBarItem();
			this.cView = new DevExpress.ExpressApp.Win.Templates.ActionContainers.ActionContainerBarItem();
			this.barSubItemTools = new DevExpress.ExpressApp.Win.Templates.MainMenuItem();
			this.cTools = new DevExpress.ExpressApp.Win.Templates.ActionContainers.ActionContainerMenuBarItem();
			this.cDiagnostic = new DevExpress.ExpressApp.Win.Templates.ActionContainers.ActionContainerMenuBarItem();
			this.cOptions = new DevExpress.ExpressApp.Win.Templates.ActionContainers.ActionContainerMenuBarItem();
			this.barSubItemWindow = new DevExpress.ExpressApp.Win.Templates.MainMenuItem();
			this.barButtonItemCloseAll = new DevExpress.XtraBars.BarButtonItem();
			this.barMdiChildrenListItem = new DevExpress.XtraBars.BarMdiChildrenListItem();
			this.barSubItemHelp = new DevExpress.ExpressApp.Win.Templates.MainMenuItem();
			this.cAbout = new DevExpress.ExpressApp.Win.Templates.ActionContainers.ActionContainerMenuBarItem();
			this.StandardToolBar = new DevExpress.XtraBars.Bar();
			this.cFiltersSearch = new DevExpress.ExpressApp.Win.Templates.ActionContainers.ActionContainerBarItem();
			this.cFilters = new DevExpress.ExpressApp.Win.Templates.ActionContainers.ActionContainerBarItem();
			this.StatusBar = new DevExpress.XtraBars.Bar();
			this.barAndDockingController1 = new DevExpress.XtraBars.BarAndDockingController(this.components);
			this.barDockControlTop = new DevExpress.XtraBars.BarDockControl();
			this.barDockControlBottom = new DevExpress.XtraBars.BarDockControl();
			this.barDockControlLeft = new DevExpress.XtraBars.BarDockControl();
			this.barDockControlRight = new DevExpress.XtraBars.BarDockControl();
			this.dockManager1 = new DevExpress.XtraBars.Docking.DockManager();
			this.dockPanelNavigation = new DevExpress.XtraBars.Docking.DockPanel();
			this.dockPanelNavigation_Container = new DevExpress.XtraBars.Docking.ControlContainer();
			this.navigation = new DevExpress.ExpressApp.Win.Templates.ActionContainers.NavigationActionContainer();
			this.xtraTabbedMdiManager1 = new DevExpress.XtraTabbedMdi.XtraTabbedMdiManager(this.components);
			((System.ComponentModel.ISupportInitialize)(this.barManager)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.barAndDockingController1)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.dockManager1)).BeginInit();
			this.dockPanelNavigation.SuspendLayout();
			this.dockPanelNavigation_Container.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.xtraTabbedMdiManager1)).BeginInit();
			this.SuspendLayout();
			// 
			// barManager
			// 
			this.barManager.Bars.AddRange(new DevExpress.XtraBars.Bar[] {
            this.MainMenuBar,
            this.StandardToolBar,
            this.StatusBar});
			this.barManager.Controller = this.barAndDockingController1;
			this.barManager.DockControls.Add(this.barDockControlTop);
			this.barManager.DockControls.Add(this.barDockControlBottom);
			this.barManager.DockControls.Add(this.barDockControlLeft);
			this.barManager.DockControls.Add(this.barDockControlRight);
			this.barManager.DockManager = this.dockManager1;
			this.barManager.Form = this;
			this.barManager.Items.AddRange(new DevExpress.XtraBars.BarItem[] {
            this.barSubItemFile,
            this.barSubItemEdit,
            this.barSubItemView,
            this.barSubItemTools,
            this.barSubItemHelp,
            this.cFile,
            this.cObjectsCreation,
            this.cPrint,
            this.cExport,
            this.cExit,
            this.cRecordEdit,
            this.cRecordsNavigation,
            this.cFiltersSearch,
            this.cFilters,
            this.cView,
            this.cTools,
            this.cDiagnostic,
            this.cOptions,
            this.cAbout,
            this.barSubItemWindow,
            this.barMdiChildrenListItem,
            this.barButtonItemCloseAll,
            this.barCheckItemNavigationPaneVisibility});
			this.barManager.MainMenu = this.MainMenuBar;
			this.barManager.MaxItemId = 34;
			this.barManager.StatusBar = this.StatusBar;
			this.barManager.Merge += new DevExpress.XtraBars.BarManagerMergeEventHandler(this.barManager_Merge);
			this.barManager.UnMerge += new DevExpress.XtraBars.BarManagerMergeEventHandler(this.barManager_UnMerge);
			// 
			// MainMenuBar
			// 
			this.MainMenuBar.BarName = "Main Menu";
			this.MainMenuBar.DockCol = 0;
			this.MainMenuBar.DockRow = 0;
			this.MainMenuBar.DockStyle = DevExpress.XtraBars.BarDockStyle.Top;
			this.MainMenuBar.LinksPersistInfo.AddRange(new DevExpress.XtraBars.LinkPersistInfo[] {
            new DevExpress.XtraBars.LinkPersistInfo(this.barSubItemFile),
            new DevExpress.XtraBars.LinkPersistInfo(this.barSubItemEdit),
            new DevExpress.XtraBars.LinkPersistInfo(this.barSubItemView),
            new DevExpress.XtraBars.LinkPersistInfo(this.barSubItemTools),
            new DevExpress.XtraBars.LinkPersistInfo(this.barSubItemWindow),
            new DevExpress.XtraBars.LinkPersistInfo(this.barSubItemHelp)});
			this.MainMenuBar.OptionsBar.MultiLine = true;
			this.MainMenuBar.OptionsBar.UseWholeRow = true;
			this.MainMenuBar.Text = "Main Menu";
			// 
			// barSubItemFile
			// 
			this.barSubItemFile.Caption = "File";
			this.barSubItemFile.Id = 0;
			this.barSubItemFile.LinksPersistInfo.AddRange(new DevExpress.XtraBars.LinkPersistInfo[] {
            new DevExpress.XtraBars.LinkPersistInfo(this.cObjectsCreation, true),
            new DevExpress.XtraBars.LinkPersistInfo(this.cFile, true),
            new DevExpress.XtraBars.LinkPersistInfo(this.cPrint, true),
            new DevExpress.XtraBars.LinkPersistInfo(this.cExport, true),
            new DevExpress.XtraBars.LinkPersistInfo(this.cExit, true)});
			this.barSubItemFile.MergeType = DevExpress.XtraBars.BarMenuMerge.MergeItems;
			this.barSubItemFile.Name = "barSubItemFile";
			// 
			// cObjectsCreation
			//			
			this.cObjectsCreation.Caption = "Objects creation";
			this.cObjectsCreation.ContainerId = "ObjectsCreation";
			this.cObjectsCreation.Id = 18;
			this.cObjectsCreation.MergeType = DevExpress.XtraBars.BarMenuMerge.MergeItems;
			this.cObjectsCreation.Name = "cObjectsCreation";
			// 
			// cFile
			// 
			this.cFile.Caption = "File";
			this.cFile.ContainerId = "File";
			this.cFile.Id = 5;
			this.cFile.MergeOrder = 2;
			this.cFile.MergeType = DevExpress.XtraBars.BarMenuMerge.MergeItems;
			this.cFile.Name = "cFile";
			// 
			// cPrint
			// 
			this.cPrint.Caption = "Print";
			this.cPrint.ContainerId = "Print";
			this.cPrint.Id = 6;
			this.cPrint.MergeType = DevExpress.XtraBars.BarMenuMerge.MergeItems;
			this.cPrint.Name = "cPrint";
			// 
			// cExport
			// 
			this.cExport.Caption = "Export";
			this.cExport.ContainerId = "Export";
			this.cExport.Id = 7;
			this.cExport.MergeType = DevExpress.XtraBars.BarMenuMerge.MergeItems;
			this.cExport.Name = "cExport";
			// 
			// cExit
			// 
			this.cExit.Caption = "Exit";
			this.cExit.ContainerId = "Exit";
			this.cExit.Id = 8;
			this.cExit.Name = "cExit";
			// 
			// barSubItemEdit
			// 
			this.barSubItemEdit.Caption = "Edit";
			this.barSubItemEdit.Id = 1;
			this.barSubItemEdit.LinksPersistInfo.AddRange(new DevExpress.XtraBars.LinkPersistInfo[] {
            new DevExpress.XtraBars.LinkPersistInfo(this.cRecordEdit, true),
            new DevExpress.XtraBars.LinkPersistInfo(this.cFiltersSearch, true),
			new DevExpress.XtraBars.LinkPersistInfo(this.cFilters, true)});
			this.barSubItemEdit.MergeType = DevExpress.XtraBars.BarMenuMerge.MergeItems;
			this.barSubItemEdit.Name = "barSubItemEdit";
			// 
			// cRecordEdit
			// 
			this.cRecordEdit.Caption = "Record Edit";
			this.cRecordEdit.ContainerId = "RecordEdit";
			this.cRecordEdit.Id = 9;
			this.cRecordEdit.MergeType = DevExpress.XtraBars.BarMenuMerge.MergeItems;
			this.cRecordEdit.Name = "cRecordEdit";
			// 
			// barSubItemView
			// 
			this.barSubItemView.Caption = "View";
			this.barSubItemView.Id = 2;
			this.barSubItemView.LinksPersistInfo.AddRange(new DevExpress.XtraBars.LinkPersistInfo[] {
            new DevExpress.XtraBars.LinkPersistInfo(this.barCheckItemNavigationPaneVisibility, true),
            new DevExpress.XtraBars.LinkPersistInfo(this.cRecordsNavigation, true),
            new DevExpress.XtraBars.LinkPersistInfo(this.cView, true)});
			this.barSubItemView.MergeType = DevExpress.XtraBars.BarMenuMerge.MergeItems;
			this.barSubItemView.Name = "barSubItemView";
			// 
			// barCheckItemNavigationPaneVisibility
			// 
			this.barCheckItemNavigationPaneVisibility.Caption = "Navigation Bar";
			this.barCheckItemNavigationPaneVisibility.Id = 33;
			this.barCheckItemNavigationPaneVisibility.Name = "barCheckItemNavigationPaneVisibility";
			this.barCheckItemNavigationPaneVisibility.CheckedChanged += new DevExpress.XtraBars.ItemClickEventHandler(this.barCheckItemNavigationPaneVisibility_CheckedChanged);
			// 
			// cRecordsNavigation
			// 
			this.cRecordsNavigation.Caption = "Records Navigation";
			this.cRecordsNavigation.ContainerId = "RecordsNavigation";
			this.cRecordsNavigation.Id = 10;
			this.cRecordsNavigation.MergeType = DevExpress.XtraBars.BarMenuMerge.MergeItems;
			this.cRecordsNavigation.Name = "cRecordsNavigation";
			// 
			// cView
			// 
			this.cView.Caption = "View";
			this.cView.ContainerId = "View";
			this.cView.Id = 12;
			this.cView.MergeType = DevExpress.XtraBars.BarMenuMerge.MergeItems;
			this.cView.Name = "cView";
			// 
			// barSubItemTools
			// 
			this.barSubItemTools.Caption = "Tools";
			this.barSubItemTools.Id = 3;
			this.barSubItemTools.LinksPersistInfo.AddRange(new DevExpress.XtraBars.LinkPersistInfo[] {
            new DevExpress.XtraBars.LinkPersistInfo(this.cTools, true),
            new DevExpress.XtraBars.LinkPersistInfo(this.cDiagnostic, true),
            new DevExpress.XtraBars.LinkPersistInfo(this.cOptions, true)});
			this.barSubItemTools.MergeType = DevExpress.XtraBars.BarMenuMerge.MergeItems;
			this.barSubItemTools.Name = "barSubItemTools";
			// 
			// cTools
			// 
			this.cTools.Caption = "Tools";
			this.cTools.ContainerId = "Tools";
			this.cTools.Id = 13;
			this.cTools.MergeType = DevExpress.XtraBars.BarMenuMerge.MergeItems;
			this.cTools.Name = "cTools";
			// 
			// cDiagnostic
			// 
			this.cDiagnostic.Caption = "Diagnostic";
			this.cDiagnostic.ContainerId = "Diagnostic";
			this.cDiagnostic.Id = 16;
			this.cDiagnostic.MergeType = DevExpress.XtraBars.BarMenuMerge.MergeItems;
			this.cDiagnostic.Name = "cDiagnostic";
			// 
			// cOptions
			// 
			this.cOptions.Caption = "Options";
			this.cOptions.ContainerId = "Options";
			this.cOptions.Id = 14;
			this.cOptions.MergeType = DevExpress.XtraBars.BarMenuMerge.MergeItems;
			this.cOptions.Name = "cOptions";
			// 
			// barSubItemWindow
			// 
			this.barSubItemWindow.Caption = "Window";
			this.barSubItemWindow.Id = 32;
			this.barSubItemWindow.LinksPersistInfo.AddRange(new DevExpress.XtraBars.LinkPersistInfo[] {
            new DevExpress.XtraBars.LinkPersistInfo(this.barButtonItemCloseAll, true),
            new DevExpress.XtraBars.LinkPersistInfo(this.barMdiChildrenListItem, true)});
			this.barSubItemWindow.MergeType = DevExpress.XtraBars.BarMenuMerge.MergeItems;
			this.barSubItemWindow.Name = "barSubItemWindow";
			// 
			// barButtonItemCloseAll
			// 
			this.barButtonItemCloseAll.Caption = "Close &All Windows";
			this.barButtonItemCloseAll.Id = 43;
			this.barButtonItemCloseAll.Name = "barButtonItemCloseAll";
			this.barButtonItemCloseAll.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.barButtonItemCloseAll_ItemClick);
			// 
			// barMdiChildrenListItem
			// 
			this.barMdiChildrenListItem.Caption = "Window List";
			this.barMdiChildrenListItem.Id = 37;
			this.barMdiChildrenListItem.Name = "barMdiChildrenListItem";
			// 
			// barSubItemHelp
			// 
			this.barSubItemHelp.Caption = "Help";
			this.barSubItemHelp.Id = 4;
			this.barSubItemHelp.LinksPersistInfo.AddRange(new DevExpress.XtraBars.LinkPersistInfo[] {
            new DevExpress.XtraBars.LinkPersistInfo(this.cAbout, true)});
			this.barSubItemHelp.MergeType = DevExpress.XtraBars.BarMenuMerge.MergeItems;
			this.barSubItemHelp.Name = "barSubItemHelp";
			// 
			// cAbout
			// 
			this.cAbout.Caption = "About";
			this.cAbout.ContainerId = "About";
			this.cAbout.Id = 15;
			this.cAbout.MergeType = DevExpress.XtraBars.BarMenuMerge.MergeItems;
			this.cAbout.Name = "cAbout";
			// 
			// StandardToolBar
			// 
			this.StandardToolBar.BarName = "Main Toolbar";
			this.StandardToolBar.DockCol = 0;
			this.StandardToolBar.DockRow = 1;
			this.StandardToolBar.DockStyle = DevExpress.XtraBars.BarDockStyle.Top;
			this.StandardToolBar.LinksPersistInfo.AddRange(new DevExpress.XtraBars.LinkPersistInfo[] {
            new DevExpress.XtraBars.LinkPersistInfo(this.cObjectsCreation, true),
            new DevExpress.XtraBars.LinkPersistInfo(this.cView, true),
            new DevExpress.XtraBars.LinkPersistInfo(this.cRecordEdit, true),
            new DevExpress.XtraBars.LinkPersistInfo(this.cFiltersSearch, true),
            new DevExpress.XtraBars.LinkPersistInfo(this.cFilters, true),
            new DevExpress.XtraBars.LinkPersistInfo(this.cRecordsNavigation, true)});
			this.StandardToolBar.Text = "Main Toolbar";
			// 
			// cFiltersSearch
			// 
			this.cFiltersSearch.Caption = "Search";
			this.cFiltersSearch.ContainerId = "Search";
			this.cFiltersSearch.Id = 11;
			this.cFiltersSearch.MergeType = DevExpress.XtraBars.BarMenuMerge.MergeItems;
			this.cFiltersSearch.Name = "cFiltersSearch";
			// 
			// cFilters
			// 
			this.cFilters.Caption = "Filters";
			this.cFilters.ContainerId = "Filters";
			this.cFilters.Id = 26;
			this.cFilters.MergeType = DevExpress.XtraBars.BarMenuMerge.MergeItems;
			this.cFilters.Name = "cFilters";
			// 
			// StatusBar
			// 
			this.StatusBar.BarName = "StatusBar";
			this.StatusBar.CanDockStyle = DevExpress.XtraBars.BarCanDockStyle.Bottom;
			this.StatusBar.DockCol = 0;
			this.StatusBar.DockRow = 0;
			this.StatusBar.DockStyle = DevExpress.XtraBars.BarDockStyle.Bottom;
			this.StatusBar.OptionsBar.AllowQuickCustomization = false;
			this.StatusBar.OptionsBar.DisableClose = true;
			this.StatusBar.OptionsBar.DisableCustomization = true;
			this.StatusBar.OptionsBar.DrawDragBorder = false;
			this.StatusBar.OptionsBar.DrawSizeGrip = true;
			this.StatusBar.OptionsBar.UseWholeRow = true;
			this.StatusBar.Text = "Status Bar";
			// 
			// barAndDockingController1
			// 
			this.barAndDockingController1.PropertiesBar.AllowLinkLighting = false;
			// 
			// dockManager1
			// 
			this.dockManager1.Controller = this.barAndDockingController1;
			this.dockManager1.Form = this;
			this.dockManager1.RootPanels.AddRange(new DevExpress.XtraBars.Docking.DockPanel[] {
            this.dockPanelNavigation});
			this.dockManager1.TopZIndexControls.AddRange(new string[] {
            "DevExpress.XtraBars.BarDockControl",
            "System.Windows.Forms.StatusBar"});
			// 
			// dockPanelNavigation
			// 
			this.dockPanelNavigation.Controls.Add(this.dockPanelNavigation_Container);
			this.dockPanelNavigation.Dock = DevExpress.XtraBars.Docking.DockingStyle.Left;
			this.dockPanelNavigation.FloatSize = new System.Drawing.Size(146, 428);
			this.dockPanelNavigation.ID = new System.Guid("24977e30-0ea6-44aa-8fa4-9abaeb178b5e");
			this.dockPanelNavigation.Location = new System.Drawing.Point(0, 49);
			this.dockPanelNavigation.Name = "dockPanelNavigation";
			this.dockPanelNavigation.Options.AllowDockBottom = false;
			this.dockPanelNavigation.Options.AllowDockTop = false;
			this.dockPanelNavigation.SavedDock = DevExpress.XtraBars.Docking.DockingStyle.Left;
			this.dockPanelNavigation.SavedIndex = 2;
			this.dockPanelNavigation.Size = new System.Drawing.Size(146, 495);
			this.dockPanelNavigation.TabStop = false;
			this.dockPanelNavigation.Text = "Navigation";
			this.dockPanelNavigation.ClosingPanel += new DevExpress.XtraBars.Docking.DockPanelCancelEventHandler(this.dockPanelNavigation_ClosingPanel);
			// 
			// dockPanelNavigation_Container
			// 
			this.dockPanelNavigation_Container.Controls.Add(this.navigation);
			this.dockPanelNavigation_Container.Dock = System.Windows.Forms.DockStyle.Fill;
			this.dockPanelNavigation_Container.Location = new System.Drawing.Point(3, 25);
			this.dockPanelNavigation_Container.Name = "dockPanelNavigation_Container";
			this.dockPanelNavigation_Container.Size = new System.Drawing.Size(140, 467);
			this.dockPanelNavigation_Container.TabIndex = 0;
			// 
			// navigation
			// 
			this.navigation.Dock = System.Windows.Forms.DockStyle.Fill;
			this.navigation.Location = new System.Drawing.Point(0, 0);
			this.navigation.Name = "navigation";
			this.navigation.Size = new System.Drawing.Size(140, 467);
			// 
			// xtraTabbedMdiManager1
			// 
			this.xtraTabbedMdiManager1.Controller = this.barAndDockingController1;
			this.xtraTabbedMdiManager1.MdiParent = this;
			this.xtraTabbedMdiManager1.PageAdded += new DevExpress.XtraTabbedMdi.MdiTabPageEventHandler(this.xtraTabbedMdiManager1_PageAdded);
			this.xtraTabbedMdiManager1.SelectedPageChanged += new System.EventHandler(this.xtraTabbedMdiManager1_SelectedPageChanged);
			// 
			// MDIMainForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(792, 566);
			this.Controls.Add(this.dockPanelNavigation);
			this.Controls.Add(this.barDockControlLeft);
			this.Controls.Add(this.barDockControlRight);
			this.Controls.Add(this.barDockControlBottom);
			this.Controls.Add(this.barDockControlTop);
			this.IsMdiContainer = true;
			this.Name = "MDIMainForm";
			this.Text = "MDIMainForm";
			((System.ComponentModel.ISupportInitialize)(this.barManager)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.barAndDockingController1)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.dockManager1)).EndInit();
			this.dockPanelNavigation.ResumeLayout(false);
			this.dockPanelNavigation_Container.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.xtraTabbedMdiManager1)).EndInit();
			this.ResumeLayout(false);

		}
		private DevExpress.XtraBars.BarManager barManager;
		private DevExpress.XtraBars.BarDockControl barDockControlTop;
		private DevExpress.XtraBars.BarDockControl barDockControlBottom;
		private DevExpress.XtraBars.BarDockControl barDockControlLeft;
		private DevExpress.XtraBars.BarDockControl barDockControlRight;
		private DevExpress.XtraBars.Bar MainMenuBar;
		private DevExpress.XtraBars.Bar StandardToolBar;
		private DevExpress.XtraBars.Bar StatusBar;
		private DevExpress.XtraBars.BarAndDockingController barAndDockingController1;
		private DevExpress.XtraBars.Docking.DockManager dockManager1;
		private DevExpress.ExpressApp.Win.Templates.ActionContainers.ActionContainerBarItem cFile;
		private DevExpress.ExpressApp.Win.Templates.ActionContainers.ActionContainerBarItem cObjectsCreation;
		private DevExpress.ExpressApp.Win.Templates.ActionContainers.ActionContainerBarItem cPrint;
		private DevExpress.ExpressApp.Win.Templates.ActionContainers.ActionContainerBarItem cExport;
		private DevExpress.ExpressApp.Win.Templates.ActionContainers.ActionContainerBarItem cExit;
		private DevExpress.ExpressApp.Win.Templates.MainMenuItem barSubItemFile;
		private DevExpress.ExpressApp.Win.Templates.MainMenuItem barSubItemEdit;
		private DevExpress.ExpressApp.Win.Templates.MainMenuItem barSubItemView;
		private DevExpress.ExpressApp.Win.Templates.MainMenuItem barSubItemTools;
		private DevExpress.ExpressApp.Win.Templates.MainMenuItem barSubItemHelp;
		private DevExpress.ExpressApp.Win.Templates.ActionContainers.ActionContainerBarItem cRecordEdit;
		private DevExpress.ExpressApp.Win.Templates.ActionContainers.ActionContainerBarItem cRecordsNavigation;
		private DevExpress.ExpressApp.Win.Templates.ActionContainers.ActionContainerBarItem cFiltersSearch;
		private DevExpress.ExpressApp.Win.Templates.ActionContainers.ActionContainerBarItem cFilters;
		private DevExpress.ExpressApp.Win.Templates.ActionContainers.ActionContainerBarItem cView;
		private DevExpress.ExpressApp.Win.Templates.ActionContainers.ActionContainerMenuBarItem cTools;
		private DevExpress.ExpressApp.Win.Templates.ActionContainers.ActionContainerMenuBarItem cDiagnostic;
		private DevExpress.ExpressApp.Win.Templates.ActionContainers.ActionContainerMenuBarItem cOptions;
		private DevExpress.ExpressApp.Win.Templates.ActionContainers.ActionContainerMenuBarItem cAbout;
		private DevExpress.ExpressApp.Win.Templates.ActionContainers.NavigationActionContainer navigation;
		private DevExpress.XtraTabbedMdi.XtraTabbedMdiManager xtraTabbedMdiManager1;
		private DevExpress.ExpressApp.Win.Templates.MainMenuItem barSubItemWindow;
		private DevExpress.XtraBars.BarMdiChildrenListItem barMdiChildrenListItem;
		private BarButtonItem barButtonItemCloseAll;
		private DevExpress.XtraBars.Docking.DockPanel dockPanelNavigation;
		private DevExpress.XtraBars.Docking.ControlContainer dockPanelNavigation_Container;
		private DevExpress.XtraBars.BarCheckItem barCheckItemNavigationPaneVisibility;

		#endregion
		
	}
}
