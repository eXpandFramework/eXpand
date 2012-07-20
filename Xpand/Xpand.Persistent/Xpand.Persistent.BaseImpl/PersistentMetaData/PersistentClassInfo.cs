using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using DevExpress.ExpressApp.ConditionalAppearance;
using DevExpress.ExpressApp.Model;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.Validation;
using DevExpress.Xpo;
using Xpand.ExpressApp.Core;
using Xpand.ExpressApp.WorldCreator.Core;
using Xpand.Persistent.Base;
using Xpand.Persistent.Base.PersistentMetaData;
using Xpand.Persistent.Base.Validation.FromIPropertyValueValidator;
using Xpand.Persistent.BaseImpl.PersistentMetaData.PersistentAttributeInfos;
using Xpand.Utils.Helpers;

namespace Xpand.Persistent.BaseImpl.PersistentMetaData {
    [InterfaceRegistrator(typeof(IPersistentClassInfo))]
    public class PersistentClassInfo : PersistentTemplatedTypeInfo, IPersistentClassInfo, IPropertyValueValidator {
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
        [Appearance("Disable_ClassInfo_BaseType_For_Legacy",AppearanceItemType.ViewItem,"PersistentAssemblyInfo.IsLegacy=true",Enabled = false)]
        public Type BaseType {
            get { return _baseType; }
            set {
                SetPropertyValue("BaseType", ref _baseType, value);
                if (_baseType != null)
                    _baseTypeFullName = _baseType.FullName;
                else if (_baseClassInfo == null && _baseType == null)
                    _baseTypeFullName = null;
            }
        }

        [Index(1)]
        [Appearance("Disable_ClassInfo_BaseClasInfo_For_Legacy", AppearanceItemType.ViewItem, "PersistentAssemblyInfo.IsLegacy=true", Enabled = false)]
        public PersistentClassInfo BaseClassInfo {
            get { return _baseClassInfo; }
            set {
                SetPropertyValue("BaseClassInfo", ref _baseClassInfo, value);
                if (_baseClassInfo != null && _baseClassInfo.PersistentAssemblyInfo != null) {
                    _baseTypeFullName = _baseClassInfo.PersistentAssemblyInfo.Name + "." + _baseClassInfo.Name;
                } else if (_baseClassInfo == null && _baseType == null)
                    _baseTypeFullName = null;
            }
        }

        [Index(2)]
        [RuleFromIPropertyValueValidator(null, DefaultContexts.Save)]
        [Size(SizeAttribute.Unlimited)]
        [ValueConverter(typeof(TypeValueConverter))]
        [TypeConverter(typeof(LocalizedClassInfoTypeConverter))]
        public Type MergedObjectType {
            get { return _mergedObjectType; }
            set {
                SetPropertyValue("MergedObjectType", ref _mergedObjectType, value);
                if (_mergedObjectType != null)
                    _mergedObjectFullName = _mergedObjectType.FullName;
                else if (_mergedClassInfo == null && _mergedObjectType == null)
                    _mergedObjectFullName = null;
            }
        }

        [Index(3)]
        public PersistentClassInfo MergedClassInfo {
            get { return _mergedClassInfo; }
            set {
                SetPropertyValue("MergedClassInfo", ref _mergedClassInfo, value);
                if (_mergedClassInfo != null && _mergedClassInfo.PersistentAssemblyInfo != null) {
                    _mergedObjectFullName = _mergedClassInfo.PersistentAssemblyInfo.Name + "." + _mergedClassInfo.Name;
                } else if (_mergedClassInfo == null && _mergedObjectType == null)
                    _mergedObjectFullName = null;
            }
        }

        [Index(4)]
        [VisibleInListView(false)]
        [ModelDefault("AllowEdit", "false")]
        [Size(SizeAttribute.Unlimited)]
        public string GeneratedCode {
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
        [Browsable(false)]
        [Size(SizeAttribute.Unlimited)]
        public string BaseTypeFullName {
            get { return _baseTypeFullName; }
            set {
                SetPropertyValue("BaseTypeFullName", ref _baseTypeFullName, value);
                _baseType = ReflectionHelper.FindType(value);
            }
        }

        [Browsable(false)]
        [Size(SizeAttribute.Unlimited)]
        public string MergedObjectFullName {
            get { return _mergedObjectFullName; }
            set { SetPropertyValue("MergedObjectFullName", ref _mergedObjectFullName, value); }
        }

        IList<IInterfaceInfo> IPersistentClassInfo.Interfaces {
            get { return new ListConverter<IInterfaceInfo, InterfaceInfo>(Interfaces); }
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
                if (BaseType == null && BaseClassInfo == null) {
                    errorMessageTemplate = "One of " + this.GetPropertyInfo(x => x.BaseType).Name + ", " +
                                           this.GetPropertyInfo(x => x.BaseClassInfo).Name + " should not be null";
                    return false;
                }
                if (TypeAttributes.FirstOrDefault(info => info is PersistentMapInheritanceAttribute) == null) {
                    errorMessageTemplate = typeof(PersistentMapInheritanceAttribute).Name + " is required";
                    return false;
                }
            }
            return true;
        }
        #endregion
   }
}