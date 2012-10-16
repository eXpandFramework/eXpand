using System.ComponentModel;
using System.Drawing;
using DevExpress.ExpressApp.Chart.Win;

namespace Xpand.ExpressApp.Chart.Win {
    [ToolboxBitmap(typeof(XpandChartWinModule))]
    [ToolboxItem(true)]
    public sealed class XpandChartWinModule : XpandModuleBase {
        public XpandChartWinModule() {
            RequiredModuleTypes.Add(typeof(ChartWindowsFormsModule));
        }

    }
}