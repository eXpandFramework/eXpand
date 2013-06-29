using System;
using System.Collections.Generic;
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.DC;
using DevExpress.Persistent.Base;
using Xpand.ExpressApp.Attributes;
using Xpand.ExpressApp.Logic.Conditional.Security;

namespace Xpand.ExpressApp.Logic.Conditional.Logic {
    public abstract class ConditionalLogicRuleViewController<TConditionalLogicRule, TModule> :
        LogicRuleViewController<TConditionalLogicRule,TModule> where TConditionalLogicRule : IConditionalLogicRule where TModule : XpandModuleBase, ILogicModuleBase {
        protected override LogicRuleInfo<TConditionalLogicRule> CalculateLogicRuleInfo(object targetObject, TConditionalLogicRule logicRule, ActionBase action) {
            LogicRuleInfo<TConditionalLogicRule> calculateLogicRuleInfo = base.CalculateLogicRuleInfo(targetObject, logicRule, action);
            ConditionalLogicRuleManager<TConditionalLogicRule>.CalculateLogicRuleInfo(calculateLogicRuleInfo);
            return calculateLogicRuleInfo;
        }
        public override void CustomizeTypesInfo(ITypesInfo typesInfo) {
            base.CustomizeTypesInfo(typesInfo);
            AddNewObjectCreateGroup(typesInfo, new List<Type> { typeof(ConditionalLogicRulePermission), typeof(Security.Improved.ConditionalLogicOperationPermissionData) });
        }

        void AddNewObjectCreateGroup(ITypesInfo typesInfo, IEnumerable<Type> types) {
            foreach (var type in types) {
                var typeDescendants = ReflectionHelper.FindTypeDescendants(typesInfo.FindTypeInfo(type));
                foreach (var typeInfo in typeDescendants) {
                    typeInfo.AddAttribute(new NewObjectCreateGroupAttribute("Conditional"));
                }
            }
        }
    }
}