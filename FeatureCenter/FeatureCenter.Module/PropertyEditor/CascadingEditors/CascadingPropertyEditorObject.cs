using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using DevExpress.ExpressApp;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.BaseImpl;
using DevExpress.Xpo;
using Xpand.ExpressApp.Core;
using Xpand.ExpressApp.PropertyEditors;
using Xpand.Persistent.Base.General;
using Xpand.Persistent.Base.General.CustomAttributes;
using Xpand.Xpo.Converters.ValueConverters;

namespace FeatureCenter.Module.PropertyEditor.CascadingEditors {
    public class CascadingPropertyEditorObject : BaseObject {
        public CascadingPropertyEditorObject(Session session)
            : base(session) {
        }
        private Type _objectType;
        [Size(SizeAttribute.Unlimited)]
        [ValueConverter(typeof(TypeValueConverter))]
        [TypeConverter(typeof(LocalizedClassInfoTypeConverter))]
        [Index(0)]
        [ImmediatePostData]
        public Type ObjectType {
            get {
                return _objectType;
            }
            set {
                SetPropertyValue("ObjectType", ref _objectType, value);
            }
        }
        private string _propertyName;
        [Index(1)]
        [ImmediatePostData]
        [DataSourceProperty("PropertyNames")]
        [PropertyEditor(typeof(IStringLookupPropertyEditor))]
        public string PropertyName {
            get {
                return _propertyName;
            }
            set {
                SetPropertyValue("PropertyName", ref _propertyName, value);
            }
        }
        [Browsable(false)]
        public IList<string> PropertyNames {
            get {
                return ObjectType != null ? XafTypesInfo.Instance.FindTypeInfo(_objectType).Members.Select(info => info.Name).ToList() : new List<string>();
            }
        }

        private object _value;
        [Index(2)]
        [PropertyEditorProperty("ValueEditorType")]
        [Size(SizeAttribute.Unlimited)]
        [ValueConverter(typeof(SerializableObjectConverter))]
        [PropertyEditor(typeof(ISerializableObjectPropertyEditor))]
        public object Value {
            get {
                return _value;
            }
            set {
                SetPropertyValue("Value", ref _value, value);
            }
        }


        [Browsable(false)]
        public Type ValueEditorType {
            get {
                return PropertyName != null && ObjectType != null
                           ? XafTypesInfo.Instance.FindTypeInfo(ObjectType).FindMember(PropertyName).MemberType
                           : null;
            }
        }
    }
}
