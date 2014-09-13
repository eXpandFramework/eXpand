using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using DevExpress.ExpressApp.CloneObject;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Validation;
using DevExpress.ExpressApp.Web;
using DevExpress.Utils;
using Xpand.ExpressApp.SystemModule;
using Xpand.ExpressApp.Web.ListEditors.TwoDimensionListEditor;
using Xpand.ExpressApp.Web.Model;
using Xpand.ExpressApp.Web.PropertyEditors;
using Xpand.Persistent.Base.General;
using Xpand.Persistent.Base.General.Model.Options;
using EditorAliases = Xpand.Persistent.Base.General.EditorAliases;

namespace Xpand.ExpressApp.Web.SystemModule {
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

        protected override IEnumerable<Type> GetDeclaredExportedTypes() {
            return new List<Type>();
        }

        protected override void RegisterEditorDescriptors(List<EditorDescriptor> editorDescriptors) {
            base.RegisterEditorDescriptors(editorDescriptors);
            editorDescriptors.Add(new PropertyEditorDescriptor(new EditorTypeRegistration(EditorAliases.TimePropertyEditor, typeof(DateTime), typeof(ASPxTimePropertyEditor), false)));
        }

        public override void ExtendModelInterfaces(ModelInterfaceExtenders extenders) {
            base.ExtendModelInterfaces(extenders);
            extenders.Add<IModelOptions, IModelOptionsQueryStringParameter>();
            extenders.Add<IModelMemberViewItem, IModelMemberViewItemRelativeDate>();
            extenders.Add<IModelListView, IModelListViewTwoDimensionListEditor>();
            extenders.Add<IModelColumnSummaryItem, IModelColumnSummaryItemTwoDimensionListEditor>();
        }
    }
}
