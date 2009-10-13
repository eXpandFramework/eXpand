using System.ComponentModel;
using DevExpress.ExpressApp;
using DevExpress.Utils;

namespace eXpand.ExpressApp.ViewVariantsModule.Win
{
    [Description(
        "Allows View managments"),
     ToolboxTabName("eXpressApp"), EditorBrowsable(EditorBrowsableState.Always), Browsable(true), ToolboxItem(true)]
    public sealed partial class eXpandViewVariantsWin : ModuleBase
    {
        public eXpandViewVariantsWin()
        {
            InitializeComponent();
        }
    }
}