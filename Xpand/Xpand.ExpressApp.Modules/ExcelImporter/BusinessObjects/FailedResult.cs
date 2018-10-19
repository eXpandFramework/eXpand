using DevExpress.ExpressApp.DC;

namespace Xpand.ExpressApp.ExcelImporter.BusinessObjects{
    [DomainComponent]
    public class FailedResult{
        
        public string Reason{ get; set; }
        public string ImportedObject{ get; set; }
        public long Index{ get; set; }
        public string ExcelColumnValue{ get; set; }
        public string ExcelColumnName{ get; set; }

        public override string ToString(){
            return $"ImportedObject:{ImportedObject}, Index:{Index}, {nameof(ExcelColumnName)}:{ExcelColumnName}, {nameof(ExcelColumnValue)}:{ExcelColumnValue}, {nameof(Reason)}:{Reason}";
        }
    }
}