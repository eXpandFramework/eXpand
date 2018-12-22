using DevExpress.Persistent.Base;
using PropertyChanged;
using Xpand.Persistent.Base;

namespace Xpand.ExpressApp.XtraDashboard.Web.BusinessObjects{
    [AddINotifyPropertyChangedInterface]
    public class DashboardFileData {

        [FileTypeFilter("Dashboard xml", 1, "*.xml")]
        public XpandFileData FileData{get; set; }
    }
}