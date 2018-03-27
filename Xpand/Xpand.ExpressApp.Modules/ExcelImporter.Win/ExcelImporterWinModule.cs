using DevExpress.ExpressApp;
using System.ComponentModel;
using System.Drawing;
using DevExpress.Utils;

namespace Xpand.ExpressApp.ExcelImporter.Win {
    [ToolboxBitmap(typeof(ExcelImporterWinModule))]
     [ToolboxItem(true)]
     [ToolboxTabName(XpandAssemblyInfo.TabWinModules)]
    public sealed partial class ExcelImporterWinModule : ModuleBase {
        public ExcelImporterWinModule() {
            InitializeComponent();
        }


    }
}
