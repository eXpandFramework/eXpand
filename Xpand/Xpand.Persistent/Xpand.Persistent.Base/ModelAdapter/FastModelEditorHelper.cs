using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Model.Core;

namespace Xpand.Persistent.Base.ModelAdapter {
    class FastModelEditorHelper {
        readonly Dictionary<Type, Dictionary<string, object>> allAttributesCache =
            new Dictionary<Type, Dictionary<string, object>>();

        readonly Dictionary<Type, Dictionary<string, Attribute>> attributesCache =
            new Dictionary<Type, Dictionary<string, Attribute>>();

        readonly Dictionary<string, string> descriptionsCache = new Dictionary<string, string>();
        readonly object lockObject = new object();

        readonly Dictionary<Type, Dictionary<string, Attribute>> nodeAttributes =
            new Dictionary<Type, Dictionary<string, Attribute>>();

        readonly Dictionary<Type, IMemberInfo> nodeDisplayPropertyCache = new Dictionary<Type, IMemberInfo>();

        public T GetPropertyAttribute<T>(ITypeInfo typeInfo, string propertyName) where T : Attribute {
            lock (lockObject) {
                Attribute result;
                Type nodeType = typeInfo.Type;
                Dictionary<string, Attribute> nodeAttributesCache;
                if (!attributesCache.TryGetValue(nodeType, out nodeAttributesCache)) {
                    nodeAttributesCache = new Dictionary<string, Attribute>();
                    attributesCache[nodeType] = nodeAttributesCache;
                }
                string key = propertyName + ":" + typeof(T).FullName;
                if (nodeAttributesCache.ContainsKey(key)) {
                    result = nodeAttributesCache[key];
                } else {
                    result = ModelEditorHelper.GetPropertyAttribute<T>(typeInfo, propertyName);
                    nodeAttributesCache[key] = result;
                }
                return (T)result;
            }
        }

        public T GetPropertyAttribute<T>(ModelNode node, string propertyName) where T : Attribute {
            ITypeInfo type = XafTypesInfo.Instance.FindTypeInfo(node.GetType());
            return GetPropertyAttribute<T>(type, propertyName);
        }

        public IList<T> GetPropertyAttributes<T>(ModelNode node, string propertyName) where T : Attribute {
            lock (lockObject) {
                IList<T> result;
                Type nodeType = node.GetType();
                Dictionary<string, object> nodeAttributesCache;
                if (!allAttributesCache.TryGetValue(nodeType, out nodeAttributesCache)) {
                    nodeAttributesCache = new Dictionary<string, object>();
                    allAttributesCache[nodeType] = nodeAttributesCache;
                }
                string key = propertyName + ":" + typeof(T).FullName;
                if (nodeAttributesCache.ContainsKey(key)) {
                    result = (IList<T>)nodeAttributesCache[key];
                } else {
                    result = ModelEditorHelper.GetPropertyAttributes<T>(node, propertyName);
                    nodeAttributesCache[key] = result;
                }
                return result;
            }
        }

        public T GetNodeAttribute<T>(Type nodeType) where T : Attribute {
            lock (lockObject) {
                Dictionary<string, Attribute> nodeAttributesCache;
                if (!nodeAttributes.TryGetValue(nodeType, out nodeAttributesCache)) {
                    nodeAttributesCache = new Dictionary<string, Attribute>();
                    nodeAttributes[nodeType] = nodeAttributesCache;
                }
                Attribute result;
                string key = typeof(T).FullName;
                if (key == null) throw new ArgumentNullException(string.Format("key{0}", typeof(T)));
                if (nodeAttributesCache.ContainsKey(key)) {
                    result = nodeAttributesCache[key];
                } else {
                    result = ModelEditorHelper.GetNodeAttribute<T>(nodeType);
                    nodeAttributesCache[key] = result;
                }
                return (T)result;
            }
        }

        public T GetNodeAttribute<T>(IModelNode node) where T : Attribute {
            return GetNodeAttribute<T>(XafTypesInfo.Instance.FindTypeInfo(node.GetType()).Type);
        }

