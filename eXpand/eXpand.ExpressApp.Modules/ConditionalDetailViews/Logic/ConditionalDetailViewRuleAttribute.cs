using System.ComponentModel;
using DevExpress.ExpressApp.Model;
using eXpand.ExpressApp.Logic.Conditional.Logic;
using eXpand.ExpressApp.Logic.TypeConverters;

namespace eXpand.ExpressApp.ConditionalDetailViews.Logic
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
