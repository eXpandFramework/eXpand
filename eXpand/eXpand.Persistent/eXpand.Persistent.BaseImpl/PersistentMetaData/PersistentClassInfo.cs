using System;
using System.Collections.Generic;
using System.ComponentModel;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.Validation;
using DevExpress.Xpo;
using eXpand.Persistent.Base.PersistentMetaData;
using eXpand.Persistent.BaseImpl.PersistentMetaData.PersistentAttributeInfos;
using eXpand.Persistent.BaseImpl.Validation.FromIPropertyValueValidator;
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

        public PersistentClassInfo(Session session) : base(session) {
        }

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


        public virtual Type GetDefaultBaseClass() {
            return typeof (eXpandCustomObject);
        }

        IList<IPersistentMemberInfo> IPersistentClassInfo.OwnMembers {
            get { return new ListConverter<IPersistentMemberInfo, PersistentMemberInfo>(OwnMembers); }
        }

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
                    errorMessageTemplate = typeof (PeristentMapInheritanceAttribute).Name + " is required";
                    return false;
                }
            }
            return true;
        }
    }
}