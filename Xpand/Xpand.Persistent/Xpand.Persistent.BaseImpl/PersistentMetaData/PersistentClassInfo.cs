using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using DevExpress.ExpressApp.Model;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.Validation;
using DevExpress.Xpo;
using Xpand.ExpressApp.WorldCreator.CodeProvider;
using Xpand.Extensions.XAF.Xpo.ValueConverters;
using Xpand.Persistent.Base;
using Xpand.Persistent.Base.General;
using Xpand.Persistent.Base.PersistentMetaData;
using Xpand.Persistent.Base.Validation.FromIPropertyValueValidator;
using Xpand.Persistent.BaseImpl.PersistentMetaData.PersistentAttributeInfos;

namespace Xpand.Persistent.BaseImpl.PersistentMetaData {
    [InterfaceRegistrator(typeof(IPersistentClassInfo))]
    [RuleObjectExists(nameof(Name) + "='@" + nameof(Name) + "'", InvertResult = true,
        IncludeCurrentObject = false,LooksFor = typeof(PersistentClassInfo),CriteriaEvaluationBehavior = CriteriaEvaluationBehavior.InTransaction)]
    
    public class PersistentClassInfo : PersistentTemplatedTypeInfo, IPersistentClassInfo,IPropertyValueValidator {
        public const string KeyPropertiesMessage = "Key properties count is incorrect";
        private const string BaseTypeFullNameName = "BaseTypeFullName";
        private const string MergedObjectFullNameName = "MergedObjectFullName";
        private const string BaseTypeName = "BaseType";
        PersistentClassInfo _baseClassInfo;
        Type _baseType;
        string _baseTypeFullName;
        PersistentClassInfo _mergedClassInfo;
        string _mergedObjectFullName;


        Type _mergedObjectType;

        PersistentAssemblyInfo _persistentAssemblyInfo;


        public PersistentClassInfo(Session session)
            : base(session) {
        }
        public override void AfterConstruction() {
            base.AfterConstruction();
            BaseType = typeof(XpandBaseCustomObject);
        }
        [Index(0)]
        [Size(SizeAttribute.Unlimited)]
        [ValueConverter(typeof(TypeValueConverter))]
        [TypeConverter(typeof(XpandLocalizedClassInfoTypeConverter))]
        [RuleRequiredField(TargetCriteria = "(MergedObjectType Is Not NULL OR MergedClassInfo Is Not Null) and BaseClassInfo Is NULL")]
        public Type BaseType {
            get => _baseType;
            set {
                SetPropertyValue(BaseTypeName, ref _baseType, value);
                if (_baseType != null) {
                    _baseTypeFullName = _baseType.FullName;
                    OnChanged(BaseTypeFullNameName);
                }
                else if (_baseClassInfo == null && _baseType == null) {
                    _baseTypeFullName = null;
                    OnChanged(BaseTypeFullNameName);
                }
            }
        }

        [Index(1)]
        [RuleRequiredField(TargetCriteria = "(MergedObjectType Is Not NULL OR MergedClassInfo Is Not Null) and BaseType Is NULL")]
        public PersistentClassInfo BaseClassInfo {
            get => _baseClassInfo;
            set {
                SetPropertyValue("BaseClassInfo", ref _baseClassInfo, value);
                if (_baseClassInfo?.PersistentAssemblyInfo != null) {
                    _baseTypeFullName = _baseClassInfo.PersistentAssemblyInfo.Name + "." + _baseClassInfo.Name;
                    OnChanged(BaseTypeFullNameName);
                } else if (_baseClassInfo == null && _baseType == null) {
                    _baseTypeFullName = null;
                    OnChanged(BaseTypeFullNameName);
                }
            }
        }

        [Index(2)]
        [Size(SizeAttribute.Unlimited)]
        [ValueConverter(typeof(TypeValueConverter))]
        [TypeConverter(typeof(LocalizedClassInfoTypeConverter))]
        public Type MergedObjectType {
            get => _mergedObjectType;
            set {
                SetPropertyValue(nameof(MergedObjectType), ref _mergedObjectType, value);
                if (_mergedObjectType != null) {
                    _mergedObjectFullName = _mergedObjectType.FullName;
                    OnChanged(MergedObjectFullNameName);
                }
                else if (_mergedClassInfo == null && _mergedObjectType == null) {
                    _mergedObjectFullName = null;
                    OnChanged(MergedObjectFullNameName);
                }
            }
        }

