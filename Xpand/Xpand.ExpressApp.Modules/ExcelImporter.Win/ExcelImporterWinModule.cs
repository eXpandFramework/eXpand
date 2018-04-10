using System.ComponentModel;
using System.Drawing;
using DevExpress.Utils;
using Xpand.Persistent.Base.General;

namespace Xpand.ExpressApp.ExcelImporter.Win {
    [ToolboxBitmap(typeof(ExcelImporterWinModule))]
     [ToolboxItem(true)]
     [ToolboxTabName(XpandAssemblyInfo.TabWinModules)]
    public sealed partial class ExcelImporterWinModule : XpandModuleBase {
        public ExcelImporterWinModule() {
            InitializeComponent();
        }


    }
}
