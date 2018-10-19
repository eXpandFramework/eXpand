using System;
using System.Data;

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
            int failedRecordsCount) : base(excelImportKey,totalRecordsCount,failedRecordsCount) {
            Exception = exception;
        }
    }

    public class ImportProgressComplete:ImportProgress {
        public ImportProgressComplete(Guid excelImportKey, int totalRecordsCount, int failedRecordsCount) : base(excelImportKey) {
            TotalRecordsCount = totalRecordsCount;
            FailedRecordsCount = failedRecordsCount;
        }

        public int TotalRecordsCount { get; }
        public int FailedRecordsCount { get; }
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

        public int Total { get; set; }
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