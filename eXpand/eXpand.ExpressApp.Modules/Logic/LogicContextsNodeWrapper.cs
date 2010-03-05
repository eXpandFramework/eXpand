using System;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.NodeWrappers;

namespace eXpand.ExpressApp.Logic {
    public class LogicContextsNodeWrapper : NodeWrapper {
        public const string CurrentGroupAttribute = "CurrentGroup";
        public const string ContextGroupAttribute = "ContextGroup";
        public const string ContextsAttribute = "Contexts";
        ExecutionContext _currentExecutionContext;

        public LogicContextsNodeWrapper(DictionaryNode node) : base(node) {
            CalculateCurrentExecutionContext();
        }

        public ExecutionContext CurrentExecutionContext {
            get { return _currentExecutionContext; }
        }

        public ReadOnlyDictionaryNodeCollection Contexts {
            get { return Node.ChildNodes; }
        }

        void CalculateCurrentExecutionContext() {
            string attributeValue = Node.GetAttributeValue(CurrentGroupAttribute);
            ReadOnlyDictionaryNodeCollection contextNodes =
                Node.GetChildNode(ContextGroupAttribute, "ID", attributeValue).ChildNodes;
            foreach (DictionaryNode childNode in contextNodes) {
                string value = childNode.GetAttributeValue("ID");
                _currentExecutionContext |= (ExecutionContext) Enum.Parse(typeof (ExecutionContext), value);
            }
        }
    }
}