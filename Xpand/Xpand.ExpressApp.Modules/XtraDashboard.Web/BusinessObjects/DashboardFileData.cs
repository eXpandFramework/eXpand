using DevExpress.Persistent.Base;
using DevExpress.Xpo;
using Xpand.Persistent.Base;

namespace Xpand.ExpressApp.XtraDashboard.Web.BusinessObjects{
    [NonPersistent]
    public class DashboardFileData : XpandBaseCustomObject{
        private XpandFileData _fileData;

        public DashboardFileData(Session session)
            : base(session){
        }

        [FileTypeFilter("Dashboard xml", 1, "*.xml")]
        public XpandFileData FileData{
            get { return _fileData; }
            set { SetPropertyValue("FileData", ref _fileData, value); }
        }
    }
}