using System.ComponentModel;
using DevExpress.ExpressApp.Model;
using Xpand.ExpressApp.ConditionalObjectView.Logic;
using Xpand.ExpressApp.Logic.Conditional.Model;
using Xpand.Persistent.Base.General;

namespace Xpand.ExpressApp.ConditionalObjectView.Model {
    [ModelInterfaceImplementor(typeof(IConditionalObjectViewRule), "Attribute")]
    [ModelEditorLogicRule(typeof(IModelLogicConditionalObjectView))]
    public interface IModelConditionalObjectViewRule : IConditionalObjectViewRule, IModelConditionalLogicRule<IConditionalObjectViewRule> {
        [Browsable(false)]
        IModelList<IModelObjectView> ObjectViews { get; }
    }
}
