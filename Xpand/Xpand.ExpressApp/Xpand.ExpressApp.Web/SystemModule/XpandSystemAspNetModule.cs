using System.ComponentModel;
using System.Drawing;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Editors;
using Xpand.ExpressApp.SystemModule;
using Xpand.ExpressApp.Web.ListEditors;

namespace Xpand.ExpressApp.Web.SystemModule {
    [ToolboxItem(true)]
    [DevExpress.Utils.ToolboxTabName(XafAssemblyInfo.DXTabXafModules)]
    [Description("Overrides Controllers from the SystemModule and supplies additional basic Controllers that are specific for ASP.NET applications.")]
    [Browsable(true)]
    [EditorBrowsable(EditorBrowsableState.Always)]
    [ToolboxBitmap(typeof(XpandWebApplication), "Resources.WebSystemModule.ico")]
    [ToolboxItemFilter("Xaf.Platform.Web")]
    public sealed class XpandSystemAspNetModule : XpandModuleBase {
        public XpandSystemAspNetModule() {
            RequiredModuleTypes.Add(typeof(XpandSystemModule));
        }

        protected override void RegisterEditorDescriptors(System.Collections.Generic.List<EditorDescriptor> editorDescriptors)
        {
            base.RegisterEditorDescriptors(editorDescriptors);
            editorDescriptors.Add(new ListEditorDescriptor(new EditorTypeRegistration(EditorAliases.GridListEditor, typeof(object), typeof(XpandASPxGridListEditor), true)));
        }
    }
}
