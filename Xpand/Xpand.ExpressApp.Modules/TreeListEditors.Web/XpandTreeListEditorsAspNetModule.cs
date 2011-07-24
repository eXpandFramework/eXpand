using System.ComponentModel;
using System.Drawing;
using DevExpress.Utils;

namespace Xpand.ExpressApp.TreeListEditors.Web {
    [Description, ToolboxTabName("eXpressApp"), EditorBrowsable(EditorBrowsableState.Always)]
    [ToolboxBitmap(typeof(XpandTreeListEditorsAspNetModule))]
    [ToolboxItem(true)]
    public sealed partial class XpandTreeListEditorsAspNetModule : XpandModuleBase {
        public XpandTreeListEditorsAspNetModule() {
            InitializeComponent();
        }


    }
}