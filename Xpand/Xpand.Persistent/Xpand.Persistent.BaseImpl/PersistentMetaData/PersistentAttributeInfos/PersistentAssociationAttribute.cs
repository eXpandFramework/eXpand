using System;
using System.ComponentModel;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.Validation;
using DevExpress.Xpo;
using Xpand.Persistent.Base.General;
using Xpand.Persistent.Base.General.Controllers;
using Xpand.Persistent.Base.General.ValueConverters;
using Xpand.Persistent.Base.PersistentMetaData;
using Xpand.Persistent.Base.PersistentMetaData.PersistentAttributeInfos;

namespace Xpand.Persistent.BaseImpl.PersistentMetaData.PersistentAttributeInfos {
    [DefaultProperty("AssociationName")]
    [InterfaceRegistrator(typeof(IPersistentAssociationAttribute))]
    [System.ComponentModel.DisplayName("Association")]
    [CreateableItem(typeof(IPersistentMemberInfo))]
    [CreateableItem(typeof(IExtendedMemberInfo))]
    public class PersistentAssociationAttribute : PersistentAttributeInfo, IPersistentAssociationAttribute {
        private RelationType _relationType;
        string _associationName;
        PersistentClassInfo _elementClassInfo;
        Type _elementType;
        bool _useAssociationNameAsIntermediateTableName ;
        string _elementTypeFullName;


        public PersistentAssociationAttribute(Session session)
            : base(session) {
        }

        public RelationType RelationType{
            get => _relationType;
            set => SetPropertyValue("RelationType", ref _relationType, value);
        }

        [VisibleInListView(true)]
        [RuleRequiredField(null, DefaultContexts.Save)]
        [Size(SizeAttribute.Unlimited)]
        public string AssociationName {
            get => _associationName;
            set => SetPropertyValue("AssociationName", ref _associationName, value);
        }

        [AttributeInfo]
        public bool UseAssociationNameAsIntermediateTableName  {
            get => _useAssociationNameAsIntermediateTableName ;
            set => SetPropertyValue(nameof(UseAssociationNameAsIntermediateTableName ), ref _useAssociationNameAsIntermediateTableName , value);
        }

        [Size(SizeAttribute.Unlimited)]
        [ValueConverter(typeof(TypeValueConverter))]
        [TypeConverter(typeof(LocalizedClassInfoTypeConverter))]
        public Type ElementType {
            get => _elementType;
            set {
                SetPropertyValue("ElementType", ref _elementType, value);
                if (_elementType != null)
                    _elementTypeFullName = _elementType.FullName;
                else if (_elementClassInfo == null && _elementType == null)
                    _elementTypeFullName = null;
            }
        }

        public PersistentClassInfo ElementClassInfo {
            get => _elementClassInfo;
            set {
                SetPropertyValue("ElementClassInfo", ref _elementClassInfo, value);
                if (_elementClassInfo?.PersistentAssemblyInfo != null) {
                    _elementTypeFullName = _elementClassInfo.PersistentAssemblyInfo.Name + "." + _elementClassInfo.Name;
                } else if (_elementClassInfo == null && _elementType == null)
                    _elementTypeFullName = null;
            }
        }

        [Size(SizeAttribute.Unlimited)]
        [Browsable(false)]
        public string ElementTypeFullName {
            get => _elementTypeFullName;
            set => SetPropertyValue("ElementTypeFullName", ref _elementTypeFullName, value);
        }


        public override AttributeInfoAttribute Create() {
            if (!string.IsNullOrEmpty(ElementTypeFullName))
                return GetElementTypeDefinedAttributeInfo();
            var constructorInfo = typeof(AssociationAttribute).GetConstructor(new[] { typeof(string) });
            return new AttributeInfoAttribute(constructorInfo, AssociationName){Instance = this};
        }

        AttributeInfoAttribute GetElementTypeDefinedAttributeInfo() {
            var type = ReflectionHelper.GetType(ElementTypeFullName);
            var constructorInfo =typeof(AssociationAttribute).GetConstructor(new[] { typeof(string), typeof(string), typeof(string) });
            return new AttributeInfoAttribute(constructorInfo, AssociationName, type){Instance = this};
        }
    }
}