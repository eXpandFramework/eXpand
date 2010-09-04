using System.ComponentModel;
using DevExpress.ExpressApp.Model;
using Xpand.ExpressApp.Logic.Conditional.Logic;
using Xpand.ExpressApp.Logic.TypeConverters;

namespace Xpand.ExpressApp.ConditionalDetailViews.Logic
{
    public class ConditionalDetailViewRuleAttribute : ConditionalLogicRuleAttribute, IConditionalDetailViewRule
    {
        public ConditionalDetailViewRuleAttribute(string id, string normalCriteria, string emptyCriteria, string detailView) : base(id, normalCriteria, emptyCriteria) {
            DetailView=detailView;
        }

        public string DetailView { get; set; }
        [TypeConverter(typeof(StringToModelViewConverter))]
        IModelDetailView IConditionalDetailViewRule.DetailView {get ; set ; }
    }
}
