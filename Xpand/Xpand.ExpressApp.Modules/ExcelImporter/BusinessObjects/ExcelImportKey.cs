using System;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.Validation;
using DevExpress.Xpo;
using Xpand.Extensions.XAF.Xpo.ValueConverters;
using Xpand.Persistent.Base;
using Xpand.Xpo;

namespace Xpand.ExpressApp.ExcelImporter.BusinessObjects {
    [DefaultClassOptions]
    [SuppressMessage("Design", "XAF0023:Do not implement IObjectSpaceLink in the XPO types")]
    public class ExcelImportKeyMap:XpandBaseCustomObject {
        public ExcelImportKeyMap(Session session) : base(session){
        }

        [Association("ExcelImportKeyMap-ExcelImportKeys")][Aggregated]
        public XPCollection<ExcelImportKey> Keys => GetCollection<ExcelImportKey>(nameof(Keys));

        [Association("ExcelImport-ExcelImportKeyMaps")]
        public XPCollection<ExcelImport> ExcelImports => GetCollection<ExcelImport>(nameof(ExcelImports));
        string _name;
        [RuleRequiredField][RuleUniqueValue]
        public string Name {
            get => _name;
            set => SetPropertyValue(nameof(Name), ref _name, value);
        }
    }
    
    public class ExcelImportKey:XpandCustomObject {
        public ExcelImportKey(Session session) : base(session){
        }

        ExcelImportKeyMap _excelImportKeyMap;

        [Association("ExcelImportKeyMap-ExcelImportKeys")]
        [RuleRequiredField]
        public ExcelImportKeyMap ExcelImportKeyMap {
            get => _excelImportKeyMap;
            set => SetPropertyValue(nameof(ExcelImportKeyMap), ref _excelImportKeyMap, value);
        }

        Type _type;
        [Size(SizeAttribute.Unlimited)]
        [ValueConverter(typeof(TypeValueConverter))]
        [VisibleInDetailView(true)]
        [TypeConverter(typeof(XpandLocalizedClassInfoTypeConverter))]
        [VisibleInListView(true)]
        [RuleRequiredField]
        public Type Type {
            get => _type;
            set => SetPropertyValue(nameof(Type), ref _type, value);
        }

        string _property;
        [RuleRequiredField]
        public string Property {
            get => _property;
            set => SetPropertyValue(nameof(Property), ref _property, value);
        }
    }
}