        public bool IsPropertyModelBrowsableVisible(ModelNode parentNode, string propertyName) {
            IEnumerable<ModelBrowsableAttribute> modelBrowsableAttributes =
                GetPropertyAttributes<ModelBrowsableAttribute>(parentNode, propertyName);
            var modelIsVisibles = modelBrowsableAttributes.Select(modelBrowsableAttribute => modelBrowsableAttribute.VisibilityCalculatorType).Select(Activator.CreateInstance).OfType<IModelIsVisible>();
            bool isModelBrowsable = modelIsVisibles.Aggregate(true, (current, modelIsVisible) => current && modelIsVisible.IsVisible(parentNode, propertyName));
            IEnumerable<BrowsableAttribute> browsableAttributes = GetPropertyAttributes<BrowsableAttribute>(parentNode, propertyName);
            if (browsableAttributes.Any(modelBrowsableAttribute => !modelBrowsableAttribute.Browsable)) {
                isModelBrowsable = false;
            }
            return isModelBrowsable;
        }

        public string GetPropertyDescription(ModelNode node, string propertyName) {
            lock (lockObject) {
                if (!string.IsNullOrEmpty(propertyName) && node != null) {
                    string result;
                    string key = node.GetType().FullName + propertyName;
                    if (!descriptionsCache.TryGetValue(key, out result)) {
                        result = ModelEditorHelper.GetPropertyDescription(node, propertyName);
                        descriptionsCache[key] = result;
                    }
                    return result;
                }
                return string.Empty;
            }
        }

        public string GetNodeDescription(ModelNode node) {
            lock (lockObject) {
                if (node != null) {
                    string result;
                    string key = node.GetType().FullName;
                    if (key == null) throw new ArgumentNullException(string.Format("key{0}", node.GetType()));
                    if (!descriptionsCache.TryGetValue(key, out result)) {
                        result = ModelEditorHelper.GetNodeDescription(node);
                        descriptionsCache[key] = result;
                    }
                    return result;
                }
                return string.Empty;
            }
        }

        public bool IsReadOnly(ModelNode node, String propertyName) {
            if (string.IsNullOrEmpty(propertyName)) {
                return IsReadOnly(node);
            }
            bool result = false;
            var modelReadOnlyAttribute = GetPropertyAttribute<ModelReadOnlyAttribute>(node, propertyName);
            if (modelReadOnlyAttribute != null) {
                if (modelReadOnlyAttribute.ReadOnlyCalculatorType == typeof(DesignerOnlyCalculator)) {
                    result = !DesignerOnlyCalculator.IsRunFromDesigner;
                } else {
                    var modelIsVisible =
                        Activator.CreateInstance(modelReadOnlyAttribute.ReadOnlyCalculatorType) as IModelIsReadOnly;
                    if (modelIsVisible != null) {
                        result = modelIsVisible.IsReadOnly(node, propertyName);
                    }
                }
            }
            var readOnlyAttribute = GetPropertyAttribute<ReadOnlyAttribute>(node, propertyName);
            if (readOnlyAttribute != null) {
                result |= readOnlyAttribute.IsReadOnly;
            }
            ModelValueInfo valueInfo = node.GetValueInfo(propertyName);
            if (valueInfo != null) {
                result |= valueInfo.IsReadOnly;
            }
            return result;
        }

        public bool IsReadOnly(ModelNode node) {
            bool result = false;
            var modelReadOnlyAttribute = GetNodeAttribute<ModelReadOnlyAttribute>(node);
            if (modelReadOnlyAttribute != null) {
                if (modelReadOnlyAttribute.ReadOnlyCalculatorType == typeof(DesignerOnlyCalculator)) {
                    result = !DesignerOnlyCalculator.IsRunFromDesigner;
                } else {
                    var modelIsVisible =
                        Activator.CreateInstance(modelReadOnlyAttribute.ReadOnlyCalculatorType) as IModelIsReadOnly;
                    if (modelIsVisible != null) {
                        result = modelIsVisible.IsReadOnly(node);
                    }
                }
            }
            var readOnly = GetNodeAttribute<ReadOnlyAttribute>(node);
            if (readOnly != null) {
                result |= readOnly.IsReadOnly;
            }
            return result;
        }

