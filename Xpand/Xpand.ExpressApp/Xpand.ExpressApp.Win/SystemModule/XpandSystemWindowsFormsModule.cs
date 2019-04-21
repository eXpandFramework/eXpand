using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Model.Core;
using DevExpress.ExpressApp.SystemModule;
using DevExpress.ExpressApp.Updating;
using DevExpress.ExpressApp.Win;
using DevExpress.ExpressApp.Win.SystemModule;
using DevExpress.Utils;
using Xpand.ExpressApp.SystemModule;
using Xpand.ExpressApp.Win.ListEditors.GridListEditors.AdvBandedView.Model;
using Xpand.ExpressApp.Win.ListEditors.GridListEditors.ColumnView;
using Xpand.ExpressApp.Win.ListEditors.GridListEditors.ColumnView.RepositoryItems;
using Xpand.ExpressApp.Win.ListEditors.GridListEditors.GridView.Model;
using Xpand.ExpressApp.Win.ListEditors.GridListEditors.LayoutView;
using Xpand.ExpressApp.Win.ListEditors.GridListEditors.LayoutView.Model;
using Xpand.ExpressApp.Win.Model;
using Xpand.ExpressApp.Win.Model.NodeUpdaters;
using Xpand.ExpressApp.Win.PropertyEditors;
using Xpand.ExpressApp.Win.PropertyEditors.RichEdit;
using Xpand.ExpressApp.Win.SystemModule.ModelAdapters;
using Xpand.ExpressApp.Win.SystemModule.ToolTip;
using Xpand.Persistent.Base.General;
using Xpand.Persistent.Base.General.Model;
using Xpand.Persistent.Base.General.Model.Options;
using ProcessDataLockingInfoController = Xpand.ExpressApp.Win.PropertyEditors.ProcessDataLockingInfoController;

namespace Xpand.ExpressApp.Win.SystemModule {
    [ToolboxItem(true)]
    [ToolboxTabName(XpandAssemblyInfo.TabWinModules)]
    [Description("Overrides Controllers from the SystemModule and supplies additional basic Controllers that are specific for Windows Forms applications.")]
    [Browsable(true)]
    [EditorBrowsable(EditorBrowsableState.Always)]
    [ToolboxBitmap(typeof(WinApplication), "Resources.Toolbox_Module_System_Win.ico")]
    public sealed class XpandSystemWindowsFormsModule : XpandModuleBase, IColumnCellFilterUser,IModelXmlConverter,IGridOptionsUser {
        public const string XpandWin = "Xpand.Win";
        public XpandSystemWindowsFormsModule() {
            RequiredModuleTypes.Add(typeof(XpandSystemModule));
            RequiredModuleTypes.Add(typeof(SystemWindowsFormsModule));
        }
        public override void ExtendModelInterfaces(ModelInterfaceExtenders extenders) {
            base.ExtendModelInterfaces(extenders);
            extenders.Add<IModelRootNavigationItems, IModelRootNavigationItemsAutoSelectedGroupItem>();
            extenders.Add<IModelMemberViewItem, IModelMemberViewItemFastSearch>();
            extenders.Add<IModelMemberViewItem, IModelMemberViewItemDuration>();
        }

        protected override IEnumerable<Type> GetDeclaredExportedTypes() {
            return new List<Type>();
        }

        public override void Setup(XafApplication application) {
            base.Setup(application);
            application.SetupComplete+=ApplicationOnSetupComplete;
        }

        private void ApplicationOnSetupComplete(object sender, EventArgs e) {
            CurrentProcessController.ApplyConfig(Application.Model.Options);
        }

        protected override IEnumerable<Type> GetDeclaredControllerTypes(){
            Type[] controllerTypes = {
                typeof(PopupWindowStateController),
                typeof(CurrentProcessController),
                typeof(RefreshObjectViewController),
                typeof(DragNDropImageController),
                typeof(RibbonFromModelController),
                typeof(SplitContainerControlModelAdapterController),
                typeof(ProcessDataLockingInfoController),
                typeof(DatabaseMaintenanceController),
                typeof(AutoScrollGridListEditorController),
                typeof(EditModelController),
                typeof(ApplicationMultiInstancesController),
                typeof(AutoExpandNewRowController),
                typeof(ApplicationExitController),
                typeof(ActiveDocumentViewController),
                typeof(LayoutControlGroupModelAdapterController),
                typeof(FilterByGridViewColumnController),
                typeof(FullTextAutoFilterRowController),
                typeof(GridListEditorEventController),
                typeof(CustomSummaryCalculateController),
                typeof(GroupedRowMasterDetailViewController),
                typeof(ImmediatePostDataController),
                typeof(OpenWithController),
                typeof(SelectedItemSumController),
                typeof(XafLayoutControlModelAdapterController),
                typeof(EMailHighlightingController),
                typeof(UnboundColumnController),
                typeof(CursorPositionController),
                typeof(CriteriaPropertyEditorControlAdapterController),
                typeof(PreventDataLoadingGridViewController),
                typeof(NewObjectCollectCreatableItemTypesDataSource),
                typeof(PessimisticLockingViewController),
                typeof(SelectFirstNavigationItemController),
                typeof(FilterByPropertyPathViewController),
                typeof(FilterControlListViewController),
                typeof(FocusControlByShortcutController),
                typeof(GuessAutoFilterRowValuesFromFilterController),
                typeof(HideGridPopUpMenuViewController),
                typeof(HideToolBarController),
                typeof(HighlightFocusedLayoutItemDetailViewController),
                typeof(LockListEditorDataUpdatesController),
                typeof(LoadWithWindowsController),
                typeof(CloseFormController),
                typeof(NotifyIconController),
                typeof(ReadOnlyTabStopController),
                typeof(RemoveNavigationItemsController),
                typeof(SearchFromDetailViewController),
                typeof(ViewEditValueChangedFiringModeController),
                typeof(WindowHintController),
                typeof(GridViewImageTextToolTipController),
                typeof(WinToolTipsController),
                typeof(HyperLinkGridListViewController),
                typeof(LabelControlModelAdapterController),
                typeof(RichEditToolbarController),
                typeof(RichEditModelAdapterController),
                typeof(LayoutViewColumnChooserController),
                typeof(LayoutViewModelAdapterController),
                typeof(GridViewModelAdapterController),
                typeof(RememberGridSelectionController),
                typeof(RepositoryItemModelAdapterController),
                typeof(AdvBandedViewModelAdapterController)
            };
            return FilterDisabledControllers(GetDeclaredControllerTypesCore(controllerTypes));
        }

        public override void AddModelNodeUpdaters(IModelNodeUpdaterRegistrator updaterRegistrator){
            base.AddModelNodeUpdaters(updaterRegistrator);
            updaterRegistrator.AddUpdater(new ModelOptionsAdvBandedViewUpdater());
        }

        void IModelXmlConverter.ConvertXml(ConvertXmlParameters parameters) {
            ConvertXml(parameters);
            if (parameters.XmlNodeName == "GridColumnOptions")
                parameters.NodeType = typeof (IModelOptionsColumnGridView);
            
        }

    }


}