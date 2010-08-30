using System.ComponentModel;
using DevExpress.ExpressApp.Model;
using eXpand.ExpressApp.ConditionalDetailViews.Logic;
using eXpand.ExpressApp.Logic.Conditional.Model;

namespace eXpand.ExpressApp.ConditionalDetailViews.Model
{
    [ModelInterfaceImplementor(typeof(IConditionalDetailViewRule), "Attribute")]
    public interface IModelConditionalDetailViewRule : IConditionalDetailViewRule, IModelConditionalLogicRule<IConditionalDetailViewRule>
    {
        [Browsable(false)]
        IModelList<IModelDetailView> DetailViews { get; }
    }
}
