using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Model.Core;
using eXpand.ExpressApp.Logic.Model;
using eXpand.ExpressApp.Logic.NodeGenerators;
using eXpand.Utils.Helpers;

namespace eXpand.ExpressApp.Logic.NodeUpdaters {
    public abstract class LogicRulesNodeUpdater<TLogicRule, TModelLogicRule, TRootModelNode> :
        ModelNodesGeneratorUpdater<LogicRulesNodesGenerator>
        where TLogicRule : ILogicRule
        where TModelLogicRule : IModelLogicRule
        where TRootModelNode : IModelNode
    {

        void AddRules(ModelNode node, IEnumerable<TLogicRule> attributes, IModelClass modelClass) {
            foreach (TLogicRule attribute in attributes) {
                var rule = node.AddNode<TModelLogicRule>(attribute.Id);
                ((IModelNode) rule).Index = attribute.Index;
                rule.ModelClass = modelClass;
                rule.TypeInfo = modelClass.TypeInfo;
                SetAttribute(rule, attribute);
            }
        }

        protected abstract void SetAttribute(TModelLogicRule rule, TLogicRule attribute);


        public override void UpdateNode(ModelNode node) {
            TRootModelNode rootModelNode = default(TRootModelNode);
            var propertyName = rootModelNode.GetPropertyName(ExecuteExpression());
            if (node.Parent.Id == propertyName) {
                foreach (IModelClass modelClass in node.Application.BOModel) {
                    var findAttributes = LogicRuleManager<TLogicRule>.FindAttributes(modelClass.TypeInfo);
                    AddRules(node, findAttributes, modelClass);
                }
            }
        }
        protected abstract Expression<Func<TRootModelNode, object>> ExecuteExpression();
    }
}