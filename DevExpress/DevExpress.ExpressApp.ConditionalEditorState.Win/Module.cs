using DevExpress.ExpressApp;
using System.ComponentModel;
using System.Drawing;

namespace DevExpress.ExpressApp.ConditionalEditorState.Win {
    [Browsable(true)]
    [EditorBrowsable(EditorBrowsableState.Always)]
    [ToolboxItem(true)]
    [DevExpress.Utils.ToolboxTabName(XafAssemblyInfo.DXTabXafModules)]
    [ToolboxBitmap(typeof(ConditionalEditorStateWindowsFormsModule), "Resources.ConditionalEditorStateWindowsFormsModule.ico")]
    [Description("Provides the capability to customize the view's editors against against business rules in Windows Forms XAF applications.")]
    public sealed partial class ConditionalEditorStateWindowsFormsModule : ModuleBase {
        public ConditionalEditorStateWindowsFormsModule() {
            InitializeComponent();
        }
    }
}
