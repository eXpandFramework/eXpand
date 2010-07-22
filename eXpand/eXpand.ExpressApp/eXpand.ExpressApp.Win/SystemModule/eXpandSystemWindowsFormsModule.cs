using System.ComponentModel;
using System.Drawing;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Win;
using DevExpress.Utils;
using eXpand.ExpressApp.SystemModule;
using eXpand.ExpressApp.Win.ListEditors;

namespace eXpand.ExpressApp.Win.SystemModule {
    [ToolboxItem(true)]
    [ToolboxTabName(XafAssemblyInfo.DXTabXafModules)]
    [Description("Overrides Controllers from the SystemModule and supplies additional basic Controllers that are specific for Windows Forms applications.")]
    [Browsable(true)]
    [EditorBrowsable(EditorBrowsableState.Always)]
    [ToolboxBitmap(typeof (WinApplication), "Resources.WinSystemModule.ico")]
    [ToolboxItemFilter("Xaf.Platform.Win")]
    public sealed class eXpandSystemWindowsFormsModule : ModuleBase {
        public eXpandSystemWindowsFormsModule() {
            RequiredModuleTypes.Add(typeof (eXpandSystemModule));
        }

        protected override void RegisterEditorDescriptors(System.Collections.Generic.List<EditorDescriptor> editorDescriptors)
        {
            base.RegisterEditorDescriptors(editorDescriptors);
            editorDescriptors.Add(new ListEditorDescriptor(new EditorTypeRegistration(EditorAliases.GridListEditor, typeof(object), typeof(GridListEditor), true)));
            
        }
    }
}