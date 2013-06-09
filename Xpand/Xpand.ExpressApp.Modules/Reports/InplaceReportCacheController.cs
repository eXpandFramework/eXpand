using DevExpress.ExpressApp;

namespace Xpand.ExpressApp.Reports {
    public class InplaceReportCacheController : DevExpress.ExpressApp.Reports.InplaceReportCacheController {
        protected override IObjectSpace CreateObjectSpace() {
            return Application.CreateObjectSpace(ReportDataType);
        }
    }
}
