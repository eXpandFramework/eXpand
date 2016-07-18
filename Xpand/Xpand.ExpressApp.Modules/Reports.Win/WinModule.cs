using System.ComponentModel;
using System.Drawing;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Reports;
using DevExpress.Utils;
using Xpand.Persistent.Base.General;
using Xpand.Persistent.Base.General.Controllers.Dashboard;

namespace Xpand.ExpressApp.Reports.Win {
    [ToolboxBitmap(typeof(XpandReportsWindowsFormsModule))]
    [ToolboxItem(true)]
    [ToolboxTabName(XpandAssemblyInfo.TabWinModules)]
    public sealed class XpandReportsWindowsFormsModule : XpandModuleBase, IDashboardInteractionUser {
        public override void Setup(XafApplication application) {
            base.Setup(application);
            LoadDxBaseImplType("DevExpress.Persistent.BaseImpl.ReportData");
        }

        protected override ModuleTypeList GetRequiredModuleTypesCore() {
            ModuleTypeList requiredModuleTypesCore = base.GetRequiredModuleTypesCore();
            requiredModuleTypesCore.Add(typeof(ReportsModule));
            requiredModuleTypesCore.Add(typeof(ReportsModule));
            requiredModuleTypesCore.Add(typeof(DevExpress.ExpressApp.Reports.Win.ReportsWindowsFormsModule));
            requiredModuleTypesCore.Add(typeof(XpandReportsModule));
            return requiredModuleTypesCore;
        }
    }
}