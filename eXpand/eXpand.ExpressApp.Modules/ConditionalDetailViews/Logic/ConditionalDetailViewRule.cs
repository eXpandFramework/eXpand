using DevExpress.ExpressApp.Model;
using eXpand.ExpressApp.Logic.Conditional.Logic;

namespace eXpand.ExpressApp.ConditionalDetailViews.Logic
{
    public class ConditionalDetailViewRule :ConditionalLogicRule, IConditionalDetailViewRule
    {
        public ConditionalDetailViewRule(IConditionalDetailViewRule conditionalDetailViewRule)
            : base(conditionalDetailViewRule)
        {
            DetailView=conditionalDetailViewRule.DetailView;
        }

        public IModelDetailView DetailView{get; set;}
    }
}
