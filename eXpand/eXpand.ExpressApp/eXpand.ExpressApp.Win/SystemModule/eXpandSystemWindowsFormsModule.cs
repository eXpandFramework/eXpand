using System.ComponentModel;
using System.Drawing;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Win;
using DevExpress.Utils;
using eXpand.ExpressApp.Win.ListEditors;

namespace eXpand.ExpressApp.Win.SystemModule {
    [ToolboxItem(true)]
    [ToolboxTabName(XafAssemblyInfo.DXTabXafModules)]
    [Description(
        "Overrides Controllers from the SystemModule and supplies additional basic Controllers that are specific for Windows Forms applications."
        )]
    [Browsable(true)]
    [EditorBrowsable(EditorBrowsableState.Always)]
    [ToolboxBitmap(typeof (WinApplication), "Resources.WinSystemModule.ico")]
    [ToolboxItemFilter("Xaf.Platform.Win")]
    public sealed class eXpandSystemWindowsFormsModule : ModuleBase {
        public override void UpdateModel(IModelApplication applicationModel) {
            base.UpdateModel(applicationModel);
            applicationModel.Views.DefaultListEditor = typeof (GridListEditor);
        }
    }
}