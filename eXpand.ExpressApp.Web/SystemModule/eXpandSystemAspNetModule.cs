using System.ComponentModel;
using System.Drawing;
using DevExpress.ExpressApp;

namespace eXpand.ExpressApp.Web.SystemModule {
    [ToolboxItem(true)]
    [DevExpress.Utils.ToolboxTabName(XafAssemblyInfo.DXTabXafModules)]
    [Description("Overrides Controllers from the SystemModule and supplies additional basic Controllers that are specific for ASP.NET XAFPoint applications.")]
    [Browsable(true)]
    [EditorBrowsable(EditorBrowsableState.Always)]
    [ToolboxBitmap(typeof(WebApplication), "Resources.WebSystemModule.ico")]
    [ToolboxItemFilter("Xaf.Platform.Web")]
    public sealed partial class eXpandSystemAspNetModule : ModuleBase {
        public eXpandSystemAspNetModule() {
            InitializeComponent();
        }
    }
}
