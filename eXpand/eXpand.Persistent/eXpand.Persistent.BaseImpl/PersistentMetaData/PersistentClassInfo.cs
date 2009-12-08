using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.Validation;
using DevExpress.Xpo;
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


        Type _mergedObject;
        PersistentAssemblyInfo _persistentAssemblyInfo;

        public PersistentClassInfo(Session session) : base(session) {
        }

        [Size(SizeAttribute.Unlimited)]
        [ValueConverter(typeof (TypeValueConverter))]
        [TypeConverter(typeof (LocalizedClassInfoTypeConverter))]
        public Type BaseType {
            get { return _baseType; }
            set {
                SetPropertyValue("BaseType", ref _baseType, value);
                _baseTypeFullName = _baseType != null ? _baseType.FullName : null;
            }
        }

        [Browsable(false)]
        public string BaseTypeFullName
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

        [Association("PersistentClassInfo-OwnMembers")]
        [Aggregated]
        public XPCollection<PersistentMemberInfo> OwnMembers {
            get { return GetCollection<PersistentMemberInfo>("OwnMembers"); }
        }

        [Association("PersistentClassInfos-Interfaces")]
        public XPCollection<InterfaceInfo> Interfaces {
            get { return GetCollection<InterfaceInfo>("Interfaces"); }
        }

        [RuleRequiredField(null, DefaultContexts.Save)]
        [Association("PersistentAssemblyInfo-PersistentClassInfos")]
        public PersistentAssemblyInfo PersistentAssemblyInfo {
            get { return _persistentAssemblyInfo; }
            set { SetPropertyValue("PersistentAssemblyInfo", ref _persistentAssemblyInfo, value); }
        }
        #region IPersistentClassInfo Members
        [RuleFromIPropertyValueValidatorAttribute(null, DefaultContexts.Save)]
        [Size(SizeAttribute.Unlimited)]
        [ValueConverter(typeof (TypeValueConverter))]
        [TypeConverter(typeof (LocalizedClassInfoTypeConverter))]
        public Type MergedObjectType {
            get { return _mergedObject; }
            set { SetPropertyValue("MergedObjectType", ref _mergedObject, value); }
        }


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
                if (BaseType == null) {
                    errorMessageTemplate = this.GetPropertyInfo(x => x.BaseType).Name + " cannot be null";
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