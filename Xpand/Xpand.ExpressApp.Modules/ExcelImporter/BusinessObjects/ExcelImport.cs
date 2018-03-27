using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.DC;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.Validation;
using DevExpress.Xpo;
using DevExpress.Xpo.Metadata.Helpers;
using Xpand.ExpressApp.ExcelImporter.Controllers;
using Xpand.ExpressApp.ExcelImporter.Services;
using Xpand.Persistent.Base;
using Xpand.Persistent.Base.General;
using Xpand.Persistent.Base.General.CustomAttributes;
using Xpand.Persistent.Base.General.ValueConverters;
using Xpand.Xpo.Converters.ValueConverters;

namespace Xpand.ExpressApp.ExcelImporter.BusinessObjects{
    [DefaultClassOptions]
    [DefaultProperty(nameof(Name))]
    public class ExcelImport : XpandBaseCustomObject,IObjectSpaceLink{
        public ExcelImport(Session session) : base(session){
        }

        [Association("ExcelImport-AutoImportedFiles")]
        [InvisibleInAllViews][DevExpress.Xpo.Aggregated]
        [CollectionOperationSet(AllowAdd = false, AllowRemove = true)]
        public XPCollection<AutoImportedFile> AutoImportedFiles => GetCollection<AutoImportedFile>(nameof(AutoImportedFiles));
        ImportStrategy _importStrategy;

        public ImportStrategy ImportStrategy{
            get => _importStrategy;
            set => SetPropertyValue(nameof(ImportStrategy), ref _importStrategy, value);
        }

        int _headerRows;
        [RuleValueComparison(ValueComparisonType.GreaterThan, 0,TargetCriteria = nameof(UseHeaderRows)+"=True",TargetContextIDs = ExcelImportDetailViewController.ExcelMappingActionName)]
        public int HeaderRows{
            get => _headerRows;
            set => SetPropertyValue(nameof(HeaderRows), ref _headerRows, value);
        }

        string _validationContexts;

        public string ValidationContexts{
            get => _validationContexts;
            set => SetPropertyValue(nameof(ValidationContexts), ref _validationContexts, value);
        }

        string _autoImportFrom;
        [InvisibleInAllViews]
        public string AutoImportFrom{
            get => _autoImportFrom;
            set => SetPropertyValue(nameof(AutoImportFrom), ref _autoImportFrom, value);
        }

        string _autoImportRegex;
        [RuleRequiredField]
        public string AutoImportRegex{
            get => _autoImportRegex;
            set => SetPropertyValue(nameof(AutoImportRegex), ref _autoImportRegex, value);
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
                        .Where(IsMappable)
                        .Select(info => string.IsNullOrEmpty(info.DisplayName) ? info.Name : info.DisplayName).ToList();
                return new List<string>();
            }
        }

        private  bool IsMappable(IMemberInfo info){
            var isMappable = info.IsPersistent && !info.IsService && info.Name != GCRecordField.StaticName;
            if (isMappable){
                var browsableAttribute = info.FindAttribute<BrowsableAttribute>();
                var isBrowsable = browsableAttribute == null || browsableAttribute.Browsable;
                var visibleInLookupListViewAttribute = info.FindAttribute<VisibleInLookupListViewAttribute>();
                var visibleInLookup = visibleInLookupListViewAttribute == null ||(bool) visibleInLookupListViewAttribute.Value;
                var visibleInListViewAttribute = info.FindAttribute<VisibleInListViewAttribute>();
                var visibleInListView= visibleInListViewAttribute == null ||(bool) visibleInListViewAttribute.Value;
                var visibleInExcelMapAttribute = info.FindAttribute<VisibleInExcelMapAttribute>();
                var visibleInExcelMap = visibleInExcelMapAttribute == null ||visibleInExcelMapAttribute.Visible;
                return (isBrowsable && visibleInLookup && visibleInListView) || visibleInExcelMap;
            }
            return false;
        }

        public override void AfterConstruction(){
            base.AfterConstruction();
            AutoImportRegex = ".*xlsx|.*xls|.*csv|.*txt";
            ColumnMappingRegexPattern = "( *)";
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

        [Association("ExcelImport-ExcelColumnMaps")][DevExpress.Xpo.Aggregated]
        [CollectionOperationSet(AllowAdd = false, AllowRemove = true)]
        [RuleRequiredField(TargetContextIDs = ExcelImportDetailViewController.ImportExcelActionName)]
        public XPCollection<ExcelColumnMap> ExcelColumnMaps => GetCollection<ExcelColumnMap>(nameof(ExcelColumnMaps));

         Type _type;
        [Size(SizeAttribute.Unlimited)]
        [ValueConverter(typeof(TypeValueConverter))]
        [RuleRequiredField(TargetContextIDs = ExcelImportDetailViewController.ExcelMappingActionName+";"+ExcelImportDetailViewController.ImportExcelActionName)]
        [ImmediatePostData]
        [TypeConverter(typeof(XpandLocalizedClassInfoTypeConverter))]
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
        [VisibleInListView(false)]
        
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
        [Browsable(false)]
        public IObjectSpace ObjectSpace{ get; set; }

        bool _stopAutoImportOnFailure;
        [InvisibleInAllViews]
        public bool StopAutoImportOnFailure{
            get => _stopAutoImportOnFailure;
            set => SetPropertyValue(nameof(StopAutoImportOnFailure), ref _stopAutoImportOnFailure, value);
        }

        SearchOption _autoImportSearchType;

        public SearchOption AutoImportSearchType{
            get => _autoImportSearchType;
            set => SetPropertyValue(nameof(AutoImportSearchType), ref _autoImportSearchType, value);
        }

        AutoImportNotification _autoImportNotification;

        public AutoImportNotification AutoImportNotification{
            get => _autoImportNotification;
            set => SetPropertyValue(nameof(AutoImportNotification), ref _autoImportNotification, value);
        }
    }

    public enum AutoImportNotification{
        Always,
        Failures,
        Never
    }

    public enum ImportStrategy{
        CreateAlways,
        UpdateOrCreate,
        SkipOrCreate
    }
}