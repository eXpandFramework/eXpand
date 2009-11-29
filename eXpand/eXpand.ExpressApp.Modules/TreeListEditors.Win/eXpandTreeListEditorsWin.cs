using System.ComponentModel;
using DevExpress.ExpressApp;
using DevExpress.Utils;

namespace eXpand.ExpressApp.TreeListEditors.Win
{
    [Description(
        "Includes Property Editors and Controllers to DevExpress.ExpressApp.TreeListEditors.Win Module.Enables recursive filtering"
        ), ToolboxTabName("eXpressApp"), EditorBrowsable(EditorBrowsableState.Always), Browsable(true),
     ToolboxItem(true)]
    public sealed partial class eXpandTreeListEditorsWin : ModuleBase
    {
        public eXpandTreeListEditorsWin()
        {
            InitializeComponent();
        }
    }
}