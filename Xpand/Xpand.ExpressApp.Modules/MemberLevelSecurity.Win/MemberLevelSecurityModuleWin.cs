using System.ComponentModel;
using DevExpress.Utils;

namespace Xpand.ExpressApp.MemberLevelSecurity.Win
{
    [Description("Allows user to protect a member for a specific record"), ToolboxTabName("eXpressApp"),
     EditorBrowsable(EditorBrowsableState.Always), Browsable(true), ToolboxItem(true)]
    public sealed partial class MemberLevelSecurityModuleWin : MemberLevelSecurityModuleBase
    {
        public MemberLevelSecurityModuleWin()
        {
            InitializeComponent();
        }

        
    }
}