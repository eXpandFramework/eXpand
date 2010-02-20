using System;
using DevExpress.ExpressApp;
using eXpand.ExpressApp.Logic;

namespace eXpand.ExpressApp.Logic.Conditional
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true)]
    public abstract class ConditionalLogicRuleAttribute:LogicAttribute, IConditionalLogicRule {
        protected ConditionalLogicRuleAttribute(string id, Nesting targetViewNesting, string normalCriteria, string emptyCriteria, ViewType viewType, string viewId) : base(id, targetViewNesting,  viewType, viewId) {
            NormalCriteria = normalCriteria;
            EmptyCriteria = emptyCriteria;
        }
        /// <summary>
        /// Criteria to apply when show DetailView or filled ListView 
        /// </summary>
        public string NormalCriteria { get; set; }
        /// <summary>
        /// Criteria to apply when show ListView empty
        /// </summary>
        public string EmptyCriteria { get; set; }

    }
}
