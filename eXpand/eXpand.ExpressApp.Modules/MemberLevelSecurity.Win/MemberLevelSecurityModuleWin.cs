using System.ComponentModel;
using DevExpress.Utils;

namespace eXpand.ExpressApp.MemberLevelSecurity.Win
{
    [Description("Allows user to protect a member for a specific record"), ToolboxTabName("eXpressApp"),
     EditorBrowsable(EditorBrowsableState.Always), Browsable(true), ToolboxItem(true)]
    public sealed partial class MemberLevelSecurityModuleWin : MemberLevelSecurityModuleBase
    {
        public MemberLevelSecurityModuleWin()
        {
            InitializeComponent();
        }

        bool? _comparerIsSet=false;
        public override bool? ComparerIsSet {
            get { return _comparerIsSet; }
            set { _comparerIsSet = value; }
        }
    }
}