        [Index(3)]
        public PersistentClassInfo MergedClassInfo {
            get => _mergedClassInfo;
            set {
                SetPropertyValue("MergedClassInfo", ref _mergedClassInfo, value);
                if (_mergedClassInfo?.PersistentAssemblyInfo != null) {
                    _mergedObjectFullName = _mergedClassInfo.PersistentAssemblyInfo.Name + "." + _mergedClassInfo.Name;
                } else if (_mergedClassInfo == null && _mergedObjectType == null)
                    _mergedObjectFullName = null;
            }
        }

        [Index(4)]
        [VisibleInListView(false)]
        [ModelDefault("AllowEdit", "false")]
        [Size(SizeAttribute.Unlimited)]
        [EditorAlias(EditorAliases.CSCodePropertyEditor)]
        public string GeneratedCode => this.GenerateCode();

        [Association("PersistentClassInfo-OwnMembers")]
        [Aggregated]
//        [RuleFromIPropertyValueValidator()]
        public XPCollection<PersistentMemberInfo> OwnMembers => GetCollection<PersistentMemberInfo>();

        [Association("PersistentClassInfos-Interfaces")]
        public XPCollection<InterfaceInfo> Interfaces => GetCollection<InterfaceInfo>();

        [Association("PersistentAssemblyInfo-PersistentClassInfos")]
        public PersistentAssemblyInfo PersistentAssemblyInfo {
            get => _persistentAssemblyInfo;
            set => SetPropertyValue("PersistentAssemblyInfo", ref _persistentAssemblyInfo, value);
        }

        #region IPersistentClassInfo Members
        [Browsable(false)]
        [Size(SizeAttribute.Unlimited)]
        public string BaseTypeFullName {
            get => _baseTypeFullName;
            set {
                SetPropertyValue(BaseTypeFullNameName, ref _baseTypeFullName, value);
                _baseType = ReflectionHelper.FindType(value);
                OnChanged(BaseTypeName);
            }
        }

        [Browsable(false)]
        [Size(SizeAttribute.Unlimited)]
        public string MergedObjectFullName {
            get => _mergedObjectFullName;
            set {
                SetPropertyValue(MergedObjectFullNameName, ref _mergedObjectFullName, value);
                _mergedObjectType=ReflectionHelper.FindType(value);
                OnChanged("MergedObjectType");
            }
        }

        IList<IInterfaceInfo> IPersistentClassInfo.Interfaces => new ListConverter<IInterfaceInfo, InterfaceInfo>(Interfaces);


        IPersistentAssemblyInfo IPersistentClassInfo.PersistentAssemblyInfo {
            get => PersistentAssemblyInfo;
            set => PersistentAssemblyInfo = value as PersistentAssemblyInfo;
        }

        IList<IPersistentMemberInfo> IPersistentClassInfo.OwnMembers => new ListConverter<IPersistentMemberInfo, PersistentMemberInfo>(OwnMembers);

        #endregion

        public bool IsPropertyValueValid(string propertyName, ref string errorMessageTemplate, ContextIdentifiers contextIdentifiers,
            string ruleId){
            if (propertyName == "OwnMembers"&&contextIdentifiers==ContextIdentifier.Save){
                var persistentReferenceMemberInfos = OwnMembers.OfType<PersistentReferenceMemberInfo>().Where(info => info.TypeAttributes.OfType<PersistentAssociationAttribute>().Any());
                foreach (var persistentReferenceMemberInfo in persistentReferenceMemberInfos){
                    var associationAttribute = persistentReferenceMemberInfo.TypeAttributes.OfType<PersistentAssociationAttribute>().FirstOrDefault();
                    if (associationAttribute!=null){
                        var persistentCollectionMemberInfos = persistentReferenceMemberInfo.Owner.OwnMembers.OfType<PersistentCollectionMemberInfo>();
                        if (!persistentCollectionMemberInfos.Any(info => info.TypeAttributes.OfType<PersistentAssociationAttribute>()
                                .Any(attribute => attribute.AssociationName == associationAttribute.AssociationName))){
                            return false;
                        }
                    }
                }
            }
            return true;
        }

    }
}