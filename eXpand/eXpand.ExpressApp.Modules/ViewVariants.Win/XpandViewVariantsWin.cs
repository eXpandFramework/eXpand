using System.ComponentModel;
using DevExpress.Utils;

namespace eXpand.ExpressApp.ViewVariants.Win {
    [Description(
        "Allows View managments"),
     ToolboxTabName("eXpressApp"), EditorBrowsable(EditorBrowsableState.Always), Browsable(true), ToolboxItem(true)]
    public sealed partial class XpandViewVariantsWin : XpandModuleBase
    {
        public XpandViewVariantsWin()
        {
            InitializeComponent();
        }
    }
}