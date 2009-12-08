using System;
using System.Collections.Generic;
using System.ComponentModel;
<<<<<<< HEAD
using DevExpress.Persistent.Base;
using DevExpress.Persistent.BaseImpl;
=======
using System.Linq;
using DevExpress.Persistent.Base;
>>>>>>> CodeDomApproachForWorldCreator
using DevExpress.Persistent.Validation;
using DevExpress.Xpo;
using eXpand.Persistent.Base.PersistentMetaData;
using eXpand.Persistent.BaseImpl.PersistentMetaData.PersistentAttributeInfos;
using eXpand.Persistent.BaseImpl.Validation.FromIPropertyValueValidator;
<<<<<<< HEAD
using eXpand.Xpo;
using eXpand.Xpo.Converters.ValueConverters;
using eXpand.Utils.Helpers;
using System.Linq;

namespace eXpand.Persistent.BaseImpl.PersistentMetaData {
    [DefaultClassOptions]
    [NavigationItem("WorldCreator")]
    public class PersistentClassInfo : PersistentTypeInfo, IPersistentClassInfo,IPropertyValueValidator {
        public const string DynamicAssemblyName = "WorldCreator";

        
        private Type _baseType;
=======
using eXpand.Utils.Helpers;
using eXpand.Xpo;
using eXpand.Xpo.Converters.ValueConverters;

namespace eXpand.Persistent.BaseImpl.PersistentMetaData {
    
    public class PersistentClassInfo : PersistentTemplatedTypeInfo, IPersistentClassInfo, IPropertyValueValidator {
        Type _baseType;
        string _baseTypeFullName;


        Type _mergedObject;
        PersistentAssemblyInfo _persistentAssemblyInfo;
>>>>>>> CodeDomApproachForWorldCreator

        public PersistentClassInfo(Session session) : base(session) {
        }

<<<<<<< HEAD
        private Type _mergedObject;
        [RuleFromIPropertyValueValidatorAttribute(null,DefaultContexts.Save)]
        [Size(SizeAttribute.Unlimited)]
        [ValueConverter(typeof(TypeValueConverter))]
        [TypeConverter(typeof(LocalizedClassInfoTypeConverter))]
        public Type MergedObjectType
        {
            get
            {
                return _mergedObject;
            }
            set
            {
                SetPropertyValue("MergedObjectType", ref _mergedObject, value);
            }
        }
        private string _baseTypeAssemblyQualifiedName;
        [Size(SizeAttribute.Unlimited)]
        [Browsable(false)][MemberDesignTimeVisibility(false)]
        public string BaseTypeAssemblyQualifiedName
        {
            get
            {
                return _baseTypeAssemblyQualifiedName;
            }
            set
            {
                SetPropertyValue("BaseTypeAssemblyQualifiedName", ref _baseTypeAssemblyQualifiedName, value);
            }
        }
        [Size(SizeAttribute.Unlimited)]
        [ValueConverter(typeof(TypeValueConverter))]
        [TypeConverter(typeof(LocalizedClassInfoTypeConverter))]
        public Type BaseType {
            get { return _baseType; }
            set {
                SetPropertyValue("BaseType", ref _baseType, value);
                if (value != null) _baseTypeAssemblyQualifiedName = value.AssemblyQualifiedName;
            }
        }
        
        
        [Association]
        public XPCollection<PersistentMemberInfo> OwnMembers {
            get { return GetCollection<PersistentMemberInfo>("OwnMembers"); }
        }
        
        [Association("PersistentClassInfos-Interfaces")]
        public XPCollection<InterfaceInfo> Interfaces
        {
            get
            {
                return GetCollection<InterfaceInfo>("Interfaces");
            }
        }

        IList<IInterfaceInfo> IPersistentClassInfo.Interfaces {
            get { return new ListConverter<IInterfaceInfo,InterfaceInfo>(Interfaces); }
        }
        #region IPersistentClassInfo Members
        [Browsable(false)]
        [MemberDesignTimeVisibility(false)]
        public string AssemblyName {
            get { return DynamicAssemblyName; }
        }


=======
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

>>>>>>> CodeDomApproachForWorldCreator
        public virtual Type GetDefaultBaseClass() {
            return typeof (eXpandCustomObject);
        }

<<<<<<< HEAD
=======
        IPersistentAssemblyInfo IPersistentClassInfo.PersistentAssemblyInfo {
            get { return PersistentAssemblyInfo; }
            set { PersistentAssemblyInfo = value as PersistentAssemblyInfo; }
        }

>>>>>>> CodeDomApproachForWorldCreator
        IList<IPersistentMemberInfo> IPersistentClassInfo.OwnMembers {
            get { return new ListConverter<IPersistentMemberInfo, PersistentMemberInfo>(OwnMembers); }
        }

<<<<<<< HEAD
        Type IPersistentClassInfo.BaseType {
            get { return BaseType; }
            set { BaseType = value ; }
        }
        #endregion




        public bool IsPropertyValueValid(string propertyName, ref string errorMessageTemplate, ContextIdentifiers contextIdentifiers, string ruleId) {
            if (propertyName==this.GetPropertyInfo(x=>x.MergedObjectType).Name&&MergedObjectType!= null) {
                if (BaseType== null) {
                    errorMessageTemplate = this.GetPropertyInfo(x => x.BaseType).Name + " cannot be null";
                    return false;
                }
                if (TypeAttributes.Where(info => info is PeristentMapInheritanceAttribute).FirstOrDefault()== null) {
=======
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
>>>>>>> CodeDomApproachForWorldCreator
                    errorMessageTemplate = typeof (PeristentMapInheritanceAttribute).Name + " is required";
                    return false;
                }
            }
            return true;
        }
<<<<<<< HEAD
=======
        #endregion
>>>>>>> CodeDomApproachForWorldCreator
    }
}