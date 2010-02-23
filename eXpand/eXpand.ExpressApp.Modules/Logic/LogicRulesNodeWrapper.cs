using System;
using System.Collections.Generic;
using System.Linq;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.NodeWrappers;
using eXpand.Persistent.Base.General;

namespace eXpand.ExpressApp.Logic {
    public abstract class LogicRulesNodeWrapper<TLogicRule> : NodeWrapper where TLogicRule:ILogicRule
    {
        protected LogicRulesNodeWrapper(DictionaryNode dictionaryNode) : base(dictionaryNode) {
        }


        protected abstract string ChildNodeName { get; }

        public List<TLogicRule> Rules{
            get { return GetRules(); }
        }

        internal TypesInfo TypesInfo { get; set; }


        public virtual TLogicRule AddRule(TLogicRule logicRuleAttribute, ITypeInfo typeInfo, Type logicRuleNodeWrapperType)
        {
            
            var artifactStateRuleNodeWrapper =
                (TLogicRule)
                Activator.CreateInstance(logicRuleNodeWrapperType, new[] { Node.AddChildNode(ChildNodeName) });
            artifactStateRuleNodeWrapper.ViewType = logicRuleAttribute.ViewType;
            artifactStateRuleNodeWrapper.Nesting = logicRuleAttribute.Nesting;
            artifactStateRuleNodeWrapper.Nesting = logicRuleAttribute.Nesting;
            artifactStateRuleNodeWrapper.ID = logicRuleAttribute.ID;
            artifactStateRuleNodeWrapper.TypeInfo = typeInfo;
            artifactStateRuleNodeWrapper.Description = logicRuleAttribute.Description;
            artifactStateRuleNodeWrapper.ViewId = logicRuleAttribute.ViewId;
            return artifactStateRuleNodeWrapper;
        }


        public IEnumerable<TLogicRule> FindRules(ITypeInfo typeInfo)
        {
            if (typeInfo != null) {
                foreach (TLogicRule rule in Rules.Where(rule => rule.TypeInfo == typeInfo)){
                    yield return rule;
                }
            }
        }

        protected virtual List<TLogicRule> GetRules()
        {
//            var single = AppDomain.CurrentDomain.GetTypes(typeof(LogicRuleNodeWrapper)).Where(type => !(type.IsAbstract)&&typeof(TLogicRule).IsAssignableFrom(type)).Single();
            return
                Node.ChildNodes.GetOrderedByIndex().Select(node =>(TLogicRule)Activator.CreateInstance(TypesInfo.LogicRuleNodeWrapperType, new[] { node })).ToList();
        }
    }
}