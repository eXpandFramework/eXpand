
using System.ComponentModel;
using Xpand.Persistent.Base.MemberLevelSecurity;

namespace Xpand.ExpressApp.MemberLevelSecurity {
    [ToolboxItem(false)]
    public sealed partial class MemberLevelSecurityModule : XpandModuleBase, IMemberLevelSecurityModule {
        public MemberLevelSecurityModule() {
            InitializeComponent();
        }


    }
}