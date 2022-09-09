using System;
using System.Diagnostics.CodeAnalysis;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Utils;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.Validation;
using DevExpress.Xpo;
using Xpand.Extensions.XAF.Attributes;
using Xpand.Extensions.XAF.Xpo.ValueConverters;
using Xpand.Persistent.Base;
using EditorAliases = Xpand.Persistent.Base.General.EditorAliases;

namespace Xpand.ExpressApp.ExcelImporter.BusinessObjects{
    [DefaultListViewOptions(true,NewItemRowPosition.Top)]
    [SuppressMessage("Design", "XAF0023:Do not implement IObjectSpaceLink in the XPO types")]
    public class ExcelColumnMapMemberTypeValue:XpandBaseCustomObject {
        public ExcelColumnMapMemberTypeValue(Session session) : base(session) {
        }

        public override void AfterConstruction() {
            base.AfterConstruction();
            _regexValue = ".*";
        }

        string _regexValue;
        [RuleRequiredField]
        [EditorAlias(EditorAliases.StringPropertyEditor)]
        [Index(1)]
        public string RegexValue {
            get => _regexValue;
            set => SetPropertyValue(nameof(RegexValue), ref _regexValue, value);
        }

        Type _propertyType;

        protected override void OnChanged(string propertyName, object oldValue, object newValue) {
            base.OnChanged(propertyName, oldValue, newValue);
            if (propertyName == nameof(Type)) {
                _propertyType = XafTypesInfo.Instance.FindTypeInfo(Type).Type;
                _type = CaptionHelper.GetClassCaption(_propertyType.FullName);
            }
        }

        [RuleRequiredField]
        [Size(SizeAttribute.Unlimited)]
        [ValueConverter(typeof(TypeValueConverter))]
        [InvisibleInAllViews]
        [Index(0)]
        public Type PropertyType {
            get => _propertyType;
            set => SetPropertyValue(nameof(PropertyType), ref _propertyType, value);
        }

        protected override void OnLoaded() {
            base.OnLoaded();
            _type = CaptionHelper.GetClassCaption(_propertyType.FullName);
        }

        string _type;
        [NonPersistent]
        [EditorAlias(EditorAliases.StringPropertyEditor)]
        [NonCloneable]
        public string Type {
            get => _type;
            set => SetPropertyValue(nameof(Type), ref _type, value);
        }

        private ExcelColumnMap _excelColumnMap;

        [Association("ExcelColumnMap-ExcelColumnMapMemberTypeValues")]
        public ExcelColumnMap ExcelColumnMap {
            get => _excelColumnMap;
            set => SetPropertyValue(nameof(ExcelColumnMap), ref _excelColumnMap, value);
        }
    }
}