using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Text;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.SystemModule;
using DevExpress.ExpressApp.Updating;
using DevExpress.ExpressApp.Win;
using DevExpress.ExpressApp.Win.SystemModule;
using DevExpress.Utils;
using Xpand.ExpressApp.SystemModule;
using Xpand.ExpressApp.Win.ListEditors.GridListEditors.ColumnView;
using Xpand.ExpressApp.Win.ListEditors.GridListEditors.LayoutView;
using Xpand.ExpressApp.Win.ListEditors.GridListEditors.LayoutView.Model;
using Xpand.ExpressApp.Win.Model;
using Xpand.ExpressApp.Win.PropertyEditors;
using Xpand.ExpressApp.Win.PropertyEditors.RichEdit;
using Xpand.ExpressApp.Win.SystemModule.ModelAdapters;
using Xpand.ExpressApp.Win.SystemModule.ToolTip;
using Xpand.Persistent.Base.General;
using Xpand.Persistent.Base.General.Model;
using Xpand.XAF.Modules.GridListEditor;
using Xpand.XAF.Modules.ModelMapper;
using Xpand.XAF.Modules.ModelMapper.Configuration;
using Xpand.XAF.Modules.ModelMapper.Services;
using ProcessDataLockingInfoController = Xpand.ExpressApp.Win.PropertyEditors.ProcessDataLockingInfoController;

namespace Xpand.ExpressApp.Win.SystemModule {
    [ToolboxItem(true)]
    [ToolboxTabName(XpandAssemblyInfo.TabWinModules)]
    [Description("Overrides Controllers from the SystemModule and supplies additional basic Controllers that are specific for Windows Forms applications.")]
    [Browsable(true)]
    [EditorBrowsable(EditorBrowsableState.Always)]
    [ToolboxBitmap(typeof(WinApplication), "Resources.Toolbox_Module_System_Win.ico")]
    public sealed class XpandSystemWindowsFormsModule : XpandModuleBase, IColumnCellFilterUser,IModelXmlConverter {
        public const string XpandWin = "Xpand.Win";
        public XpandSystemWindowsFormsModule() {
            RequiredModuleTypes.Add(typeof(XpandSystemModule));
            RequiredModuleTypes.Add(typeof(SystemWindowsFormsModule));
            RequiredModuleTypes.Add(typeof(ModelMapperModule));
            RequiredModuleTypes.Add(typeof(GridListEditorModule));
        }
        public override void ExtendModelInterfaces(ModelInterfaceExtenders extenders) {
            base.ExtendModelInterfaces(extenders);
            extenders.Add<IModelRootNavigationItems, IModelRootNavigationItemsAutoSelectedGroupItem>();
            extenders.Add<IModelMemberViewItem, IModelMemberViewItemFastSearch>();
            extenders.Add<IModelMemberViewItem, IModelMemberViewItemDuration>();
        }

        protected override IEnumerable<Type> GetDeclaredExportedTypes() => new List<Type>();

        public override void Setup(XafApplication application) {
            base.Setup(application);
            application.SetupComplete+=ApplicationOnSetupComplete;
            ModelBindingService.ControlBind.Where(parameter => parameter.ObjectView is ListView { Editor: LayoutViewListEditor } && parameter.Model.Parent is IModelListView)
                .Select(parameter => {
                    var layoutView = ((LayoutViewListEditor) ((ListView) parameter.ObjectView).Editor).XafLayoutView;
                    var modelLayoutViewDesign =((IModelLayoutViewDesign) parameter.ObjectView.Model.GetNode(LayoutViewMapName)).DesignLayoutView.LayoutStore;
                    var buffer = Encoding.UTF32.GetBytes(modelLayoutViewDesign);
                    using (var memoryStream = new MemoryStream(buffer)){
                        layoutView.RestoreLayoutFromStream(memoryStream);
                    }
                    return Unit.Default;
                })
                .Subscribe();
        }

        public static string AdvBandedGridViewMapName = "OptionsAdvBandedView";
        public static string BandedGridColumnMapName = "OptionsColumnAdvBandedView";
        public static string GridViewMapName = "GridViewOptions";
        public static string GridColumnMapName = "OptionsColumnGridView";
        public static string LayoutViewMapName = "OptionsLayoutView";
        public static string LayoutViewColumnMapName = "OptionsColumnLayoutView";
        
