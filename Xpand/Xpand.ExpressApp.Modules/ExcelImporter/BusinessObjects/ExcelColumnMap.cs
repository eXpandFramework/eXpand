using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Editors;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.Validation;
using DevExpress.Xpo;
using Xpand.ExpressApp.ExcelImporter.Controllers;
using Xpand.Persistent.Base;

namespace Xpand.ExpressApp.ExcelImporter.BusinessObjects{
    [DefaultListViewOptions(true,NewItemRowPosition.None)]
    public class ExcelColumnMap : XpandBaseCustomObject{
        private ExcelImport _excelImport;

        public ExcelColumnMap(Session session) : base(session){
        }

        string _excelColumnName;
        [EditorAlias(EditorAliases.StringPropertyEditor)]
        [RuleRequiredField(TargetContextIDs = ExcelImportDetailViewController.ImportExcelActionName)]
        public string ExcelColumnName{
            get => _excelColumnName;
            set => SetPropertyValue(nameof(ExcelColumnName), ref _excelColumnName, value);
        }

        string _propertyName;

        [RuleRequiredField(TargetContextIDs = ExcelImportDetailViewController.ImportExcelActionName)]
        [EditorAlias(EditorAliases.StringPropertyEditor)]
        public string PropertyName{
            get => _propertyName;
            set => SetPropertyValue(nameof(PropertyName), ref _propertyName, value);
        }        

        PersistentTypesImportStrategy _importStrategy ;
        
        public PersistentTypesImportStrategy ImportStrategy {
            get => _importStrategy;
            set => SetPropertyValue(nameof(ImportStrategy), ref _importStrategy, value);
        }

        [Association("ExcelImport-ExcelColumnMaps")]
        public ExcelImport ExcelImport{
            get => _excelImport;
            set => SetPropertyValue(nameof(ExcelImport), ref _excelImport, value);
        }
    }
}