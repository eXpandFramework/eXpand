using System.ComponentModel;
using DevExpress.Utils;
using EditorBrowsableState=System.ComponentModel.EditorBrowsableState;

namespace eXpand.ExpressApp.ViewVariants
{
    [Description(
        "Includes Property Editors and Controllers to DevExpress.ExpressApp.ViewVariants Module. Enables View Cloning"),
     ToolboxTabName("eXpressApp"), EditorBrowsable(EditorBrowsableState.Always), Browsable(true), ToolboxItem(true)]
    public sealed partial class XpandViewVariantsModule : XpandModuleBase
    {
        public XpandViewVariantsModule()
        {
            InitializeComponent();
        }
        
    }
}