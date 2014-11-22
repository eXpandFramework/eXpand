using System.Drawing;
using System.Windows.Forms;
using DevExpress.XtraLayout;
using DevExpress.ExpressApp.Win.Layout;
using DevExpress.ExpressApp.Win.Core;
using DevExpress.ExpressApp.Win.Templates;
namespace FeatureCenter.Module.Win {
    partial class PopupForm {
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

        

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent() {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new XafComponentResourceManager(typeof(PopupForm));
            this.xafBarManager1 = new DevExpress.ExpressApp.Win.Templates.Controls.XafBarManager(this.components);
            this.barDockControlTop = new DevExpress.XtraBars.BarDockControl();
            this.barDockControlBottom = new DevExpress.XtraBars.BarDockControl();
            this.barDockControlLeft = new DevExpress.XtraBars.BarDockControl();
            this.barDockControlRight = new DevExpress.XtraBars.BarDockControl();
            this.cObjectsCreation = new DevExpress.ExpressApp.Win.Templates.ActionContainers.ActionContainerBarItem();
            this.cRecordEdit = new DevExpress.ExpressApp.Win.Templates.ActionContainers.ActionContainerBarItem();
            this.cView = new DevExpress.ExpressApp.Win.Templates.ActionContainers.ActionContainerBarItem();
            this.cPrint = new DevExpress.ExpressApp.Win.Templates.ActionContainers.ActionContainerBarItem();
            this.cEdit = new DevExpress.ExpressApp.Win.Templates.ActionContainers.ActionContainerBarItem();
            this.cOpenObject = new DevExpress.ExpressApp.Win.Templates.ActionContainers.ActionContainerBarItem();
            this.cUndoRedo = new DevExpress.ExpressApp.Win.Templates.ActionContainers.ActionContainerBarItem();
            this.cExport = new DevExpress.ExpressApp.Win.Templates.ActionContainers.ActionContainerBarItem();
            this.actionContainersManager1 = new DevExpress.ExpressApp.Win.Templates.ActionContainersManager(this.components);
            this.buttonsContainer1 = new DevExpress.ExpressApp.Win.Templates.ActionContainers.ButtonsContainer();
            this.Root = new DevExpress.XtraLayout.LayoutControlGroup();
            this.viewSitePanel = new DevExpress.XtraEditors.PanelControl();
            this.viewSiteControlPanel = new DevExpress.XtraEditors.PanelControl();
            this.hintPanel = new DevExpress.Utils.Frames.NotePanel8_1();
            this.bottomPanel = new DevExpress.ExpressApp.Win.Layout.XafLayoutControl();
            this.layoutControlGroup1 = new DevExpress.XtraLayout.LayoutControlGroup();
            this.simpleSeparator1 = new DevExpress.XtraLayout.SimpleSeparator();
            this.emptySpaceItem1 = new DevExpress.XtraLayout.EmptySpaceItem();
            this.layoutControlItem1 = new DevExpress.XtraLayout.LayoutControlItem();
            this.formStateModelSynchronizer = new DevExpress.ExpressApp.Win.Core.FormStateModelSynchronizer(this.components);
            this.viewSiteManager = new ViewSiteManager(components);
            ((System.ComponentModel.ISupportInitialize)(this.xafBarManager1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.buttonsContainer1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.Root)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.viewSitePanel)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.viewSiteControlPanel)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.bottomPanel)).BeginInit();
            this.bottomPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.simpleSeparator1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem1)).BeginInit();
            this.SuspendLayout();
            // 
            // xafBarManager1
            // 
            this.xafBarManager1.DockControls.Add(this.barDockControlTop);
            this.xafBarManager1.DockControls.Add(this.barDockControlBottom);
            this.xafBarManager1.DockControls.Add(this.barDockControlLeft);
            this.xafBarManager1.DockControls.Add(this.barDockControlRight);
            this.xafBarManager1.Form = this;
            this.xafBarManager1.Items.AddRange(new DevExpress.XtraBars.BarItem[] {
            this.cObjectsCreation,
            this.cRecordEdit,
            this.cView,
            this.cPrint,
            this.cEdit,
            this.cOpenObject,
            this.cUndoRedo,
            this.cExport});
            this.xafBarManager1.MaxItemId = 8;
            // 
            // barDockControlTop
            // 
            this.barDockControlTop.CausesValidation = false;
            resources.ApplyResources(this.barDockControlTop, "barDockControlTop");
            // 
            // barDockControlBottom
            // 
            this.barDockControlBottom.CausesValidation = false;
            resources.ApplyResources(this.barDockControlBottom, "barDockControlBottom");
            // 
            // barDockControlLeft
            // 
            this.barDockControlLeft.CausesValidation = false;
            resources.ApplyResources(this.barDockControlLeft, "barDockControlLeft");
            // 
            // barDockControlRight
            // 
            this.barDockControlRight.CausesValidation = false;
            resources.ApplyResources(this.barDockControlRight, "barDockControlRight");
            // 
            // cObjectsCreation
            // 
            resources.ApplyResources(this.cObjectsCreation, "cObjectsCreation");
            this.cObjectsCreation.ContainerId = "ObjectsCreation";
            this.cObjectsCreation.Id = 0;
            this.cObjectsCreation.Name = "cObjectsCreation";
            this.cObjectsCreation.TargetPageCategoryColor = System.Drawing.Color.Empty;
            this.cObjectsCreation.TargetPageGroupCaption = null;
            // 
            // cRecordEdit
            // 
            resources.ApplyResources(this.cRecordEdit, "cRecordEdit");
            this.cRecordEdit.ContainerId = "RecordEdit";
            this.cRecordEdit.Id = 1;
            this.cRecordEdit.Name = "cRecordEdit";
            this.cRecordEdit.TargetPageCategoryColor = System.Drawing.Color.Empty;
            this.cRecordEdit.TargetPageGroupCaption = null;
            // 
            // cView
            // 
            resources.ApplyResources(this.cView, "cView");
            this.cView.ContainerId = "View";
            this.cView.Id = 2;
            this.cView.Name = "cView";
            this.cView.TargetPageCategoryColor = System.Drawing.Color.Empty;
            this.cView.TargetPageGroupCaption = null;
            // 
            // cPrint
            // 
            resources.ApplyResources(this.cPrint, "cPrint");
            this.cPrint.ContainerId = "Print";
            this.cPrint.Id = 3;
            this.cPrint.Name = "cPrint";
            this.cPrint.TargetPageCategoryColor = System.Drawing.Color.Empty;
            this.cPrint.TargetPageGroupCaption = null;
            // 
            // cEdit
            // 
            resources.ApplyResources(this.cEdit, "cEdit");
            this.cEdit.ContainerId = "Edit";
            this.cEdit.Id = 4;
            this.cEdit.Name = "cEdit";
            this.cEdit.TargetPageCategoryColor = System.Drawing.Color.Empty;
            this.cEdit.TargetPageGroupCaption = null;
            // 
            // cOpenObject
            // 
            resources.ApplyResources(this.cOpenObject, "cOpenObject");
            this.cOpenObject.ContainerId = "OpenObject";
            this.cOpenObject.Id = 5;
            this.cOpenObject.Name = "cOpenObject";
            this.cOpenObject.TargetPageCategoryColor = System.Drawing.Color.Empty;
            this.cOpenObject.TargetPageGroupCaption = null;
            // 
            // cUndoRedo
            // 
            resources.ApplyResources(this.cUndoRedo, "cUndoRedo");
            this.cUndoRedo.ContainerId = "UndoRedo";
            this.cUndoRedo.Id = 6;
            this.cUndoRedo.Name = "cUndoRedo";
            this.cUndoRedo.TargetPageCategoryColor = System.Drawing.Color.Empty;
            this.cUndoRedo.TargetPageGroupCaption = null;
            // 
            // cExport
            // 
            resources.ApplyResources(this.cExport, "cExport");
            this.cExport.ContainerId = "Export";
            this.cExport.Id = 7;
            this.cExport.Name = "cExport";
            this.cExport.TargetPageCategoryColor = System.Drawing.Color.Empty;
            this.cExport.TargetPageGroupCaption = null;
            // 
            // actionContainersManager1
            // 
            this.actionContainersManager1.ActionContainerComponents.Add(this.cObjectsCreation);
            this.actionContainersManager1.ActionContainerComponents.Add(this.cRecordEdit);
            this.actionContainersManager1.ActionContainerComponents.Add(this.cView);
            this.actionContainersManager1.ActionContainerComponents.Add(this.cPrint);
            this.actionContainersManager1.ActionContainerComponents.Add(this.cEdit);
            this.actionContainersManager1.ActionContainerComponents.Add(this.cOpenObject);
            this.actionContainersManager1.ActionContainerComponents.Add(this.cUndoRedo);
            this.actionContainersManager1.ActionContainerComponents.Add(this.cExport);
            this.actionContainersManager1.ActionContainerComponents.Add(this.buttonsContainer1);
            this.actionContainersManager1.ContextMenuContainers.Add(this.cObjectsCreation);
            this.actionContainersManager1.ContextMenuContainers.Add(this.cEdit);
            this.actionContainersManager1.ContextMenuContainers.Add(this.cRecordEdit);
            this.actionContainersManager1.ContextMenuContainers.Add(this.cUndoRedo);
            this.actionContainersManager1.ContextMenuContainers.Add(this.cOpenObject);
            this.actionContainersManager1.ContextMenuContainers.Add(this.cView);
            this.actionContainersManager1.ContextMenuContainers.Add(this.cExport);
            this.actionContainersManager1.ContextMenuContainers.Add(this.cPrint);
            this.actionContainersManager1.DefaultContainer = this.cObjectsCreation;
            this.actionContainersManager1.Template = this;
            // 
            // buttonsContainer1
            // 
            this.buttonsContainer1.ActionId = null;
            this.buttonsContainer1.AllowCustomizationMenu = false;
            resources.ApplyResources(this.buttonsContainer1, "buttonsContainer1");
            this.buttonsContainer1.ContainerId = "PopupActions";
            this.buttonsContainer1.Name = "buttonsContainer1";
            this.buttonsContainer1.Orientation = DevExpress.ExpressApp.Model.ActionContainerOrientation.Horizontal;
            this.buttonsContainer1.PaintStyle = DevExpress.ExpressApp.Templates.ActionItemPaintStyle.CaptionAndImage;
            this.buttonsContainer1.Root = this.Root;
            
            // 
            // Root
            // 
            resources.ApplyResources(this.Root, "Root");
            this.Root.DefaultLayoutType = DevExpress.XtraLayout.Utils.LayoutType.Horizontal;
            this.Root.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
            this.Root.GroupBordersVisible = false;
            this.Root.Location = new System.Drawing.Point(0, 0);
            this.Root.Name = "Root";
            this.Root.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 7, 2, 0);
            this.Root.Size = new System.Drawing.Size(497, 29);
            this.Root.Spacing = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
            this.Root.TextVisible = false;
            // 
            // viewSitePanel
            // 
            this.viewSitePanel.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
            resources.ApplyResources(this.viewSitePanel, "viewSitePanel");
            this.viewSitePanel.Name = "viewSitePanel";
            // 
            // viewSiteControlPanel
            // 
            this.viewSiteControlPanel.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
            this.viewSiteControlPanel.Controls.Add(this.hintPanel);
            this.viewSiteControlPanel.Controls.Add(this.viewSitePanel);
            resources.ApplyResources(this.viewSiteControlPanel, "viewSiteControlPanel");
            this.viewSiteControlPanel.Name = "viewSiteControlPanel";
            // 
            // hintPanel
            // 
            resources.ApplyResources(this.hintPanel, "hintPanel");
            this.hintPanel.MaxRows = 25;
            this.hintPanel.Name = "hintPanel";
            this.hintPanel.TabStop = false;
            this.hintPanel.SizeChanged += new System.EventHandler(this.hintPanel_SizeChanged);
            // 
            // bottomPanel
            // 
            this.bottomPanel.AllowCustomizationMenu = false;
            resources.ApplyResources(this.bottomPanel, "bottomPanel");
            this.bottomPanel.Controls.Add(this.buttonsContainer1);
            this.bottomPanel.Name = "bottomPanel";
            this.bottomPanel.OptionsCustomizationForm.ShowLoadButton = false;
            this.bottomPanel.OptionsCustomizationForm.ShowSaveButton = false;
            this.bottomPanel.OptionsView.AllowItemSkinning = false;
            this.bottomPanel.OptionsView.EnableIndentsInGroupsWithoutBorders = true;
            this.bottomPanel.Root = this.layoutControlGroup1;
            // 
            // layoutControlGroup1
            // 
            resources.ApplyResources(this.layoutControlGroup1, "layoutControlGroup1");
            this.layoutControlGroup1.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
            this.layoutControlGroup1.GroupBordersVisible = false;
            this.layoutControlGroup1.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
            this.simpleSeparator1,
            this.emptySpaceItem1,
            this.layoutControlItem1});
            this.layoutControlGroup1.Location = new System.Drawing.Point(0, 0);
            this.layoutControlGroup1.Name = "Root";
            this.layoutControlGroup1.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
            this.layoutControlGroup1.Size = new System.Drawing.Size(552, 45);
            this.layoutControlGroup1.Spacing = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
            this.layoutControlGroup1.TextVisible = false;
            // 
            // simpleSeparator1
            // 
            resources.ApplyResources(this.simpleSeparator1, "simpleSeparator1");
            this.simpleSeparator1.Location = new System.Drawing.Point(0, 0);
            this.simpleSeparator1.Name = "simpleSeparator1";
            this.simpleSeparator1.Size = new System.Drawing.Size(552, 2);
            // 
            // emptySpaceItem1
            // 
            resources.ApplyResources(this.emptySpaceItem1, "emptySpaceItem1");
            this.emptySpaceItem1.Location = new System.Drawing.Point(0, 2);
            this.emptySpaceItem1.Name = "emptySpaceItem1";
            this.emptySpaceItem1.Size = new System.Drawing.Size(51, 43);
            this.emptySpaceItem1.TextSize = new System.Drawing.Size(0, 0);
            // 
            // layoutControlItem1
            // 
            this.layoutControlItem1.Control = this.buttonsContainer1;
            resources.ApplyResources(this.layoutControlItem1, "layoutControlItem1");
            this.layoutControlItem1.Location = new System.Drawing.Point(51, 2);
            this.layoutControlItem1.Name = "layoutControlItem1";
            this.layoutControlItem1.Size = new System.Drawing.Size(501, 33);
            this.layoutControlItem1.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem1.TextToControlDistance = 0;
            this.layoutControlItem1.TextVisible = false;
            // 
            // formStateModelSynchronizer
            // 
            this.formStateModelSynchronizer.Form = this;
            this.formStateModelSynchronizer.Model = null;
            //
            // viewSiteManager
            //
            this.viewSiteManager.UseDefferedLoading = false;
            this.viewSiteManager.ViewSiteControl = this.viewSitePanel;
            // 
            // PopupForm
            // 
            resources.ApplyResources(this, "$this");
            this.Controls.Add(this.viewSiteControlPanel);
            this.Controls.Add(this.bottomPanel);
            this.Controls.Add(this.barDockControlLeft);
            this.Controls.Add(this.barDockControlRight);
            this.Controls.Add(this.barDockControlBottom);
            this.Controls.Add(this.barDockControlTop);
            this.Name = "PopupForm";
            ((System.ComponentModel.ISupportInitialize)(this.xafBarManager1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.buttonsContainer1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.Root)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.viewSitePanel)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.viewSiteControlPanel)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.bottomPanel)).EndInit();
            this.bottomPanel.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.simpleSeparator1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem1)).EndInit();
            this.ResumeLayout(false);

        }

        private DevExpress.ExpressApp.Win.Templates.Controls.XafBarManager xafBarManager1;
        private DevExpress.ExpressApp.Win.Templates.ActionContainers.ActionContainerBarItem cObjectsCreation;
        private DevExpress.ExpressApp.Win.Templates.ActionContainers.ActionContainerBarItem cRecordEdit;
        private DevExpress.ExpressApp.Win.Templates.ActionContainers.ActionContainerBarItem cView;
        private DevExpress.ExpressApp.Win.Templates.ActionContainers.ActionContainerBarItem cPrint;
        private DevExpress.ExpressApp.Win.Templates.ActionContainers.ActionContainerBarItem cEdit;
        private DevExpress.ExpressApp.Win.Templates.ActionContainers.ActionContainerBarItem cOpenObject;
        private DevExpress.XtraBars.BarDockControl barDockControlTop;
        private DevExpress.XtraBars.BarDockControl barDockControlBottom;
        private DevExpress.XtraBars.BarDockControl barDockControlLeft;
        private DevExpress.XtraBars.BarDockControl barDockControlRight;
        private ActionContainersManager actionContainersManager1;
        private DevExpress.ExpressApp.Win.Templates.ActionContainers.ActionContainerBarItem cUndoRedo;
        private DevExpress.ExpressApp.Win.Templates.ActionContainers.ActionContainerBarItem cExport;
        public DevExpress.Utils.Frames.NotePanel8_1 hintPanel;
        private XafLayoutControl bottomPanel;
        private LayoutControlGroup layoutControlGroup1;
        private DevExpress.ExpressApp.Win.Templates.ActionContainers.ButtonsContainer buttonsContainer1;
        private LayoutControlGroup Root;
        private SimpleSeparator simpleSeparator1;
        private EmptySpaceItem emptySpaceItem1;
        private LayoutControlItem layoutControlItem1;
        private FormStateModelSynchronizer formStateModelSynchronizer;
        protected DevExpress.XtraEditors.PanelControl viewSiteControlPanel;
        protected DevExpress.XtraEditors.PanelControl viewSitePanel;
        private ViewSiteManager viewSiteManager;
    }
}
