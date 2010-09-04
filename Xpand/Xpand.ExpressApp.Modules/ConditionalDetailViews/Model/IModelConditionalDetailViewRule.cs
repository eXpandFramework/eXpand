using System.ComponentModel;
using DevExpress.ExpressApp.Model;
using Xpand.ExpressApp.ConditionalDetailViews.Logic;
using Xpand.ExpressApp.Logic.Conditional.Model;

namespace Xpand.ExpressApp.ConditionalDetailViews.Model
{
    [ModelInterfaceImplementor(typeof(IConditionalDetailViewRule), "Attribute")]
    public interface IModelConditionalDetailViewRule : IConditionalDetailViewRule, IModelConditionalLogicRule<IConditionalDetailViewRule>
    {
        [Browsable(false)]
        IModelList<IModelDetailView> DetailViews { get; }
    }
}
