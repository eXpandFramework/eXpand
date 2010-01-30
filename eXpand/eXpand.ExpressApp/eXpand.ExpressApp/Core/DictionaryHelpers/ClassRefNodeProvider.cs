using System;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Core.DictionaryHelpers;
using DevExpress.ExpressApp.NodeWrappers;
using DevExpress.Persistent.Base;

namespace eXpand.ExpressApp.Core.DictionaryHelpers {
    public class ClassRefNodeProvider : AttributeRefNodeProvider {
        readonly string classNameAttrPath;
        readonly ExpressionParamsParser parser;

        public ClassRefNodeProvider(string param)
            : base(param) {
            parser = new ExpressionParamsParser(param);
            classNameAttrPath = parser.GetParamValue("ClassName");
        }

        protected override ReadOnlyDictionaryNodeCollection GetNodesCollectionInternal(DictionaryNode node,
                                                                                       string attributeName) {
            var result = new DictionaryNodeCollection();
            DictionaryNode classesNode = node.Dictionary.RootNode.FindChildNode(BOModelNodeWrapper.NodeName);
            if (classesNode != null) {
                string typeName = parser.GetAttributeValueByPath(node, classNameAttrPath);
                DictionaryNode classNode = classesNode.FindChildNode(ClassInfoNodeWrapper.NameAttribute, typeName);
                Type type = ReflectionHelper.GetType(typeName);
                result.Add(classNode);
                foreach (DictionaryNode checkNode in classesNode.ChildNodes) {
                    Type checkType = ReflectionHelper.GetType(checkNode.GetAttributeValue("Name"));
                    if (checkType.IsSubclassOf(type) || type.IsSubclassOf(checkType)) {
                        result.Add(checkNode);
                    }
                }
            }
            return result;
        }
    }
}