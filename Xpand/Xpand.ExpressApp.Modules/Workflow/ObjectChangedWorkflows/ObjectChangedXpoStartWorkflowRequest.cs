using System;
using System.ComponentModel;
using DevExpress.ExpressApp.Model.Core;
using DevExpress.ExpressApp.Workflow;
using DevExpress.Xpo;
using Xpand.Xpo;
using Xpand.Xpo.Converters;


namespace Xpand.ExpressApp.Workflow.ObjectChangedWorkflows {
    public interface IObjectChangedWorkflowRequest : IStartWorkflowRequest {
        string PropertyName { get; set; }
        object OldValue { get; set; }
    }
    public class KeyConverter : DevExpress.Xpo.Metadata.ValueConverter {
        public override object ConvertFromStorageType(object value) {
            return XPWeakReference.StringToKey((string)value);
        }
        public override object ConvertToStorageType(object value) {
            return XPWeakReference.KeyToString(value);
        }
        public override Type StorageType => typeof(string);
    }

    public class ObjectChangedXpoStartWorkflowRequest : XpandCustomObject, IObjectChangedWorkflowRequest {

        [TypeConverter(typeof(StringToTypeConverter))]
        public Type TargetObjectType {
            get => _targetObjectType;
            set => SetPropertyValue("TargetObjectType", ref _targetObjectType, value);
        }
        #region IDCStartWorkflowRequest Members
        public string TargetWorkflowUniqueId {
            get => GetPropertyValue<string>();
            set => SetPropertyValue("TargetWorkflowUniqueId", value);
        }

        [ValueConverter(typeof(KeyConverter))]
        public object TargetObjectKey {
            get => GetPropertyValue<object>();
            set => SetPropertyValue<object>("TargetObjectKey", value);
        }
        #endregion
        #region IObjectChangedWorkflowRequest Members
        public string PropertyName {
            get => _propertyName;
            set => SetPropertyValue("PropertyName", ref _propertyName, value);
        }

        [ValueConverter(typeof(SerializableObjectConverter))]
        [Size(SizeAttribute.Unlimited)]
        public object OldValue {
            get => _oldValue;
            set => SetPropertyValue("OldValue", ref _oldValue, value);
        }
        #endregion
        public ObjectChangedXpoStartWorkflowRequest(Session session)
            : base(session) {
        }

        object _oldValue;
        string _propertyName;
        Type _targetObjectType;
    }
}