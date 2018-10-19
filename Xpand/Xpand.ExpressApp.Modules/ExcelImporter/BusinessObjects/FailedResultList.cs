using System.ComponentModel;
using DevExpress.ExpressApp.DC;

namespace Xpand.ExpressApp.ExcelImporter.BusinessObjects{
    [DomainComponent]
    public class FailedResultList{
        public FailedResultList(){
            FailedResults = new BindingList<FailedResult>();
        }

        public BindingList<FailedResult> FailedResults{ get; set; }
    }
}