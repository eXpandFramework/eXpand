using System;
using System.Collections.Generic;
using DevExpress.Data.Filtering;
using DevExpress.Data.Filtering.Helpers;
using DevExpress.ExpressApp;
using eXpand.ExpressApp.Security.NodeWrappers;
using ListView=DevExpress.ExpressApp.ListView;
using View=DevExpress.ExpressApp.View;

namespace XAFPoint.ExpressApp.ModelArtifactState.Calculators
{
    public class DictionaryActivationCalculator
    {
        private static readonly DictionaryActivationCalculator singletonInstance;

        public struct DictionaryNodeInfo
        {
            public DictionaryNodeInfo(DictionaryNode dictionaryNode, string nodeName) : this()
            {
                DictionaryNode = dictionaryNode;
                NodeName = nodeName;
            }

            public DictionaryNode DictionaryNode { get; set; }
            public string NodeName { get; set; }
        }
        private readonly Dictionary<DictionaryNodeInfo, List<DictionaryActivationNodeWrapperBase>> attributesCore = new Dictionary<DictionaryNodeInfo, List<DictionaryActivationNodeWrapperBase>>();

        static DictionaryActivationCalculator()
        {
            singletonInstance = new DictionaryActivationCalculator();
        }
        public static DictionaryActivationCalculator Instance
        {
            get { return singletonInstance; }
        }
        public List<DictionaryActivationNodeWrapperBase> this[DictionaryNode dictionaryNode, string childNodeName,DictionaryActivationNodeWrapperDelegate wrapperDelegate]
        {
            get
            {
                lock (new object())
                {
                    var dictionaryNodeInfo = new DictionaryNodeInfo(dictionaryNode, childNodeName);
                    if (!attributesCore.ContainsKey(dictionaryNodeInfo))
                        attributesCore.Add(dictionaryNodeInfo, 
                                           FindActivationWrappersAttributes(dictionaryNode,childNodeName,wrapperDelegate));
                    return attributesCore[dictionaryNodeInfo];
                }
            }
        }

        public delegate DictionaryActivationNodeWrapperBase DictionaryActivationNodeWrapperDelegate(DictionaryNode dictionaryNode);
        private List<DictionaryActivationNodeWrapperBase> FindActivationWrappersAttributes(
            DictionaryNode dictionaryNode, string childNodeName,DictionaryActivationNodeWrapperDelegate wrapperDelegate)
        {
            var dictionaryActivationNodeWrappers = new List<DictionaryActivationNodeWrapperBase>();
            foreach (var childNode in dictionaryNode.GetChildNode(childNodeName).ChildNodes)
                dictionaryActivationNodeWrappers.Add(wrapperDelegate.Invoke(childNode));
            return dictionaryActivationNodeWrappers;
        }

        public static bool IsActive(object targetObject, string criteria)
        {
            if (string.IsNullOrEmpty(criteria))
                return true;
            return
                new ExpressionEvaluator(new EvaluatorContextDescriptorDefault(targetObject.GetType()),
                                        CriteriaOperator.Parse(criteria)).Fit(targetObject);
        }

        public static void ActivationCalculated(ViewController viewController, Type actiovationRuleAttributeType,
                                                string childNodeName,
                                                Action<DictionaryActivationNodeWrapperBase, bool> action,DictionaryActivationNodeWrapperDelegate wrapperDelegate)
        {
            View view = viewController.View;
            foreach (DictionaryActivationNodeWrapperBase nodeWrapper in
                Instance[
                    view.Info,  childNodeName,wrapperDelegate])
            {
                listViewActivationCalculated(view, nodeWrapper, action);
                detailViewActivationCalculated(view, nodeWrapper, action);
            }
        }

        private static void detailViewActivationCalculated(View view, DictionaryActivationNodeWrapperBase wrapper, Action<DictionaryActivationNodeWrapperBase, bool> action)
        {
            if (view.CurrentObject != null)
            {
                bool isActive = IsActive(view.CurrentObject, wrapper.NormalCriteria);
                action.Invoke(wrapper, isActive);
            }
        }

        private static void listViewActivationCalculated(View view, DictionaryActivationNodeWrapperBase nodeWrapper, Action<DictionaryActivationNodeWrapperBase, bool> action)
        {
            if (view is ListView)
            {
                if ((nodeWrapper.Nesting == Nesting.Root && view.IsRoot) ||
                    (nodeWrapper.Nesting == Nesting.Nested && view.IsRoot == false) ||
                    (nodeWrapper.Nesting == Nesting.Any))
                {
                    bool isActive = IsActive(view.CurrentObject??new object(),
                                             view.CurrentObject == null
                                                 ? nodeWrapper.EmptyCriteria
                                                 : nodeWrapper.NormalCriteria);
                    action.Invoke(nodeWrapper, isActive);
                }
            }
        }
    }
}