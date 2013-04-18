using System.ComponentModel;
using System.Drawing;
using DevExpress.ExpressApp.Validation;
using DevExpress.Utils;

namespace Xpand.ExpressApp.Validation.Win {
    [ToolboxBitmap(typeof(ValidationModule), "Resources.BO_Validation.ico")]
    [ToolboxTabName(XpandAssemblyInfo.TabWinModules)]
    [ToolboxItem(true)]
    public sealed class XpandValidationWinModule : XpandModuleBase {
        public XpandValidationWinModule() {
            RequiredModuleTypes.Add(typeof(XpandValidationModule));
            RequiredModuleTypes.Add(typeof(DevExpress.ExpressApp.Validation.Win.ValidationWindowsFormsModule));
        }
    }
}