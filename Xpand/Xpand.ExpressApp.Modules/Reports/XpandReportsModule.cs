using System.ComponentModel;
using System.Drawing;
using DevExpress.ExpressApp;
using DevExpress.Utils;
using Xpand.Persistent.Base.General;

namespace Xpand.ExpressApp.Reports {
    [ToolboxBitmap(typeof(XpandReportsModule))]
    [ToolboxItem(true)]
    [ToolboxTabName(XpandAssemblyInfo.TabWinWebModules)]
    public sealed class XpandReportsModule : XpandModuleBase {
        public override void Setup(XafApplication application){
            base.Setup(application);
            LoadDxBaseImplType("DevExpress.Persistent.BaseImpl.ReportData");
        }

        protected override ModuleTypeList GetRequiredModuleTypesCore() {
            ModuleTypeList requiredModuleTypesCore = base.GetRequiredModuleTypesCore();
            requiredModuleTypesCore.Add(typeof(DevExpress.ExpressApp.Reports.ReportsModule));
            return requiredModuleTypesCore;
        }
    }
}