        public override void Setup(ApplicationModulesManager moduleManager) {
            base.Setup(moduleManager);
            
            moduleManager.Extend(PredefinedMap.LayoutControlGroup);
            moduleManager.Extend(PredefinedMap.AdvBandedGridView,configuration => configuration.MapName=AdvBandedGridViewMapName);
            moduleManager.Extend(PredefinedMap.BandedGridColumn,configuration => configuration.MapName=BandedGridColumnMapName);
            moduleManager.Extend(PredefinedMap.GridView,configuration => configuration.MapName=GridViewMapName);
            moduleManager.ExtendMap(PredefinedMap.GridView)
                .Subscribe(t => t.extenders.Add(t.targetInterface,typeof(IModelOptionsGridViewRules)));
            moduleManager.Extend(PredefinedMap.GridColumn,configuration => configuration.MapName=GridColumnMapName);
            
            moduleManager.Extend(PredefinedMap.XafLayoutControl);
            moduleManager.Extend(PredefinedMap.SplitContainerControl);
            moduleManager.Extend(PredefinedMap.LayoutView,configuration => configuration.MapName=LayoutViewMapName);
            moduleManager.Extend(PredefinedMap.LayoutViewColumn,configuration => configuration.MapName=LayoutViewColumnMapName);
            moduleManager.ExtendMap(PredefinedMap.LayoutView)
                .Subscribe(t => t.extenders.Add(t.targetInterface, typeof(IModelLayoutViewDesign)));
            
            var repositoryItems = EnumsNET.Enums.GetValues<PredefinedMap>().Where(map => map.IsRepositoryItem()).ToArray();
            moduleManager.Extend(repositoryItems);
            
            moduleManager.Extend(PredefinedMap.RichEditControl);

            moduleManager.ExtendMap(PredefinedMap.RichEditControl)
                .Subscribe(t => t.extenders.Add(t.targetInterface, typeof(IModelRichEditEx)));
        }
        
        private void ApplicationOnSetupComplete(object sender, EventArgs e) {
            CurrentProcessController.ApplyConfig(Application.Model.Options);
        }

        protected override IEnumerable<Type> GetDeclaredControllerTypes(){
            Type[] controllerTypes = [
                typeof(SerializeModelViewController),
                typeof(EnumRepositoryItemGridListEditorController),
                typeof(PopupWindowStateController),
                typeof(CurrentProcessController),
                typeof(RefreshObjectViewController),
                typeof(DragNDropImageController),
                typeof(RibbonFromModelController),
                typeof(ProcessDataLockingInfoController),
                typeof(DatabaseMaintenanceController),
                typeof(AutoScrollGridListEditorController),
                typeof(EditModelController),
                typeof(ApplicationMultiInstancesController),
                typeof(AutoExpandNewRowController),
                typeof(ApplicationExitController),
                typeof(ActiveDocumentViewController),
                typeof(FilterByGridViewColumnController),
                typeof(FullTextAutoFilterRowController),
                typeof(GridListEditorEventController),
                typeof(CustomSummaryCalculateController),
                typeof(GroupedRowMasterDetailViewController),
                typeof(ImmediatePostDataController),
                typeof(OpenWithController),
                typeof(SelectedItemSumController),
                typeof(EMailHighlightingController),
                typeof(UnboundColumnController),
                typeof(CursorPositionController),
                typeof(PreventDataLoadingGridViewController),
                typeof(NewObjectCollectCreatableItemTypesDataSource),
                typeof(PessimisticLockingViewController),
                typeof(SelectFirstNavigationItemController),
                typeof(FilterByPropertyPathViewController),
                typeof(FilterControlListViewController),
                typeof(FocusControlByShortcutController),
                typeof(GuessAutoFilterRowValuesFromFilterController),
                typeof(HideGridPopUpMenuViewController),
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
                typeof(RichEditToolbarController),
                typeof(LayoutViewColumnChooserController),
                typeof(RememberGridSelectionController)
            ];
            return FilterDisabledControllers(GetDeclaredControllerTypesCore(controllerTypes));
        }
        
        void IModelXmlConverter.ConvertXml(ConvertXmlParameters parameters) {
            ConvertXml(parameters);
        }
    }
}