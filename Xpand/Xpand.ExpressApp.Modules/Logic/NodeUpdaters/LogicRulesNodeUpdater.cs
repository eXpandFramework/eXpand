using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq.Expressions;
using System.Reflection;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Model.Core;
using DevExpress.Persistent.Base;
using Xpand.ExpressApp.Logic.Model;
using Xpand.ExpressApp.Logic.NodeGenerators;

using Xpand.Utils;
using Xpand.Utils.Helpers;

namespace Xpand.ExpressApp.Logic.NodeUpdaters {
    public abstract class LogicRulesNodeUpdater<TLogicRule, TModelLogicRule, TRootModelNode> :
        ModelNodesGeneratorUpdater<LogicRulesNodesGenerator>
        where TLogicRule : ILogicRule
        where TModelLogicRule : IModelLogicRule
        where TRootModelNode : IModelNode {
        IEnumerable<PropertyInfo> _explicitProperties;

        void AddRules(ModelNode node, IEnumerable<TLogicRule> attributes, IModelClass modelClass) {
            foreach (TLogicRule attribute in attributes) {
                var rule = node.AddNode<TModelLogicRule>(attribute.Id);
                SetAttribute(rule, attribute);
                ((IModelNode)rule).Index = attribute.Index;
                rule.ModelClass = modelClass;
                rule.TypeInfo = modelClass.TypeInfo;
                ConvertModelNodes(attribute, rule);
            }
        }


        void ConvertModelNodes(TLogicRule attribute, TModelLogicRule rule) {
            if (_explicitProperties == null)
                _explicitProperties = XpandReflectionHelper.GetExplicitProperties(attribute.GetType());
            foreach (PropertyInfo explicitProperty in _explicitProperties) {
                object[] customAttributes = explicitProperty.GetCustomAttributes(typeof(TypeConverterAttribute), false);
                if (customAttributes.Length > 0) {
                    var converter = (TypeConverter)ReflectionHelper.CreateObject(Type.GetType(((TypeConverterAttribute)customAttributes[0]).ConverterTypeName), new object[] { rule.Application });
                    string name = explicitProperty.Name.Substring(explicitProperty.Name.LastIndexOf(".") + 1);
                    PropertyInfo propertyInfo = attribute.GetType().GetProperty(name);
                    object value = propertyInfo.GetValue(attribute, null);
                    if (value != null) {
                        object convertTo = converter.ConvertTo(value, rule.TypeInfo.Type);
                        explicitProperty.SetValue(attribute, convertTo, null);
                    }
                }
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