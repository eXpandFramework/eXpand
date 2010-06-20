using System.ComponentModel;
using DevExpress.ExpressApp.Model.Core;
using DevExpress.Utils;
using eXpand.ExpressApp.MemberLevelSecurity.Win.NodeUpdaters;

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

        protected override bool? ComparerIsSet {
            get { return _comparerIsSet; }
            set { _comparerIsSet = value; }
        }

        public override void AddGeneratorUpdaters(ModelNodesGeneratorUpdaters updaters) {
            base.AddGeneratorUpdaters(updaters);
            updaters.Add(new MemberLevelSecurityStringPropertyEditorUpdater());            
        }
    }
}