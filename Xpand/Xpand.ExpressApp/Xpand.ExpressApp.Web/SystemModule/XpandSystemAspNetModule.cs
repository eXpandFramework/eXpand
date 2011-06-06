using System.ComponentModel;
using System.Drawing;
using DevExpress.ExpressApp;
using Xpand.ExpressApp.SystemModule;

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
        //        protected override List<Type> DeclaredBusinessClasses {
        //            get {
        //                return new List<Type>();
        //            }
        //        }
    }
}
