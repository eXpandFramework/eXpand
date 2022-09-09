using System;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using DevExpress.ExpressApp.Model;
using DevExpress.Xpo;
using Xpand.Persistent.Base;

namespace Xpand.ExpressApp.ExcelImporter.BusinessObjects{
    [SuppressMessage("Design", "XAF0023:Do not implement IObjectSpaceLink in the XPO types")]
    public class AutoImportedFile:XpandBaseCustomObject{
        public AutoImportedFile(Session session) : base(session){
        }

        ExcelImport _excelImport;

        [Association("ExcelImport-AutoImportedFiles")]
        public ExcelImport ExcelImport{
            get => _excelImport;
            set => SetPropertyValue(nameof(ExcelImport), ref _excelImport, value);
        }

        bool? _succeded;

        public bool? Succeded{
            get => _succeded;
            set => SetPropertyValue(nameof(Succeded), ref _succeded, value);
        }
        string _fileName;
        [Size(255)]
        public string FileName{
            get => _fileName;
            set => SetPropertyValue(nameof(FileName), ref _fileName, value);
        }

        DateTime _startTime;
        [ModelDefault("DisplayFormat", "{0:G}")]
        public DateTime StartTime{
            get => _startTime;
            set => SetPropertyValue(nameof(StartTime), ref _startTime, value);
        }

        DateTime _endTime;
        [ModelDefault("DisplayFormat", "{0:G}")]
        public DateTime EndTime{
            get => _endTime;
            set => SetPropertyValue(nameof(EndTime), ref _endTime, value);
        }

        DateTime _creationTime;
        [Browsable(false)]
        public DateTime CreationTime{
            get => _creationTime;
            set => SetPropertyValue(nameof(CreationTime), ref _creationTime, value);
        }
    }
}