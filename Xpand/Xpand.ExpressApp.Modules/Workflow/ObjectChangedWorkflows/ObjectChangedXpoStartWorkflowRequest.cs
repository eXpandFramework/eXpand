using System;
using System.ComponentModel;
using DevExpress.ExpressApp.Model.Core;
using DevExpress.ExpressApp.Workflow;
using DevExpress.Xpo;
using Xpand.Xpo;
using Xpand.Xpo.Converters.ValueConverters;

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
        public override Type StorageType {
            get { return typeof(string); }
        }
    }

    public class ObjectChangedXpoStartWorkflowRequest : XpandCustomObject, IObjectChangedWorkflowRequest {

        [TypeConverter(typeof(StringToTypeConverter))]
        public Type TargetObjectType {
            get { return _targetObjectType; } 
            set { SetPropertyValue("TargetObjectType", ref _targetObjectType, value); }
        }
        #region IDCStartWorkflowRequest Members
        public string TargetWorkflowUniqueId {
            get { return GetPropertyValue<string>("TargetWorkflowUniqueId"); } 
            set { SetPropertyValue("TargetWorkflowUniqueId", value); }
        }

        [ValueConverter(typeof(KeyConverter))]
        public object TargetObjectKey {
            get { return GetPropertyValue<object>("TargetObjectKey"); } 
            set { SetPropertyValue<object>("TargetObjectKey", value); }
        }
        #endregion
        #region IObjectChangedWorkflowRequest Members
        public string PropertyName {
            get { return _propertyName; } 
            set { SetPropertyValue("PropertyName", ref _propertyName, value); }
        }

        [ValueConverter(typeof(SerializableObjectConverter))]
        [Size(SizeAttribute.Unlimited)]
        public object OldValue {
            get { return _oldValue; }
            set { SetPropertyValue("OldValue", ref _oldValue, value); }
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