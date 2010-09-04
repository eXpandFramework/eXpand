using DevExpress.ExpressApp.Model;
using Xpand.ExpressApp.Logic.Conditional.Logic;

namespace Xpand.ExpressApp.ConditionalDetailViews.Logic
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
