using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Model.Core;
using Fasterflect;

namespace Xpand.Persistent.Base.ModelAdapter {
    public abstract class ModelSynchronizer<TComponent, TModelNode> : DevExpress.ExpressApp.Model.ModelSynchronizer<TComponent, TModelNode> where TModelNode : IModelNode {
        public static readonly HashSet<string> ExcludedNodeMembers =
            new HashSet<string>(new[] { "Id", "Index", "Removed", "IsNewNode", "IsRemovedNode" });
        protected ModelSynchronizer(TComponent component, TModelNode modelNode)
            : base(component, modelNode) {
        }

        protected void SynchronizeValues(ModelNode modelNode, object component, PropertyDescriptorCollection properties) {

            foreach (var valueInfo in GetModelValueInfos(modelNode)) {
                var propertyDescriptor = properties.Find(valueInfo.Name, false);
                if (propertyDescriptor != null) {
                    var propertyValue = GetPropertyValue(component, propertyDescriptor, valueInfo, modelNode);
                    var modelValue = GetSynchronizeValuesNodeValue(modelNode, valueInfo, propertyDescriptor, valueInfo.PropertyType.IsNullableType(), component);
                    if (modelValue != null && !modelValue.Equals(propertyValue)) {
                        modelNode.SetValue(valueInfo.Name, propertyValue);
                    }
                }
            }
        }

        protected virtual object GetSynchronizeValuesNodeValue(ModelNode modelNode, ModelValueInfo valueInfo, PropertyDescriptor propertyDescriptor, bool isNullableType, object component) {
            return !isNullableType ? GetNodeValueCore(modelNode, valueInfo) : null;
        }

        protected virtual object GetPropertyValue(object component, PropertyDescriptor propertyDescriptor, ModelValueInfo valueInfo, ModelNode modelNode) {
            return propertyDescriptor.GetValue(component);
        }

        protected virtual bool IsDefaultValue(object value, PropertyDescriptor propertyDescriptor) {
            var defaultValueAttribute = propertyDescriptor.Attributes.OfType<DefaultValueAttribute>().SingleOrDefault();
            return defaultValueAttribute != null && defaultValueAttribute.Value.Equals(value);
        }

        protected void ApplyModel(IModelNode node, object component, Action<ModelNode, object, PropertyDescriptorCollection> action) {
            CheckComponentType(component);
            PropertyDescriptorCollection properties = TypeDescriptor.GetProperties(component);
            var mNode = (ModelNode)node;
            action.Invoke(mNode, component, properties);
            for (int i = 0; i < node.NodeCount; i++) {
                var modelNode = mNode[i];
                var propertyDescriptor = properties.Find(modelNode.Id, false);
                if (propertyDescriptor != null) {
                    ApplyModel(modelNode, propertyDescriptor.GetValue(component), action);
                }
            }
        }

        protected virtual void CheckComponentType(object component) {
            if (component is IModelNode)
                throw new ArgumentOutOfRangeException("component");
        }

        protected void ApplyValues(ModelNode node, object component, PropertyDescriptorCollection properties) {
            foreach (var valueInfo in GetModelValueInfos(node)) {
                var propertyDescriptor = properties.Find(valueInfo.Name, false);
                if (propertyDescriptor != null) {
                    var nodeValue = GetApplyModelNodeValue(node, valueInfo);
                    if (nodeValue != null) {
                        var propertyType = propertyDescriptor.PropertyType;
                        var propertyValue = propertyDescriptor.GetValue(component);
                        if ((!IsDefaultCoreValue(nodeValue, propertyType) || (!nodeValue.Equals(propertyValue))) && propertyType.IsValidEnum(nodeValue)) {
                            if (!nodeValue.Equals(propertyValue))
                                propertyDescriptor.SetValue(component, nodeValue);
                        }
                    }
                }
            }
        }

        protected virtual object GetApplyModelNodeValue(ModelNode node, ModelValueInfo valueInfo) {
            return GetNodeValueCore(node, valueInfo);
        }

        protected object PropertyDefaultValue(object value, ModelNode node, PropertyDescriptor propertyDescriptor, ModelValueInfo valueInfo, object component) {
            var defaultValueAttribute = propertyDescriptor.Attributes.OfType<DefaultValueAttribute>().FirstOrDefault();
            if (defaultValueAttribute == null) {
                if (propertyDescriptor.PropertyType.IsStruct())
                    return propertyDescriptor.PropertyType.CreateInstance();
            } else if (defaultValueAttribute.Value.Equals(propertyDescriptor.GetValue(component))) {
                return null;
            }
            return NodeRealValueCore(valueInfo, node);
        }

        object NodeRealValueCore(ModelValueInfo valueInfo, ModelNode node) {
            var nodeValue = GetNodeValueCore(node, valueInfo);
            if (nodeValue == null && !valueInfo.IsReadOnly && valueInfo.PropertyType != typeof(string)) {
                if (valueInfo.PropertyType.IsStruct())
                    return valueInfo.PropertyType.CreateInstance();
                if (valueInfo.PropertyType.IsNullableType())
                    return valueInfo.PropertyType.GetGenericArguments()[0].CreateInstance();
                throw new NotImplementedException(valueInfo.PropertyType.ToString());
            }
            return nodeValue;
        }

        protected virtual object GetNodeValueCore(ModelNode node, ModelValueInfo valueInfo) {
            if (valueInfo.IsReadOnly)
                return null;
            var nodeValue = node.GetValue(valueInfo.Name);
            return valueInfo.PropertyType.IsStruct() && nodeValue.Equals(valueInfo.PropertyType.CreateInstance())
                       ? null
                       : nodeValue;
        }


        protected virtual IEnumerable<ModelValueInfo> GetModelValueInfos(IModelNode modelNode) {
            return IsDisabled(modelNode) ? Enumerable.Empty<ModelValueInfo>() : ((ModelNode)modelNode).NodeInfo.ValuesInfo.Where(IsNotExcluded);
        }

        bool IsDisabled(IModelNode modelNode) {
            return modelNode is IModelNodeEnabled && !((IModelNodeEnabled)modelNode).NodeEnabled;
        }

        bool IsNotExcluded(ModelValueInfo info) {
            return !ExcludedNodeMembers.Contains(info.Name);
        }

        protected virtual bool IsDefaultCoreValue(object value, Type propertyType) {
            if (value == null)
                return true;
            if (value is string)
                return ReferenceEquals(value, "");
            return propertyType.IsValueType && propertyType.CreateInstance().Equals(value);
        }
    }
    public abstract class ModelListSynchronizer : ModelSynchronizer {
        private readonly ModelSynchronizerList _modelSynchronizerList;

        protected ModelListSynchronizer(object control, IModelNode model)
            : base(control, model) {
            _modelSynchronizerList = new ModelSynchronizerList();

        }

        protected override void ApplyModelCore() {
            _modelSynchronizerList.ApplyModel();
        }
        public override void SynchronizeModel() {
            _modelSynchronizerList.SynchronizeModel();
        }

        public ModelSynchronizerList ModelSynchronizerList {
            get { return _modelSynchronizerList; }
        }

        public override void Dispose() {
            base.Dispose();
            _modelSynchronizerList.Dispose();
        }

    }

}