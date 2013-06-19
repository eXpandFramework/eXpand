using System.ComponentModel;
using DevExpress.ExpressApp.Model;
using Xpand.ExpressApp.ConditionalObjectView.Logic;
using Xpand.ExpressApp.Logic.Conditional.Model;

namespace Xpand.ExpressApp.ConditionalObjectView.Model {
    [ModelInterfaceImplementor(typeof(IConditionalObjectViewRule), "Attribute")]
    public interface IModelConditionalObjectViewRule : IConditionalObjectViewRule, IModelConditionalLogicRule<IConditionalObjectViewRule> {
        [Browsable(false)]
        IModelList<IModelObjectView> ObjectViews { get; }
    }
}
