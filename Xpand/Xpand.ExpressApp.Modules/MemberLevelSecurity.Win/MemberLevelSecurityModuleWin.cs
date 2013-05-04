using System.ComponentModel;
using System.Drawing;
using DevExpress.Utils;

namespace Xpand.ExpressApp.MemberLevelSecurity.Win {
    [Description("Allows user to protect a member for a specific record"),
     EditorBrowsable(EditorBrowsableState.Always)]
    [ToolboxBitmap(typeof(MemberLevelSecurityModuleWin))]
    [ToolboxItem(true)]
    [ToolboxTabName(XpandAssemblyInfo.TabWinModules)]
    public sealed class MemberLevelSecurityModuleWin : MemberLevelSecurityModuleBase {
        public MemberLevelSecurityModuleWin() {
            RequiredModuleTypes.Add(typeof(MemberLevelSecurityModule));
        }
    }
}