using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.CloneObject;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Validation;
using DevExpress.ExpressApp.Web;
using DevExpress.Utils;
using Xpand.ExpressApp.SystemModule;
using Xpand.ExpressApp.Web.FriendlyUrl;
using Xpand.ExpressApp.Web.ListEditors.EditableTabEnabledListEditor;
using Xpand.ExpressApp.Web.ListEditors.Model;
using Xpand.ExpressApp.Web.ListEditors.TwoDimensionListEditor;
using Xpand.ExpressApp.Web.Model;
using Xpand.ExpressApp.Web.PropertyEditors;
using Xpand.ExpressApp.Web.SystemModule.MasterDetail;
using Xpand.ExpressApp.Web.SystemModule.ModelAdapters;
using Xpand.ExpressApp.Web.SystemModule.WebShortcuts;
using Xpand.Persistent.Base.General;
using Xpand.Persistent.Base.General.Model.Options;
using Xpand.Persistent.Base.ModelAdapter;
using Xpand.Persistent.Base.TreeNode;
using EditorAliases = Xpand.Persistent.Base.General.EditorAliases;

namespace Xpand.ExpressApp.Web.SystemModule {
    public interface IModelOptionsWebApplicationStyleManager{
        IModelWebApplicationStyleManager WebApplicationStyleManager{ get; }
    }

    public interface IModelWebApplicationStyleManager:IModelNode{
        [DefaultValue(true)]
        bool EnableUpperCase{ get; set; }
        [DefaultValue(true)]
        bool EnableNavigationGroupsUpperCase{ get; set; }
        [DefaultValue(true)]
        bool EnableGridColumnsUpperCase{ get; set; }
        [DefaultValue(true)]
        bool EnableGroupUpperCase{ get; set; }
    }

    [ToolboxItem(true)]
    [ToolboxTabName(XpandAssemblyInfo.TabAspNetModules)]
    [Description("Overrides Controllers from the SystemModule and supplies additional basic Controllers that are specific for ASP.NET applications.")]
    [Browsable(true)]
    [EditorBrowsable(EditorBrowsableState.Always)]
    [ToolboxBitmap(typeof(WebApplication), "Resources.Toolbox_Module_System_Web.ico")]
    public sealed class XpandSystemAspNetModule : XpandModuleBase, IGridOptionsUser {
        public const string XpandWeb = "eXpand.Web";
        public XpandSystemAspNetModule() {
            RequiredModuleTypes.Add(typeof(XpandSystemModule));
            RequiredModuleTypes.Add(typeof(ValidationModule));
            RequiredModuleTypes.Add(typeof(DevExpress.ExpressApp.Web.SystemModule.SystemAspNetModule));
            RequiredModuleTypes.Add(typeof(CloneObjectModule));
        }

        protected override IEnumerable<Type> GetDeclaredExportedTypes(){
            return base.GetDeclaredExportedTypes().Concat(new[] {typeof(ColumnChooserList) });
        }

        protected override IEnumerable<Type> GetDeclaredControllerTypes(){
            Type[] controllerTypes ={
                typeof(OpenDetailViewInNewTabController),
                typeof(FindPopupController),
                typeof(RefreshObjectViewController),
                typeof(ASPxSpinEditModelAdapter),
                typeof(ASPxHyperLinkControlModelAdapter),
                typeof(ASPxDateEditModelAdapter),
                typeof(WebProcessDataLockingInfoController),
                typeof(ColumnChooserGridViewController),
                typeof(LayoutStyleController),
                typeof(RegisterScriptsController),
                typeof(NullTextController),
                typeof(PreviewRowDetailViewController),
                typeof(UnboundColumnController),
                typeof(PessimisticLockingViewController),
                typeof(WebToolTipsController),
                typeof(FilterByPropertyPathViewController),
                typeof(HideToolBarController),
                typeof(HighlightFocusedLayoutItemDetailViewController),
                typeof(WebShortcutsController),
                typeof(NestedListViewAutoCommitController),
                typeof(MasterDetailKeyFieldController),
                typeof(RegisterCallbackPanelScriptsController),
                typeof(DisableProcessCurrentObjectController),
                typeof(UpdateVisibilityController),
                typeof(GridViewModelAdapterController),
                typeof(TwoDimensionEditorViewItemController),
                typeof(ViewModeAppliedAtTwoDimensionListEditorController),
                typeof(EditableTabEnabledListEditorController),
                typeof(ASPxSearchDropDownEditControlModelAdapter),
                typeof(ASPxLookupFindEditControlModelAdapter),
                typeof(ASPxLookupDropDownEditControlModelAdapter),
                typeof(FriendlyUrlModelExtenderController),
                typeof(ImmediatePostDataRestoreFocusController),
                typeof(UpperCaseController)
            };
            return FilterDisabledControllers(GetDeclaredControllerTypesCore(controllerTypes));
        }

        public override void Setup(XafApplication application){
            base.Setup(application);
            application.SetupComplete+=ApplicationOnSetupComplete;
            application.CreateCustomLogonWindowControllers+=ApplicationOnCreateCustomLogonWindowControllers;
        }

        private void ApplicationOnSetupComplete(object sender, EventArgs eventArgs){
            var applicationStyleManager = ((IModelOptionsWebApplicationStyleManager) Application.Model.Options).WebApplicationStyleManager;
            if (WebApplicationStyleManager.IsNewStyle&&!InterfaceBuilder.IsDBUpdater){
                WebApplicationStyleManager.EnableUpperCase = applicationStyleManager.EnableUpperCase;
                WebApplicationStyleManager.EnableGridColumnsUpperCase = applicationStyleManager.EnableGridColumnsUpperCase;
                WebApplicationStyleManager.EnableGroupUpperCase = applicationStyleManager.EnableGroupUpperCase;
                WebApplicationStyleManager.EnableNavigationGroupsUpperCase =applicationStyleManager.EnableNavigationGroupsUpperCase;
            }
        }

        private void ApplicationOnCreateCustomLogonWindowControllers(object sender, CreateCustomLogonWindowControllersEventArgs e){
            e.Controllers.Add(Application.CreateController<UpperCaseController>());
        }

        protected override void RegisterEditorDescriptors(EditorDescriptorsFactory editorDescriptorsFactory) {
            base.RegisterEditorDescriptors(editorDescriptorsFactory);
            editorDescriptorsFactory.List.Add(new PropertyEditorDescriptor(new EditorTypeRegistration(EditorAliases.TimePropertyEditor, typeof(DateTime), typeof(ASPxTimePropertyEditor), false)));
        }

        public override void ExtendModelInterfaces(ModelInterfaceExtenders extenders) {
            base.ExtendModelInterfaces(extenders);
            extenders.Add<IModelOptions, IModelOptionsQueryStringParameter>();
            extenders.Add<IModelOptions, IModelOptionsWebApplicationStyleManager>();
            extenders.Add<IModelMemberViewItem, IModelMemberViewItemRelativeDate>();
            extenders.Add<IModelListView, IModelListViewTwoDimensionListEditor>();
            extenders.Add<IModelColumnSummaryItem, IModelColumnSummaryItemTwoDimensionListEditor>();
        }
    }
}
