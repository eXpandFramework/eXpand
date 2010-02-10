using System;
using System.Collections.Generic;
using System.Linq;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.NodeWrappers;

namespace eXpand.ExpressApp.RuleModeller {
    public abstract class ModelRulesNodeWrapper<TModelRuleWrapper,TModelRuleAttribute> : NodeWrapper
        where TModelRuleWrapper : ModelRuleNodeWrapper
        where TModelRuleAttribute : ModelRuleAttribute
    {
        protected ModelRulesNodeWrapper(DictionaryNode dictionaryNode) : base(dictionaryNode) {
        }


        protected abstract string ChildNodeName { get; }

        public List<TModelRuleWrapper> Rules {
            get { return GetRules(); }
        }

        public virtual TModelRuleWrapper AddRule(TModelRuleAttribute modelRuleAttribute, ITypeInfo typeInfo){
            
            var artifactStateRuleNodeWrapper =
                (TModelRuleWrapper)
                Activator.CreateInstance(typeof(TModelRuleWrapper), new[] { Node.AddChildNode(ChildNodeName) });
            artifactStateRuleNodeWrapper.EmptyCriteria = modelRuleAttribute.EmptyCriteria;
            artifactStateRuleNodeWrapper.NormalCriteria = modelRuleAttribute.NormalCriteria;
            artifactStateRuleNodeWrapper.ViewType = modelRuleAttribute.ViewType;
            artifactStateRuleNodeWrapper.Nesting = modelRuleAttribute.Nesting;
            artifactStateRuleNodeWrapper.Nesting = modelRuleAttribute.Nesting;
            artifactStateRuleNodeWrapper.ID = modelRuleAttribute.ID;
            artifactStateRuleNodeWrapper.TypeInfo = typeInfo;
            artifactStateRuleNodeWrapper.Description = modelRuleAttribute.Description;
            artifactStateRuleNodeWrapper.ViewId = modelRuleAttribute.ViewId;
            return artifactStateRuleNodeWrapper;
        }


        public IEnumerable<TModelRuleWrapper> FindRules(ITypeInfo typeInfo) {
            if (typeInfo != null) {
                foreach (TModelRuleWrapper rule in Rules.Where(rule => rule.TypeInfo == typeInfo)) {
                    yield return rule;
                }
            }
        }

        protected virtual List<TModelRuleWrapper> GetRules(){
            return
                Node.ChildNodes.GetOrderedByIndex().Select(
                    node =>
                    (TModelRuleWrapper)
                    Activator.CreateInstance(typeof(TModelRuleWrapper), new[] { node })).ToList();
        }
    }
}