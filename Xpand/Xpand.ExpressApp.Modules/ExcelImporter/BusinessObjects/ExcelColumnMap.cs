using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using DevExpress.ExpressApp.ConditionalAppearance;
using DevExpress.ExpressApp.DC;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.Validation;
using DevExpress.Xpo;
using Xpand.ExpressApp.ExcelImporter.Controllers;
using Xpand.ExpressApp.ExcelImporter.Services;
using Xpand.Persistent.Base;
using Xpand.Persistent.Base.General.CustomAttributes;
using Xpand.Persistent.Base.General.ValueConverters;
using EditorAliases = DevExpress.ExpressApp.Editors.EditorAliases;

namespace Xpand.ExpressApp.ExcelImporter.BusinessObjects{
    [Appearance("Abstract Types", AppearanceItemType.ViewItem,
        nameof(IsAbstract) + "=True AND " + nameof(MemberTypeValues) + ".Count=0", TargetItems = nameof(PropertyType), FontColor = "Red",
        Context = "ListView")]
    [Appearance("keyMember", AppearanceItemType.ViewItem,
        nameof(KeyMemberExists) + "=False" , TargetItems = nameof(PropertyName), FontColor = "Red",FontStyle = FontStyle.Bold|FontStyle.Strikeout,
        Context = "ListView")]
    [XafDefaultProperty(nameof(PropertyName))]
    public class ExcelColumnMap : XpandBaseCustomObject{
        private ExcelImport _excelImport;

        public ExcelColumnMap(Session session) : base(session){
            
        }

        public override void AfterConstruction() {
            base.AfterConstruction();
            ImportStrategy=PersistentTypesImportStrategy.UpdateOrCreate;
        }


        bool _isAbstract;
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
                        var member = ExcelImport.FindMember(PropertyName);
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
        [RuleFromBoolProperty(TargetContextIDs = "Save;"+ExcelImportDetailViewController.ImportExcelActionName)]
        public bool KeyMemberExists => !IsPersistentBO || ExcelImport.FindMember(PropertyName).MemberTypeInfo.GetKeyMember() != null;

        bool _isPersistentBO;
        [Browsable(false)]
        public bool IsPersistentBO {
            get => _isPersistentBO;
            set => SetPropertyValue(nameof(IsPersistentBO), ref _isPersistentBO, value);
        }

        private List<ITypeInfo> GetPropertyNameTypes() {
            var memberTypeInfo = ExcelImport?.FindMember(PropertyName)?.MemberTypeInfo;
            return memberTypeInfo != null? (!memberTypeInfo.IsPersistent? new[] {memberTypeInfo}.ToList()
                : GetMembers(memberTypeInfo).ToList()): new List<ITypeInfo>();
        }

        private  IEnumerable<ITypeInfo> GetMembers(ITypeInfo typeInfo){
            var typeInfos = new[] {typeInfo}.Concat(typeInfo.Descendants);
            return !typeInfo.IsAbstract ? typeInfos : typeInfos.OrderBy(info => info.IsAbstract);
        }

        string _excelColumnName;
        [EditorAlias(EditorAliases.StringPropertyEditor)]
        [RuleRequiredField(TargetContextIDs = ExcelImportDetailViewController.ImportExcelActionName)]
        public string ExcelColumnName{
            get => _excelColumnName;
            set => SetPropertyValue(nameof(ExcelColumnName), ref _excelColumnName, value);
        }


        [RuleRequiredField(TargetContextIDs = ExcelImportDetailViewController.ImportExcelActionName)]
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