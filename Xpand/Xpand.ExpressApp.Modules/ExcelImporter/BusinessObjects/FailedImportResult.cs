using System.Diagnostics.CodeAnalysis;
using DevExpress.Xpo;
using Xpand.Extensions.XAF.Attributes;
using Xpand.Persistent.Base;

namespace Xpand.ExpressApp.ExcelImporter.BusinessObjects{
    [SuppressMessage("Design", "XAF0023:Do not implement IObjectSpaceLink in the XPO types")]
    public class FailedImportResult:XpandBaseCustomObject{
        public FailedImportResult(Session session) : base(session){
        }

        [Size(SizeAttribute.Unlimited)]
        public string Reason{ get; set; }

        [Size(SizeAttribute.Unlimited)]
        public string ImportedObject{ get; set; }

        public long Index{ get; set; }

        [Size(SizeAttribute.Unlimited)]
        public string ExcelColumnValue{ get; set; }

        public string ExcelColumnName{ get; set; }

        ExcelImport _excelImport;

        [InvisibleInAllViews]
        [Association("ExcelImport-FailedImportResults")]
        public ExcelImport ExcelImport {
            get => _excelImport;
            set => SetPropertyValue(nameof(ExcelImport), ref _excelImport, value);
        }

        public override string ToString(){
            return $"ImportedObject:{ImportedObject}, Index:{Index}, {nameof(ExcelColumnName)}:{ExcelColumnName}, {nameof(ExcelColumnValue)}:{ExcelColumnValue}, {nameof(Reason)}:{Reason}";
        }
    }
}