using System.ComponentModel;
using System.Drawing;
using DevExpress.Utils;
using EditorBrowsableState = System.ComponentModel.EditorBrowsableState;

namespace Xpand.ExpressApp.ViewVariants {
    [Description(
        "Includes Property Editors and Controllers to DevExpress.ExpressApp.ViewVariants Module. Enables View Cloning"),
     ToolboxTabName("eXpressApp"), EditorBrowsable(EditorBrowsableState.Always)]
    [ToolboxBitmap(typeof(XpandViewVariantsModule))]
    [ToolboxItem(true)]
    public sealed partial class XpandViewVariantsModule : XpandModuleBase {
        public const string XpandViewVariants = "eXpand.ViewVariants";
        public XpandViewVariantsModule() {
            InitializeComponent();
        }

    }
}