using System;
using System.ComponentModel;
using System.Drawing;
using DevExpress.Utils;

namespace Xpand.ExpressApp.MemberLevelSecurity.Win {
    [Description("Allows user to protect a member for a specific record"), ToolboxTabName("eXpressApp"),
     EditorBrowsable(EditorBrowsableState.Always)]
    [ToolboxBitmap(typeof(MemberLevelSecurityModuleWin))]
    [ToolboxItem(true)]
    public sealed class MemberLevelSecurityModuleWin : MemberLevelSecurityModuleBase {
        public MemberLevelSecurityModuleWin() {
            RequiredModuleTypes.Add(typeof(MemberLevelSecurityModule));
        }
    }
}