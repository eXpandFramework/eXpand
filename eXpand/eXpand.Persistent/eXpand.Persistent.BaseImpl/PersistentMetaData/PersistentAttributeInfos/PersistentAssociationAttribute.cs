using System;
using System.ComponentModel;
using System.Reflection;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.Validation;
using DevExpress.Xpo;
using eXpand.ExpressApp.Core;
using eXpand.Persistent.Base.PersistentMetaData;
using eXpand.Persistent.Base.PersistentMetaData.PersistentAttributeInfos;

namespace eXpand.Persistent.BaseImpl.PersistentMetaData.PersistentAttributeInfos {
    [DefaultProperty("AssociationName")]
    [InterfaceRegistrator(typeof(IPersistentAssociationAttribute))]
    public class PersistentAssociationAttribute : PersistentAttributeInfo, IPersistentAssociationAttribute {
        string _associationName;
        PersistentClassInfo _elementClassInfo;
        Type _elementType;
        string _elementTypeFullName;


        public PersistentAssociationAttribute(Session session) : base(session) {
        }


        [VisibleInListView(true)]
        [RuleRequiredField(null, DefaultContexts.Save)]
        public string AssociationName {
            get { return _associationName; }
            set { SetPropertyValue("AssociationName", ref _associationName, value); }
        }


        [Size(SizeAttribute.Unlimited)]
        [ValueConverter(typeof (TypeValueConverter))]
        [TypeConverter(typeof (LocalizedClassInfoTypeConverter))]
        public Type ElementType {
            get { return _elementType; }
            set {
                SetPropertyValue("ElementType", ref _elementType, value);
                if (_elementType != null)
                    _elementTypeFullName = _elementType.FullName;
                else if (_elementClassInfo == null && _elementType == null)
                    _elementTypeFullName = null;
            }
        }

        public PersistentClassInfo ElementClassInfo {
            get { return _elementClassInfo; }
            set {
                SetPropertyValue("ElementClassInfo", ref _elementClassInfo, value);
                if (_elementClassInfo != null && _elementClassInfo.PersistentAssemblyInfo != null){
                    _elementTypeFullName = _elementClassInfo.PersistentAssemblyInfo.Name + "." + _elementClassInfo.Name;
                }
                else if (_elementClassInfo == null && _elementType == null)
                    _elementTypeFullName = null;
            }
        }

        [Size(SizeAttribute.Unlimited)]
        [Browsable(false)]
        public string ElementTypeFullName {
            get { return _elementTypeFullName; }
            set { SetPropertyValue("ElementTypeFullName", ref _elementTypeFullName, value); }
        }


        public override AttributeInfo Create() {
            if (!string.IsNullOrEmpty(ElementTypeFullName))
                return GetElementTypeDefinedAttributeInfo();
            ConstructorInfo constructorInfo = typeof (AssociationAttribute).GetConstructor(new[] {typeof (string)});
            return new AttributeInfo(constructorInfo, AssociationName);
        }

        AttributeInfo GetElementTypeDefinedAttributeInfo() {
            var type = ReflectionHelper.GetType(ElementTypeFullName);
            ConstructorInfo constructorInfo =
                typeof (AssociationAttribute).GetConstructor(new[] {typeof (string), typeof (string), typeof (string)});
            return new AttributeInfo(constructorInfo, AssociationName,  type);
        }
    }
}