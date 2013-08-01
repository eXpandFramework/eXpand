using System.ComponentModel;
using System.Drawing;
using DevExpress.ExpressApp.Validation;
using DevExpress.Utils;
using Xpand.Persistent.Base.General;

namespace Xpand.ExpressApp.Validation.Web {
    [ToolboxItem(true)]
    [ToolboxBitmap(typeof(ValidationModule), "Resources.BO_Validation.ico")]
    [ToolboxTabName(XpandAssemblyInfo.TabAspNetModules)]
    public sealed class XpandValidationWebModule : XpandModuleBase {
        public XpandValidationWebModule() {
            RequiredModuleTypes.Add(typeof(XpandValidationModule));
        }
    }
}