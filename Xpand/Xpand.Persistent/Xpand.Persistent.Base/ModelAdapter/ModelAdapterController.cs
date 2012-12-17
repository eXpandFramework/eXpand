using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Model.Core;
using DevExpress.ExpressApp.Utils;

namespace Xpand.Persistent.Base.ModelAdapter {
    class ParentCalculator {
        public static ModelNode GetParent(IModelNode node) {
            return GetParent<IModelColumn>(node) ?? GetParent<IModelListView>(node);
        }

        static ModelNode GetParent<TNode>(IModelNode node) where TNode : IModelNode {
            var modelNode = node;
            while (!(modelNode is TNode)) {
                if (modelNode.Parent == null)
                    return null;
                modelNode = modelNode.Parent;
            }
            return (ModelNode)modelNode;
        }

    }
    public class MapModelReadOnlyCalculator : IModelIsReadOnly {

        #region Implementation of IModelIsReadOnly
        public bool IsReadOnly(IModelNode node, string propertyName) {
            var parent = ParentCalculator.GetParent(node);
            var helper = new FastModelEditorHelper();
            return helper.IsPropertyModelBrowsableVisible(parent, propertyName);
        }

        public bool IsReadOnly(IModelNode node) {
            throw new NotImplementedException();
        }
        #endregion
    }

    public class MapModelValueCalculator : IModelValueCalculator {
        public object Calculate(ModelNode node, string propertyName) {
            if (node is IModelNodeEnabled) {
                var modelNode = ParentCalculator.GetParent(node);
                if (modelNode != null) {
                    var calculate = modelNode.GetValue(propertyName);
                    if (calculate != null && calculate.GetType().IsEnum) {
                        var propertyType = node.GetValueInfo(propertyName).PropertyType;
                        if (IsNullableType(propertyType)) {
                            var genericArgument = propertyType.GetGenericArguments()[0];
                            if (genericArgument != calculate.GetType())
                                return Enum.Parse(genericArgument, ((int)calculate).ToString(CultureInfo.InvariantCulture));
                        }
                    }
                    return calculate;
                }
            }
            throw new NotImplementedException(propertyName);
        }
        public bool IsNullableType(Type theType) {
            if (theType.IsGenericType) {
                var genericTypeDefinition = theType.GetGenericTypeDefinition();
                if (genericTypeDefinition != null) return (genericTypeDefinition == typeof(Nullable<>));
            }
            return false;
        }
    }

    public abstract class ModelAdapterController : ViewController {
        protected IEnumerable<string> GetProperties(ModelInterfaceExtenders extenders, Type targetInterface) {
            var types = extenders.GetInterfaceExtenders(targetInterface).Where(
                    type => (type.Namespace + "").StartsWith("DevExpress"));
            var properties = types.SelectMany(type => type.GetPublicProperties());
            return targetInterface.GetPublicProperties().Union(properties).Select(info => info.Name);
        }

        public string GetPath(string name) {
            var folder = AppDomain.CurrentDomain.SetupInformation.ApplicationBase;
            var applicationFolder = Path.Combine(folder, "ModelAdaptor/" + AssemblyInfo.FileVersion);
            var path2 = "ModelAdaptor" + name + ".dll";
            return Path.Combine(applicationFolder + "", path2);
        }
    }

    public static class ModelAdapterExtension {
        public static TNode GetParentNode<TNode>(this IModelNode modelNode) where TNode : IModelNode {
            var parent = modelNode.Parent;
            while (!(parent is TNode)) {
                parent = parent.Parent;
                if (parent == null)
                    break;
            }
            return parent != null ? (TNode)parent : default(TNode);
        }

        public static ModelNode GetNodeByPath(this IModelNode node, string path) {
            const string PathSeparator = "/";
            const string RootNodeName = "Application";
            Guard.ArgumentNotNull(node, "node");
            Guard.ArgumentNotNullOrEmpty(path, "path");
            string[] items = path.Split(new[] { PathSeparator }, StringSplitOptions.RemoveEmptyEntries);
            IModelNode sourceNode = items[0] == RootNodeName ? node.Root : node.GetNode(items[0]);
            for (int i = 1; i < items.Length; ++i) {
                if (sourceNode == null) {
                    return null;
                }
                sourceNode = sourceNode.GetNode(items[i]);
            }
            return (ModelNode)sourceNode;
        }
    }
}