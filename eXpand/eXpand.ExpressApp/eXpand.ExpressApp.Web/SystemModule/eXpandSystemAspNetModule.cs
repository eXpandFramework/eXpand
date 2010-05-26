using System.ComponentModel;
using System.Drawing;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Editors;
using eXpand.ExpressApp.SystemModule;
using eXpand.ExpressApp.Web.Editors;

namespace eXpand.ExpressApp.Web.SystemModule {
    [ToolboxItem(true)]
    [DevExpress.Utils.ToolboxTabName(XafAssemblyInfo.DXTabXafModules)]
    [Description("Overrides Controllers from the SystemModule and supplies additional basic Controllers that are specific for ASP.NET applications.")]
    [Browsable(true)]
    [EditorBrowsable(EditorBrowsableState.Always)]
    [ToolboxBitmap(typeof(WebComponent), "Resources.WebSystemModule.ico")]
    [ToolboxItemFilter("Xaf.Platform.Web")]
    public sealed class eXpandSystemAspNetModule : ModuleBase {
        public eXpandSystemAspNetModule() {
        }

        protected override void RegisterEditorDescriptors(System.Collections.Generic.List<DevExpress.ExpressApp.Editors.EditorDescriptor> editorDescriptors)
        {
            base.RegisterEditorDescriptors(editorDescriptors);
            editorDescriptors.Add(new DetailViewItemDescriptor(new DetailViewItemRegistration(typeof(IModelActionButtonDetailItem), typeof(ActionButtonDetailItem), true)));
        }
    }
}
