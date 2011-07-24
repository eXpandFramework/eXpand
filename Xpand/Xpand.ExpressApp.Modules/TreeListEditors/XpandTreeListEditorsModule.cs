using System.ComponentModel;
using DevExpress.Utils;

namespace Xpand.ExpressApp.TreeListEditors {
    [Description, ToolboxTabName("eXpressApp"), EditorBrowsable(EditorBrowsableState.Always), Browsable(true),
     ToolboxItem(false)]
    public sealed partial class XpandTreeListEditorsModule : XpandModuleBase {
        public XpandTreeListEditorsModule() {
            InitializeComponent();
        }


    }
}