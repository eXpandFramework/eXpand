using System;
using DevExpress.ExpressApp.Model;
using DevExpress.Xpo;
using Xpand.Persistent.Base;

namespace Xpand.ExpressApp.ExcelImporter.BusinessObjects{
    
    public class DroppedFile:XpandBaseCustomObject {
        public DroppedFile(Session session) : base(session){
        }

        ExcelImport _excelImport;

        [Association("ExcelImport-DroppedFiles")]
        public ExcelImport ExcelImport {
            get => _excelImport;
            set => SetPropertyValue(nameof(ExcelImport), ref _excelImport, value);
        }

        
        string _fileName;

        public string FileName {
            get => _fileName;
            set => SetPropertyValue(nameof(FileName), ref _fileName, value);
        }

        DateTime _dateTime;
        [ModelDefault("DisplayFormat", "{0:G}")]
        public DateTime DateTime {
            get => _dateTime;
            set => SetPropertyValue(nameof(DateTime), ref _dateTime, value);
        }

        SkipAutoImportReason _skipReason;

        public SkipAutoImportReason SkipReason {
            get => _skipReason;
            set => SetPropertyValue(nameof(SkipAutoImportReason), ref _skipReason, value);
        }
    }
    public enum SkipAutoImportReason {
        None,
        AlreadImported,
        StopAutoImportOnFailure
    }

}