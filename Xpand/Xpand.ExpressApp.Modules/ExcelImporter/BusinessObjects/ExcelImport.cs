using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using DevExpress.ExpressApp;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.Validation;
using DevExpress.Xpo;
using DevExpress.Xpo.Metadata.Helpers;
using Xpand.ExpressApp.ExcelImporter.Controllers;
using Xpand.Persistent.Base;
using Xpand.Persistent.Base.General;
using Xpand.Persistent.Base.General.ValueConverters;
using Xpand.Xpo.Converters.ValueConverters;

namespace Xpand.ExpressApp.ExcelImporter.BusinessObjects{
    [DefaultClassOptions]
    public class ExcelImport : XpandBaseCustomObject{
        public ExcelImport(Session session) : base(session){
        }

        int _headerRows;
        [RuleValueComparison(ValueComparisonType.GreaterThan, 0,TargetCriteria = nameof(UseHeaderRows)+"=True",TargetContextIDs = ExcelImportDetailViewController.ExcelMappingActionName)]
        public int HeaderRows{
            get => _headerRows;
            set => SetPropertyValue(nameof(HeaderRows), ref _headerRows, value);
        }


        string _name;
        [RuleRequiredField]
        public string Name{
            get => _name;
            set => SetPropertyValue(nameof(Name), ref _name, value);
        }

        string _sheetName;
        [EditorAlias(EditorAliases.StringLookupPropertyEditor)]
        [DataSourceProperty(nameof(SheetNames))]
        [RuleRequiredField(TargetContextIDs = ExcelImportDetailViewController.ExcelMappingActionName)]
        [ImmediatePostData]
        public string SheetName{
            get => _sheetName;
            set => SetPropertyValue(nameof(SheetName), ref _sheetName, value);
        }

        private List<string> _sheetNames;

        [Browsable(false)]
        [ValueConverter(typeof(SerializableObjectConverter))]
        [Size(SizeAttribute.Unlimited)]
        [Persistent]
        public List<string> SheetNames{
            get => _sheetNames;
            set => SetPropertyValue(nameof(SheetNames), ref _sheetNames, value);
        }

        string _columnMappingRegexPattern;
        [Size(255)]
        [ImmediatePostData]
        [VisibleInListView(false)]
        public string ColumnMappingRegexPattern{
            get => _columnMappingRegexPattern;
            set => SetPropertyValue(nameof(ColumnMappingRegexPattern), ref _columnMappingRegexPattern, value);
        }

        [Browsable(false)]
        public List<string> TypePropertyNames{
            get{
                if (Type != null)
                    return Type.GetTypeInfo().Members
                        .Where(info => info.IsPersistent && !info.IsService && info.Name != GCRecordField.StaticName)
                        .Select(info => string.IsNullOrEmpty(info.DisplayName) ? info.Name : info.DisplayName).ToList();
                return new List<string>();
            }
        }

        public override void AfterConstruction(){
            base.AfterConstruction();
            HeaderRows = 1;
            File=new XpandFileData();
            FailedResultList=new FailedResultList();
        }
        
        protected override void OnLoaded(){
            base.OnLoaded();
            File=new XpandFileData();
            FailedResultList=new FailedResultList();
        }

        string _fullName;
        [Browsable(false)]
        [Size(SizeAttribute.Unlimited)]
        public string FullName{
            get => _fullName;
            set => SetPropertyValue(nameof(FullName), ref _fullName, value);
        }

        XpandFileData _file;
        [NonPersistent]
        [ImmediatePostData]
        public XpandFileData File{
            get => _file;
            set => SetPropertyValue(nameof(File), ref _file, value);
        }

        [Association("ExcelImport-ExcelColumnMaps")][Aggregated]
        [CollectionOperationSet(AllowAdd = false, AllowRemove = true)]
        [RuleRequiredField(TargetContextIDs = ExcelImportDetailViewController.ImportExcelActionName)]
        public XPCollection<ExcelColumnMap> ExcelColumnMaps => GetCollection<ExcelColumnMap>(nameof(ExcelColumnMaps));

         Type _type;
        [Size(SizeAttribute.Unlimited)]
        [ValueConverter(typeof(TypeValueConverter))]
        [RuleRequiredField(TargetContextIDs = ExcelImportDetailViewController.ExcelMappingActionName+";"+ExcelImportDetailViewController.ImportExcelActionName)]
        [ImmediatePostData]
        public Type Type{
            get => _type;
            set => SetPropertyValue(nameof(Type), ref _type, value);
        }

        bool _useHeaderRows;
        [ImmediatePostData]
        public bool UseHeaderRows{
            get => _useHeaderRows;
            set => SetPropertyValue(nameof(UseHeaderRows), ref _useHeaderRows, value);
        }

        FailedResultList _failedResultList;
        [ExpandObjectMembers(ExpandObjectMembers.InDetailView)]
        [VisibleInListView(true)]
        
        public FailedResultList FailedResultList{
            get => _failedResultList;
            set => SetPropertyValue(nameof(FailedResultList), ref _failedResultList, value);
        }

        string _columnMappingReplacement;
        [VisibleInListView(false)]
        [ImmediatePostData]
        public string ColumnMappingReplacement{
            get => _columnMappingReplacement;
            set => SetPropertyValue(nameof(ColumnMappingReplacement), ref _columnMappingReplacement, value);
        }
    }
}