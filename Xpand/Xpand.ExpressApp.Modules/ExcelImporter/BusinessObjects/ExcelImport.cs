using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using DevExpress.ExpressApp;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.Validation;
using DevExpress.Xpo;
using ICSharpCode.SharpZipLib.Core;
using ICSharpCode.SharpZipLib.Zip;
using Xpand.ExpressApp.ExcelImporter.Services;
using Xpand.Extensions.XAF.Attributes;
using Xpand.Extensions.XAF.Xpo.ValueConverters;
using Xpand.Persistent.Base;
using Xpand.Persistent.Base.General;
using Xpand.Xpo.Converters;
using EditorAliases = Xpand.Persistent.Base.General.EditorAliases;

namespace Xpand.ExpressApp.ExcelImporter.BusinessObjects{
    
    [DefaultClassOptions]
    [DefaultProperty(nameof(Name))]
    public class ExcelImport : XpandBaseCustomObject {
        public const string MappingContext = "Mapping";
        public const string ImportingContext = "Importing";
        private const string TargetContextIDs = MappingContext + ";" +ImportingContext+";Save";
        public ExcelImport(Session session) : base(session){
        }

        [Association("ExcelImport-ExcelImportKeyMaps")]
        public XPCollection<ExcelImportKeyMap> KeyMaps => GetCollection<ExcelImportKeyMap>(nameof(KeyMaps));

        [Association("ExcelImport-DroppedFiles")]
        [Aggregated]
        [InvisibleInAllViews]
        [CollectionOperationSet(AllowAdd = false, AllowRemove = true)]
        public XPCollection<DroppedFile> DroppedFiles => GetCollection<DroppedFile>(nameof(DroppedFiles)); 

        [Association("ExcelImport-AutoImportedFiles")]
        [InvisibleInAllViews][Aggregated]
        [CollectionOperationSet(AllowAdd = false, AllowRemove = true)]
        public XPCollection<AutoImportedFile> AutoImportedFiles => GetCollection<AutoImportedFile>(nameof(AutoImportedFiles));
        ImportStrategy _importStrategy;

        [ToolTip("Controls how main objects are created")]
        public ImportStrategy ImportStrategy{
            get => _importStrategy;
            set => SetPropertyValue(nameof(ImportStrategy), ref _importStrategy, value);
        }

        protected override void OnChanged(string propertyName, object oldValue, object newValue) {
            base.OnChanged(propertyName, oldValue, newValue);
            if (propertyName==nameof(HeaderRows))
                Session.Delete(ExcelColumnMaps);
        }

        [Browsable(false)]
        public bool CanImport => ExcelColumnMaps.All(map => map.MemberTypeValues.Count > 0);

        int? _abortThreshold;
        [RuleRange(0,100,SkipNullOrEmptyValues = true)]
        public int? AbortThreshold {
            get => _abortThreshold;
            set => SetPropertyValue(nameof(AbortThreshold), ref _abortThreshold, value);
        }

        int _headerRows;
        
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
        [Size(SizeAttribute.Unlimited)]
        public string AutoImportFrom{
            get => _autoImportFrom;
            set => SetPropertyValue(nameof(AutoImportFrom), ref _autoImportFrom, value);
        }

        string _autoImportRegex;
        [RuleRequiredField(TargetContextIDs = TargetContextIDs)]
        public string AutoImportRegex{
            get => _autoImportRegex;
            set => SetPropertyValue(nameof(AutoImportRegex), ref _autoImportRegex, value);
        }

        string _name;
        [RuleRequiredField(TargetContextIDs = TargetContextIDs)]
        public string Name{
            get => _name;
            set => SetPropertyValue(nameof(Name), ref _name, value);
        }

        string _sheetName;
        [EditorAlias(EditorAliases.StringLookupPropertyEditor)]
        [DataSourceProperty(nameof(SheetNames))]
        [RuleRequiredField(TargetContextIDs = TargetContextIDs)]
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
            get {
                return Type != null ? Type.GetITypeInfo().Members.WhereMapable().Select(info => info.Caption()).ToList() : new List<string>();
            }
        }


        public MemoryStream GetXlsContent(string fileName,byte[] bytes){
            if (fileName.EndsWith("zip")) {
                using var memoryStream = new MemoryStream(bytes);
                using var zipFile = new ZipFile(memoryStream);
                var zipEntry = zipFile.Cast<ZipEntry>().FirstOrDefault(entry => Regex.IsMatch(entry.Name, AutoImportRegex));
                if (zipEntry != null) {
                    byte[] buffer = new byte[4096];
                    using var zipStream = zipFile.GetInputStream(zipEntry);
                    var outStream = new MemoryStream();
                    StreamUtils.Copy(zipStream, outStream, buffer);
                    File.FileName = zipEntry.Name;
                    if (File.FullName != null) {
                        File.FullName = Path.Combine($"{Path.GetDirectoryName(File.FullName)}",zipEntry.Name);
                        FullName = File.FullName;
                    }
                    File.Content = outStream.ToArray();
                    outStream.Position = 0;
                    return outStream;
                }
                throw new InvalidOperationException($"Zip file does not contain a file with extension {AutoImportRegex}");
            }
            return new MemoryStream(bytes);
        }

        public override void AfterConstruction(){
            base.AfterConstruction();
            AutoImportRegex = ".*xlsx|.*xls|.*csv|.*txt|.*zip";
            ColumnMappingRegexPattern = "( *)";
            HeaderRows = 1;
            File=new XpandFileData();
            
            ImportStrategy=ImportStrategy.UpdateOrCreate;
        }
        
        protected override void OnLoaded(){
            base.OnLoaded();
            _file=new XpandFileData();
            if (System.IO.File.Exists(FullName)) {
                using var fileStream = System.IO.File.OpenRead(FullName);
                _file.LoadFromStream(Path.GetFileName(FullName), fileStream);
            }
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
        [RuleRequiredField(TargetContextIDs = ImportingContext)]

        public XPCollection<ExcelColumnMap> ExcelColumnMaps => GetCollection<ExcelColumnMap>(nameof(ExcelColumnMaps));

         Type _type;
        [Size(SizeAttribute.Unlimited)]
        [ValueConverter(typeof(TypeValueConverter))]
        [RuleRequiredField(TargetContextIDs = TargetContextIDs)]
        [ImmediatePostData]
        [TypeConverter(typeof(LocalizedClassInfoTypeConverter))]
        public Type Type{
            get => _type;
            set => SetPropertyValue(nameof(Type), ref _type, value);
        }

        [Association("ExcelImport-FailedImportResults")]
        public XPCollection<FailedImportResult> FailedResults => GetCollection<FailedImportResult>(nameof(FailedResults));

        string _columnMappingReplacement;
        [VisibleInListView(false)]
        [ImmediatePostData]
        public string ColumnMappingReplacement{
            get => _columnMappingReplacement;
            set => SetPropertyValue(nameof(ColumnMappingReplacement), ref _columnMappingReplacement, value);
        }

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

    public enum PersistentTypesImportStrategy {
        UpdateOrCreate,
        CreateAlways,
        FailNotFound,
        UpdateOnly
    }

    public enum ImportStrategy{
        CreateAlways,
        UpdateOrCreate,
        SkipOrCreate,
        UpdateOnly,
        FailNotFound
    }
}