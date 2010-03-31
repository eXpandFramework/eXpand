namespace eXpand.ExpressApp.Win.Templates {
	partial class MDIChildForm {
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
			this.MainMenuBar = new DevExpress.XtraBars.Bar();
			this.barSubItemFile = new DevExpress.ExpressApp.Win.Templates.MainMenuItem();
			this.cObjectsCreation = new DevExpress.ExpressApp.Win.Templates.ActionContainers.ActionContainerBarItem();
			this.cFile = new DevExpress.ExpressApp.Win.Templates.ActionContainers.ActionContainerBarItem();
			this.cClose = new DevExpress.ExpressApp.Win.Templates.ActionContainers.ActionContainerBarItem();
			this.cPrint = new DevExpress.ExpressApp.Win.Templates.ActionContainers.ActionContainerBarItem();
			this.cExport = new DevExpress.ExpressApp.Win.Templates.ActionContainers.ActionContainerBarItem();
			this.barSubItemEdit = new DevExpress.ExpressApp.Win.Templates.MainMenuItem();
			this.cRecordEdit = new DevExpress.ExpressApp.Win.Templates.ActionContainers.ActionContainerBarItem();
			this.barSubItemTools = new DevExpress.ExpressApp.Win.Templates.MainMenuItem();
			this.cTools = new DevExpress.ExpressApp.Win.Templates.ActionContainers.ActionContainerMenuBarItem();
			this.cOptions = new DevExpress.ExpressApp.Win.Templates.ActionContainers.ActionContainerMenuBarItem();
			this.cDiagnostic = new DevExpress.ExpressApp.Win.Templates.ActionContainers.ActionContainerMenuBarItem();
			this.barSubItemView = new DevExpress.ExpressApp.Win.Templates.MainMenuItem();
			this.cRecordsNavigation = new DevExpress.ExpressApp.Win.Templates.ActionContainers.ActionContainerBarItem();
			this.cView = new DevExpress.ExpressApp.Win.Templates.ActionContainers.ActionContainerBarItem();
			this.barSubItemHelp = new DevExpress.ExpressApp.Win.Templates.MainMenuItem();
			this.cAbout = new DevExpress.ExpressApp.Win.Templates.ActionContainers.ActionContainerMenuBarItem();
			this.StandardToolBar = new DevExpress.XtraBars.Bar();
			this.cFiltersSearch = new DevExpress.ExpressApp.Win.Templates.ActionContainers.ActionContainerBarItem();
			this.cFilters = new DevExpress.ExpressApp.Win.Templates.ActionContainers.ActionContainerBarItem();
			this.barDockControlTop = new DevExpress.XtraBars.BarDockControl();
			this.barDockControlBottom = new DevExpress.XtraBars.BarDockControl();
			this.barDockControlLeft = new DevExpress.XtraBars.BarDockControl();
			this.barDockControlRight = new DevExpress.XtraBars.BarDockControl();
			this.viewSitePanel = new DevExpress.XtraEditors.PanelControl();
			((System.ComponentModel.ISupportInitialize)(this.barManager)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.viewSitePanel)).BeginInit();
			this.SuspendLayout();
			// 
			// barManager
			// 
			this.barManager.Bars.AddRange(new DevExpress.XtraBars.Bar[] {
            this.MainMenuBar,
            this.StandardToolBar});
			this.barManager.DockControls.Add(this.barDockControlTop);
			this.barManager.DockControls.Add(this.barDockControlBottom);
			this.barManager.DockControls.Add(this.barDockControlLeft);
			this.barManager.DockControls.Add(this.barDockControlRight);
			this.barManager.Form = this;
			this.barManager.Items.AddRange(new DevExpress.XtraBars.BarItem[] {
            this.barSubItemFile,
            this.barSubItemEdit,
            this.barSubItemView,
            this.barSubItemTools,
            this.barSubItemHelp,
            this.cFile,
            this.cObjectsCreation,
            this.cClose,
            this.cPrint,
            this.cExport,
            this.cRecordEdit,
            this.cRecordsNavigation,
            this.cFiltersSearch,
			this.cFilters,
            this.cView,
            this.cTools,
            this.cOptions,
            this.cDiagnostic,
            this.cAbout});
			this.barManager.MainMenu = this.MainMenuBar;
			this.barManager.MaxItemId = 27;
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
            new DevExpress.XtraBars.LinkPersistInfo(this.barSubItemTools),
            new DevExpress.XtraBars.LinkPersistInfo(this.barSubItemView),
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
            new DevExpress.XtraBars.LinkPersistInfo(this.cClose, true),
            new DevExpress.XtraBars.LinkPersistInfo(this.cPrint, true),
            new DevExpress.XtraBars.LinkPersistInfo(this.cExport, true)});
			this.barSubItemFile.MergeType = DevExpress.XtraBars.BarMenuMerge.MergeItems;
			this.barSubItemFile.Name = "barSubItemFile";
			// 
			// cObjectsCreation
			// 
			this.cObjectsCreation.Caption = "Objects Creation";
			this.cObjectsCreation.ContainerId = "ObjectsCreation";
			this.cObjectsCreation.Id = 17;
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
			// cClose
			// 
			this.cClose.Caption = "Close";
			this.cClose.ContainerId = "Close";
			this.cClose.Id = 18;
			this.cClose.MergeType = DevExpress.XtraBars.BarMenuMerge.MergeItems;
			this.cClose.Name = "cClose";
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
			// barSubItemEdit
			// 
			this.barSubItemEdit.Caption = "Edit";
			this.barSubItemEdit.Id = 1;
			this.barSubItemEdit.LinksPersistInfo.AddRange(new DevExpress.XtraBars.LinkPersistInfo[] {
            new DevExpress.XtraBars.LinkPersistInfo(this.cRecordEdit, true),
            new DevExpress.XtraBars.LinkPersistInfo(this.cFiltersSearch, true), 
			new DevExpress.XtraBars.LinkPersistInfo(this.cFilters, true)
			});
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
			// barSubItemTools
			// 
			this.barSubItemTools.Caption = "Tools";
			this.barSubItemTools.Id = 3;
			this.barSubItemTools.LinksPersistInfo.AddRange(new DevExpress.XtraBars.LinkPersistInfo[] {
            new DevExpress.XtraBars.LinkPersistInfo(this.cTools, true),
            new DevExpress.XtraBars.LinkPersistInfo(this.cOptions, true),
            new DevExpress.XtraBars.LinkPersistInfo(this.cDiagnostic, true)});
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
			// cOptions
			// 
			this.cOptions.Caption = "Options";
			this.cOptions.ContainerId = "Options";
			this.cOptions.Id = 14;
			this.cOptions.MergeType = DevExpress.XtraBars.BarMenuMerge.MergeItems;
			this.cOptions.Name = "cOptions";
			// 
			// cDiagnostic
			// 
			this.cDiagnostic.Caption = "Diagnostic";
			this.cDiagnostic.ContainerId = "Diagnostic";
			this.cDiagnostic.Id = 16;
			this.cDiagnostic.MergeType = DevExpress.XtraBars.BarMenuMerge.MergeItems;
			this.cDiagnostic.Name = "cDiagnostic";
			// 
			// barSubItemView
			// 
			this.barSubItemView.Caption = "View";
			this.barSubItemView.Id = 2;
			this.barSubItemView.LinksPersistInfo.AddRange(new DevExpress.XtraBars.LinkPersistInfo[] {
            new DevExpress.XtraBars.LinkPersistInfo(this.cRecordsNavigation, true),
            new DevExpress.XtraBars.LinkPersistInfo(this.cView, true)});
			this.barSubItemView.MergeType = DevExpress.XtraBars.BarMenuMerge.MergeItems;
			this.barSubItemView.Name = "barSubItemView";
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
            new DevExpress.XtraBars.LinkPersistInfo(this.cClose, true),
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
			// viewSitePanel
			// 
			this.viewSitePanel.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			this.viewSitePanel.Dock = System.Windows.Forms.DockStyle.Fill;
			this.viewSitePanel.Location = new System.Drawing.Point(0, 49);
			this.viewSitePanel.Name = "viewSitePanel";
			this.viewSitePanel.Size = new System.Drawing.Size(792, 495);
			this.viewSitePanel.TabIndex = 4;
			// 
			// MDIChildForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(792, 566);
			this.Controls.Add(this.viewSitePanel);
			this.Controls.Add(this.barDockControlLeft);
			this.Controls.Add(this.barDockControlRight);
			this.Controls.Add(this.barDockControlBottom);
			this.Controls.Add(this.barDockControlTop);
			this.Name = "MDIChildForm";
			this.Text = "MDIChildForm";
			((System.ComponentModel.ISupportInitialize)(this.barManager)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.viewSitePanel)).EndInit();
			this.ResumeLayout(false);

		}

		private DevExpress.XtraBars.BarManager barManager;
		private DevExpress.XtraBars.BarDockControl barDockControlTop;
		private DevExpress.XtraBars.BarDockControl barDockControlBottom;
		private DevExpress.XtraBars.BarDockControl barDockControlLeft;
		private DevExpress.XtraBars.BarDockControl barDockControlRight;
		private DevExpress.XtraBars.Bar MainMenuBar;
		private DevExpress.XtraBars.Bar StandardToolBar;
		private DevExpress.ExpressApp.Win.Templates.ActionContainers.ActionContainerBarItem cFile;
		private DevExpress.ExpressApp.Win.Templates.ActionContainers.ActionContainerBarItem cObjectsCreation;
		private DevExpress.ExpressApp.Win.Templates.ActionContainers.ActionContainerBarItem cClose;
		private DevExpress.ExpressApp.Win.Templates.ActionContainers.ActionContainerBarItem cPrint;
		private DevExpress.ExpressApp.Win.Templates.ActionContainers.ActionContainerBarItem cExport;
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
		private DevExpress.ExpressApp.Win.Templates.ActionContainers.ActionContainerMenuBarItem cOptions;
		private DevExpress.ExpressApp.Win.Templates.ActionContainers.ActionContainerMenuBarItem cAbout;
		private DevExpress.ExpressApp.Win.Templates.ActionContainers.ActionContainerMenuBarItem cDiagnostic;
		private DevExpress.XtraEditors.PanelControl viewSitePanel;
		#endregion
	}
}
