using System;
using System.ComponentModel;
using System.Reflection;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.Validation;
using DevExpress.Xpo;
using eXpand.Persistent.Base.PersistentMetaData.PersistentAttributeInfos;
using eXpand.Xpo.Converters.ValueConverters;

namespace eXpand.Persistent.BaseImpl.PersistentMetaData.PersistentAttributeInfos {
    [DefaultProperty("AssociationName")]
    public class PersistentAssociationAttribute : PersistentAttributeInfo {
        string _associationName;
        PersistentClassInfo _elementClassInfo;
        Type _elementType;
        string _elementTypeFullName;


        public PersistentAssociationAttribute(Session session) : base(session) {
        }

        public PersistentAssociationAttribute() {
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
                _elementTypeFullName = _elementType != null ? _elementType.FullName : null;
                _elementClassInfo = null;
            }
        }

        public PersistentClassInfo ElementClassInfo {
            get { return _elementClassInfo; }
            set {
                SetPropertyValue("ElementClassInfo", ref _elementClassInfo, value);
                _elementTypeFullName = _elementClassInfo != null
                                           ? _elementClassInfo.PersistentAssemblyInfo.Name + "." +
                                             _elementClassInfo.Name
                                           : null;
                _elementType = null;
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
            return new AttributeInfo(constructorInfo, AssociationName, new AssemblyName(type.Assembly.FullName+""), type.FullName);
        }
    }
}