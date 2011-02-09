using DevExpress.Persistent.Base;
using Xpand.ExpressApp.Attributes;
using Xpand.ExpressApp.Logic.Conditional.Security;

namespace Xpand.ExpressApp.Logic.Conditional.Logic {
    public abstract class ConditionalLogicRuleViewController<TConditionalLogicRule> :
        LogicRuleViewController<TConditionalLogicRule> where TConditionalLogicRule : IConditionalLogicRule{
        protected override LogicRuleInfo<TConditionalLogicRule> CalculateLogicRuleInfo(object targetObject,TConditionalLogicRule logicRule) {
            LogicRuleInfo<TConditionalLogicRule> calculateLogicRuleInfo = base.CalculateLogicRuleInfo(targetObject,logicRule);
            ConditionalLogicRuleManager<TConditionalLogicRule>.CalculateLogicRuleInfo(calculateLogicRuleInfo);
            return calculateLogicRuleInfo;
        }
        public override void CustomizeTypesInfo(DevExpress.ExpressApp.DC.ITypesInfo typesInfo) {
            base.CustomizeTypesInfo(typesInfo);
            var typeDescendants = ReflectionHelper.FindTypeDescendants(typesInfo.FindTypeInfo(typeof(ConditionalLogicRulePermission)));
            foreach (var typeInfo in typeDescendants) {
                typeInfo.AddAttribute(new NewObjectCreateGroupAttribute("Conditional"));
            }
        }
    }
}