        public bool CanDeleteNode(ModelNode node, bool readOnlyModel) {
            if (node is IModelApplication || readOnlyModel) {
                return false;
            }
            if (node != null && node.Parent != null) {
                bool readOnly = node.Parent.NodeInfo.GetListChildrenTypes().Values.All(item => node.GetType() != item);
                readOnly |= IsReadOnly(node.Parent);
                return !(readOnly);
            }
            return true;
        }

        public string GetNodeDisplayName(Type type) {
            if (type == null) {
                return "";
            }
            var modelDisplayNameAttribute = GetNodeAttribute<ModelDisplayNameAttribute>(type);
            if (modelDisplayNameAttribute != null && !string.IsNullOrEmpty(modelDisplayNameAttribute.ModelDisplayName)) {
                return modelDisplayNameAttribute.ModelDisplayName;
            }
            return ModelApplicationCreator.GetDefaultXmlName(type);
        }

        public Dictionary<string, Type> GetListChildNodeTypes(ModelNodeInfo nodeInfo) {
            var result = new Dictionary<string, Type>();
            foreach (var item in nodeInfo.GetListChildrenTypes()) {
                string caption = GetNodeDisplayName(item.Value);
                result.Add(caption, item.Value);
            }
            return result;
        }

        public Dictionary<string, Type> GetChildNodeTypes(ModelNode node) {
            var result = new Dictionary<string, Type>();
            if (node != null) {
                if (!IsReadOnly(node)) {
                    result = GetListChildNodeTypes(node.NodeInfo);
                }
            }
            return result;
        }

        public bool CanAddNode(ModelNode destNode, params ModelNode[] addingNodes) {
            if (addingNodes.Length == 0 || destNode == null) {
                return false;
            }
            foreach (ModelNode node in addingNodes) {
                if (node == null) {
                    return false;
                }
                bool result = false;
                foreach (Type childType in GetChildNodeTypes(destNode).Values) {
                    if (node.NodeInfo.GeneratedClass.IsAssignableFrom(childType)) {
                        result = true;
                    }
                }
                if (!result) {
                    return false;
                }
            }
            return true;
        }

        public string GetModelNodeDisplayValue(ModelNode modelNode) {
            lock (lockObject) {
                Type modelNodeType = modelNode.GetType();
                IMemberInfo memberInfo;
                if (!nodeDisplayPropertyCache.TryGetValue(modelNodeType, out memberInfo)) {
                    memberInfo = GetModelNodeDisplayMemberInfo(modelNodeType);
                    nodeDisplayPropertyCache[modelNodeType] = memberInfo;
                }
                return GetModelNodeDisplayValue(modelNode, memberInfo);
            }
        }

        public static string GetModelNodeDisplayValue_Static(ModelNode modelNode) {
            return GetModelNodeDisplayValue(modelNode, GetModelNodeDisplayMemberInfo(modelNode.GetType()));
        }

        static string GetModelNodeDisplayValue(ModelNode modelNode, IMemberInfo memberInfo) {
            if (memberInfo != null) {
                object displayValue = memberInfo.GetValue(modelNode);
                if (displayValue != null) {
                    return displayValue.ToString();
                }
                return modelNode.Id;
            }
            return modelNode.Id;
        }

        static IMemberInfo GetModelNodeDisplayMemberInfo(Type modelNodeType) {
            ITypeInfo typeInfo = XafTypesInfo.Instance.FindTypeInfo(modelNodeType);
            IMemberInfo memberInfo = null;
            foreach (ITypeInfo interfaceTypeInfo in typeInfo.ImplementedInterfaces) {
                var displayPropertyAttribute = interfaceTypeInfo.FindAttribute<DisplayPropertyAttribute>();
                if (displayPropertyAttribute != null) {
                    string propertyName = displayPropertyAttribute.PropertyName;
                    memberInfo = typeInfo.FindMember(propertyName);
                    if (memberInfo != null) {
                        break;
                    }
                }
            }
            return memberInfo;
        }
    }
}