using System.ComponentModel;
using DevExpress.Utils;

namespace Xpand.ExpressApp.TreeListEditors.Web {
    [Description, ToolboxTabName("eXpressApp"), EditorBrowsable(EditorBrowsableState.Always), Browsable(true),
     ToolboxItem(true)]
    public sealed partial class XpandTreeListEditorsAspNetModule : XpandModuleBase {
        public XpandTreeListEditorsAspNetModule() {
            InitializeComponent();
        }


    }
}