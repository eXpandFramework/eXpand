using System.ComponentModel;
using DevExpress.ExpressApp.Model;
using Xpand.ExpressApp.Logic.Conditional.Logic;
using Xpand.ExpressApp.Logic.TypeConverters;

namespace Xpand.ExpressApp.ConditionalObjectView.Logic {
    public class ConditionalObjectViewRuleAttribute : ConditionalLogicRuleAttribute, IConditionalObjectViewRule {
        public ConditionalObjectViewRuleAttribute(string id, string normalCriteria, string emptyCriteria, string detailView)
            : base(id, normalCriteria, emptyCriteria) {
            ObjectView = detailView;
        }

        public string ObjectView { get; set; }
        [TypeConverter(typeof(StringToModelViewConverter))]
        IModelObjectView IConditionalObjectViewRule.ObjectView { get; set; }
    }
}
