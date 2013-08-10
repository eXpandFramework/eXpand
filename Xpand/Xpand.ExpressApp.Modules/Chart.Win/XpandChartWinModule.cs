using System.ComponentModel;
using System.Drawing;
using DevExpress.ExpressApp.Chart.Win;
using DevExpress.Utils;
using Xpand.Persistent.Base.General;

namespace Xpand.ExpressApp.Chart.Win {
    [ToolboxBitmap(typeof(ChartWindowsFormsModule), "Resources.Toolbox_Module_ChartListEditor_Win.ico")]
    [ToolboxItem(true)]
    [ToolboxTabName(XpandAssemblyInfo.TabWinModules)]
    public sealed class XpandChartWinModule : XpandModuleBase {
        public XpandChartWinModule() {
            RequiredModuleTypes.Add(typeof(ChartWindowsFormsModule));
        }

    }
}