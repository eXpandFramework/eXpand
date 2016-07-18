using DevExpress.ExpressApp;
using Xpand.Persistent.Base.General;
using Xpand.Persistent.Base.General.Controllers.Dashboard;


namespace Xpand.ExpressApp.ReportsV2.Win {
    public sealed partial class ReportsV2WinModule : XpandModuleBase , IDashboardInteractionUser {
        public ReportsV2WinModule() {
            InitializeComponent();
        }

        public override void Setup(XafApplication application) {
            base.Setup(application);
            LoadDxBaseImplType("DevExpress.Persistent.BaseImpl.ReportDataV2");
        }

    }
}
