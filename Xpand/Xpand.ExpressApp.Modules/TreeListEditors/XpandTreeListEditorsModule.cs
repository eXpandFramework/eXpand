using System.ComponentModel;
using DevExpress.Utils;

namespace Xpand.ExpressApp.TreeListEditors {
    [Description, ToolboxTabName("eXpressApp"), EditorBrowsable(EditorBrowsableState.Always), Browsable(true),
     ToolboxItem(true)]
    public sealed partial class XpandTreeListEditorsModule : XpandModuleBase {
        public XpandTreeListEditorsModule() {
            InitializeComponent();
        }


    }
}