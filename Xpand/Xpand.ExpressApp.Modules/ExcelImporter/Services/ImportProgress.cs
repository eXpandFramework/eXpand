using System;
using System.ComponentModel;
using System.Data;
using DevExpress.Data.Filtering;
using DevExpress.ExpressApp;
using Xpand.ExpressApp.ExcelImporter.BusinessObjects;

namespace Xpand.ExpressApp.ExcelImporter.Services{
    public abstract class ImportProgress {
        protected ImportProgress(Guid excelImportKey) {
            ExcelImportKey = excelImportKey;
        }

        public Guid ExcelImportKey { get;  }
    }

    public class ImportProgressException:ImportProgressComplete {
        public Exception Exception{ get; }

        public ImportProgressException(Guid excelImportKey, Exception exception, int totalRecordsCount,
            BindingList<FailedResult> failedRecordsCount) : base(excelImportKey,totalRecordsCount,failedRecordsCount) {
            Exception = exception;
        }
    }
    public class RequestImportTargetObject : ImportProgress {
        public (Type objectType, CriteriaOperator criteria) Args{ get; }
        public object TargetObject { get; set; }
        public IObjectSpace ObjectSpace { get; set; }

        public RequestImportTargetObject((Type objectType, CriteriaOperator criteria) args,Guid key) : base(key) {
            Args = args;
        }
    }

    public class ImportProgressComplete:ImportProgress {
        public ImportProgressComplete(Guid excelImportKey, int totalRecordsCount, BindingList<FailedResult> failedResults) : base(excelImportKey) {
            TotalRecordsCount = totalRecordsCount;
            FailedResults = failedResults;
        }

        public int TotalRecordsCount { get; }
        public BindingList<FailedResult> FailedResults { get; }
    }

    public abstract class ImportProgressPercentage:ImportProgress {
        protected ImportProgressPercentage(Guid excelImportKey,int percentage) : base(excelImportKey) {
            Percentage = percentage;
        }

        public int Percentage { get;  }
    }

    public class ImportProgressStart:ImportProgress {
        public ImportProgressStart(Guid excelImportKey) : base(excelImportKey){
        }

        public DataTable DataTable { get; set; }
    }

    public class ImportObjectProgress : ImportProgressPercentage {
        public ImportObjectProgress(Guid excelImportKey, int percentage) : base(excelImportKey, percentage){
        }

        public object ObjectToImport { get; set; }
    }

    public class ImportDataRowProgress:ImportProgressPercentage {
        public ImportDataRowProgress(Guid excelImportKey,int percentage) : base(excelImportKey,percentage){
        }

        public DataRow DataRow { get; set; }
        
    }
}