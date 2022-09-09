using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using System.Linq;
using DevExpress.ExpressApp.ConditionalAppearance;
using DevExpress.ExpressApp.DC;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.Validation;
using DevExpress.Xpo;
using Xpand.ExpressApp.ExcelImporter.Services;
using Xpand.Extensions.XAF.Attributes;
using Xpand.Extensions.XAF.Xpo.ValueConverters;
using Xpand.Persistent.Base;
using Xpand.XAF.Modules.CloneModelView;
using EditorAliases = DevExpress.ExpressApp.Editors.EditorAliases;

namespace Xpand.ExpressApp.ExcelImporter.BusinessObjects{
    [Appearance("Abstract Types", AppearanceItemType.ViewItem,criteria: AbstractCriteria, TargetItems = nameof(PropertyType), FontColor = "Red",Context = "ListView")]
    [Appearance("keyMember", AppearanceItemType.ViewItem,nameof(KeyMemberExists) + "=False" , TargetItems = nameof(PropertyName), FontColor = "Red",FontStyle = FontStyle.Bold|FontStyle.Strikeout,Context = "ListView")]
    [XafDefaultProperty(nameof(DefaultProperty))]
    [CloneModelView(CloneViewType.ListView, nameof(ExcelColumnMap)+"_Configuration_ListView")]
    [FriendlyKeyProperty(nameof(DefaultProperty))][SuppressMessage("Design", "XAF0023:Do not implement IObjectSpaceLink in the XPO types")]
    public class ExcelColumnMap : XpandBaseCustomObject {
        public const string AbstractCriteria =nameof(IsAbstract) + "=True AND " + nameof(MemberTypeValues) + ".Count=0 AND " +
                                              "[" + nameof(ImportStrategy) +"] In ('" + nameof(PersistentTypesImportStrategy.UpdateOrCreate) + "','" +
                                              nameof(PersistentTypesImportStrategy.CreateAlways) + "')";
        private ExcelImport _excelImport;

        public ExcelColumnMap(Session session) : base(session){
            
        }

        bool _skipEmpty;
        [InvisibleInAllViews]
        public string DefaultProperty => $"{ExcelColumnName}-{PropertyName}";
        public bool SkipEmpty {
            get => _skipEmpty;
            set => SetPropertyValue(nameof(SkipEmpty), ref _skipEmpty, value);
        }

        public override void AfterConstruction() {
            base.AfterConstruction();
            ImportStrategy=PersistentTypesImportStrategy.UpdateOrCreate;
            SkipEmpty = true;
        }

        
        [Browsable(false)]
        public IMemberInfo MemberInfo => this.FindMember() ;

        bool _isAbstract;

        [RuleFromBoolProperty(TargetContextIDs = ExcelImport.ImportingContext,
            InvertResult = true, TargetCriteria = AbstractCriteria,
            CustomMessageTemplate = "Abstract type found. Please configure the map to select a different type.",
            UsedProperties = nameof(PropertyType))]
        [InvisibleInAllViews]
        public bool IsAbstract {
            get => _isAbstract;
            set => SetPropertyValue(nameof(IsAbstract), ref _isAbstract, value);
        }

        [Association("ExcelColumnMap-ExcelColumnMapMemberTypeValues")][DevExpress.Xpo.Aggregated]
        public XPCollection<ExcelColumnMapMemberTypeValue> MemberTypeValues => GetCollection<ExcelColumnMapMemberTypeValue>(nameof(MemberTypeValues));


        protected override void OnChanged(string propertyName, object oldValue, object newValue) {
            base.OnChanged(propertyName, oldValue, newValue);
            if (propertyName == nameof(PropertyName)) {
                Session.Delete(MemberTypeValues);
                if (newValue != null) {
                    var boTypes = GetPropertyNameTypes();
                    if (boTypes.Any()) {
                        var member = MemberInfo;
                        _isAbstract = member.MemberTypeInfo.IsAbstract;
                        PropertyType = member.MemberType;
                        _isPersistentBO = member.MemberTypeInfo.IsPersistent;
                        if (!_isAbstract) {
                            var memberTypeValue = new ExcelColumnMapMemberTypeValue(Session) {
                                ExcelColumnMap = this, PropertyType = member.MemberType
                            };
                            MemberTypeValues.Add(memberTypeValue);
                        }
                    }
                }
            }
        }

        [Browsable(false)]
        [RuleFromBoolProperty(TargetContextIDs = "Save;"+ExcelImport.ImportingContext)]
        public bool KeyMemberExists => !IsPersistentBO || MemberInfo?.MemberTypeInfo.GetKeyMember(ExcelImport) != null;

        bool _isPersistentBO;
        [Browsable(false)]
        public bool IsPersistentBO {
            get => _isPersistentBO;
            set => SetPropertyValue(nameof(IsPersistentBO), ref _isPersistentBO, value);
        }

        private List<ITypeInfo> GetPropertyNameTypes() {
            var memberTypeInfo = MemberInfo?.MemberTypeInfo;
            return memberTypeInfo != null? (!memberTypeInfo.IsPersistent? new[] {memberTypeInfo}.ToList()
                : GetMembers(memberTypeInfo).ToList()): new List<ITypeInfo>();
        }

        private  IEnumerable<ITypeInfo> GetMembers(ITypeInfo typeInfo){
            var typeInfos = new[] {typeInfo}.Concat(typeInfo.Descendants);
            return !typeInfo.IsAbstract ? typeInfos : typeInfos.OrderBy(info => info.IsAbstract);
        }

        string _excelColumnName;
        [EditorAlias(EditorAliases.StringPropertyEditor)]
        [RuleRequiredField(TargetContextIDs = ExcelImport.ImportingContext)]
        public string ExcelColumnName{
            get => _excelColumnName;
            set => SetPropertyValue(nameof(ExcelColumnName), ref _excelColumnName, value);
        }
        [VisibleInDetailView(false)]
        public string ExcelColumnNameRegex => this.GetColumnName();

        [RuleRequiredField(TargetContextIDs = ExcelImport.ImportingContext)]
        [EditorAlias(Persistent.Base.General.EditorAliases.StringLookupPropertyEditor)]
        [DataSourceProperty(nameof(ExcelImport)+"."+nameof(BusinessObjects.ExcelImport.TypePropertyNames))]
        [ImmediatePostData]

        public string PropertyName{
            get => _propertyName;
            set => SetPropertyValue(nameof(PropertyName), ref _propertyName, value);
        }

         string _propertyName;


        Type _propertyType;
        [Size(SizeAttribute.Unlimited)]
        [ValueConverter(typeof(TypeValueConverter))]
        [VisibleInDetailView(false)]
        [TypeConverter(typeof(AllTypesLocalizedClassInfoTypeConverter))]
        [VisibleInListView(true)]
        public Type PropertyType {
            get => _propertyType;
            set => SetPropertyValue(nameof(PropertyType), ref _propertyType, value);
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