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

        TypesInfo _typesInfo;
        internal TypesInfo TypesInfo {
            get {
                if (_typesInfo== null) {
                    _typesInfo = new TypesInfo();
                    _typesInfo.RegisterTypes<TLogicRule>();
                }
                return _typesInfo;
            }
            set { _typesInfo = value; }
        }


        public virtual TLogicRule AddRule(TLogicRule logicRuleAttribute, ITypeInfo typeInfo, Type logicRuleNodeWrapperType)
        {
            
            var ruleNodeWrapper =
                (TLogicRule)
                Activator.CreateInstance(logicRuleNodeWrapperType, new[] { Node.AddChildNode(ChildNodeName) });
            ruleNodeWrapper.ViewType = logicRuleAttribute.ViewType;
            ruleNodeWrapper.Nesting = logicRuleAttribute.Nesting;
            ruleNodeWrapper.Nesting = logicRuleAttribute.Nesting;
            ruleNodeWrapper.ID = logicRuleAttribute.ID;
            ruleNodeWrapper.TypeInfo = typeInfo;
            ruleNodeWrapper.Description = logicRuleAttribute.Description;
            ruleNodeWrapper.ViewId = logicRuleAttribute.ViewId;
            ruleNodeWrapper.Index = logicRuleAttribute.Index;
            ruleNodeWrapper.ExecutionContextGroup = logicRuleAttribute.ExecutionContextGroup;            
            return ruleNodeWrapper;
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