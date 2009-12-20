using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using DevExpress.ExpressApp.NodeWrappers;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.Validation;
using DevExpress.Xpo;
using eXpand.ExpressApp.WorldCreator.Core;
using eXpand.Persistent.Base.PersistentMetaData;
using eXpand.Persistent.BaseImpl.PersistentMetaData.PersistentAttributeInfos;
using eXpand.Persistent.BaseImpl.Validation.FromIPropertyValueValidator;
using eXpand.Utils.Helpers;
using eXpand.Xpo;
using eXpand.Xpo.Converters.ValueConverters;

namespace eXpand.Persistent.BaseImpl.PersistentMetaData {
    
    public class PersistentClassInfo : PersistentTemplatedTypeInfo, IPersistentClassInfo, IPropertyValueValidator {
        Type _baseType;
        string _baseTypeFullName;


        Type _mergedObjectType;

        PersistentAssemblyInfo _persistentAssemblyInfo;

        public PersistentClassInfo(Session session) : base(session) {
        }

        [Index(0)]
        [Size(SizeAttribute.Unlimited)]
        [ValueConverter(typeof (TypeValueConverter))]
        [TypeConverter(typeof (LocalizedClassInfoTypeConverter))]
        public Type BaseType {
            get { return _baseType; }
            set {
                SetPropertyValue("BaseType", ref _baseType, value);
                if (!IsLoading && !IsSaving) {
                    _baseTypeFullName = _baseType != null ? _baseType.FullName : null;
                    _baseClassInfo = null;
                }
            }
        }
        private PersistentClassInfo _baseClassInfo;
        [Index(1)]
        public PersistentClassInfo BaseClassInfo
        {
            get
            {
                return _baseClassInfo;
            }
            set
            {
                SetPropertyValue("BaseClassInfo", ref _baseClassInfo, value);
                if (!IsLoading && !IsSaving){
                    _baseTypeFullName = _baseClassInfo != null
                                            ? _baseClassInfo.PersistentAssemblyInfo.Name + "." + _baseClassInfo.Name
                                            : null;
                    _baseType = null;
                }
            }
        }
        [Browsable(false)]
        string IPersistentClassInfo.BaseTypeFullName
        {
            get
            {
                return _baseTypeFullName;
            }
            set
            {
                SetPropertyValue("BaseTypeFullName", ref _baseTypeFullName, value);
            }
        }
        private string _mergedObjectFullName;
        [Browsable(false)]
        [Size(SizeAttribute.Unlimited)]
        string IPersistentClassInfo.MergedObjectFullName
        {
            get
            {
                return _mergedObjectFullName;
            }
            set
            {
                SetPropertyValue("MergedObjectFullName", ref _mergedObjectFullName, value);

            }
        }

        [Index(2)]
        [RuleFromIPropertyValueValidatorAttribute(null, DefaultContexts.Save)]
        [Size(SizeAttribute.Unlimited)]
        [ValueConverter(typeof (TypeValueConverter))]
        [TypeConverter(typeof (LocalizedClassInfoTypeConverter))]
        public Type MergedObjectType {
            get { return _mergedObjectType; }
            set {
                SetPropertyValue("MergedObjectType", ref _mergedObjectType, value);
                if (!IsLoading && !IsSaving){
                    _mergedObjectFullName = _mergedObjectType != null ? _mergedObjectType.FullName : null;
                    _mergedClassInfo = null;
                }
            }
        }
        private PersistentClassInfo _mergedClassInfo;
        [Index(3)]
        public PersistentClassInfo MergedClassInfo
        {
            get
            {
                return _mergedClassInfo;
            }
            set
            {
                SetPropertyValue("MergedClassInfo", ref _mergedClassInfo, value);
                if (!IsLoading && !IsSaving){
                    _mergedObjectFullName = _mergedClassInfo != null
                                                ? _mergedClassInfo.PersistentAssemblyInfo.Name + "." + _mergedClassInfo.Name
                                                : null;
                    _baseType = null;
                }
            }
        }
        [Index(4)]
        [VisibleInListView(false)]
        [Custom(PropertyInfoNodeWrapper.AllowEditAttribute, "false")]
        [Size(SizeAttribute.Unlimited)]
        public string GeneratedCode
        {
            get { return CodeEngine.GenerateCode(this); }
        }

        [Association("PersistentClassInfo-OwnMembers")]
        [Aggregated]
        public XPCollection<PersistentMemberInfo> OwnMembers {
            get { return GetCollection<PersistentMemberInfo>("OwnMembers"); }
        }

        [Association("PersistentClassInfos-Interfaces")]
        public XPCollection<InterfaceInfo> Interfaces {
            get { return GetCollection<InterfaceInfo>("Interfaces"); }
        }

        [Association("PersistentAssemblyInfo-PersistentClassInfos")]
        public PersistentAssemblyInfo PersistentAssemblyInfo {
            get { return _persistentAssemblyInfo; }
            set { SetPropertyValue("PersistentAssemblyInfo", ref _persistentAssemblyInfo, value); }
        }
        #region IPersistentClassInfo Members
        IList<IInterfaceInfo> IPersistentClassInfo.Interfaces {
            get { return new ListConverter<IInterfaceInfo, InterfaceInfo>(Interfaces); }
        }

        public virtual Type GetDefaultBaseClass() {
            return typeof (eXpandCustomObject);
        }

        IPersistentAssemblyInfo IPersistentClassInfo.PersistentAssemblyInfo {
            get { return PersistentAssemblyInfo; }
            set { PersistentAssemblyInfo = value as PersistentAssemblyInfo; }
        }

        IList<IPersistentMemberInfo> IPersistentClassInfo.OwnMembers {
            get { return new ListConverter<IPersistentMemberInfo, PersistentMemberInfo>(OwnMembers); }
        }

        #endregion
        #region IPropertyValueValidator Members
        public bool IsPropertyValueValid(string propertyName, ref string errorMessageTemplate,
                                         ContextIdentifiers contextIdentifiers, string ruleId) {
            if (propertyName == this.GetPropertyInfo(x => x.MergedObjectType).Name && MergedObjectType != null) {
                if (BaseType == null&&BaseClassInfo== null) {
                    errorMessageTemplate = "One of " + this.GetPropertyInfo(x => x.BaseType).Name + ", " + this.GetPropertyInfo(x => x.BaseClassInfo).Name + " should not be null";
                    return false;
                }
                if (TypeAttributes.Where(info => info is PeristentMapInheritanceAttribute).FirstOrDefault() == null) {
                    errorMessageTemplate = typeof (PeristentMapInheritanceAttribute).Name + " is required";
                    return false;
                }
            }
            return true;
        }
        #endregion
    }
}