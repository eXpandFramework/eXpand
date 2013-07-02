using System.ComponentModel;
using DevExpress.ExpressApp.Model;
using Xpand.ExpressApp.ConditionalObjectView.Logic;
using Xpand.Persistent.Base.General;
using Xpand.Persistent.Base.Logic.Model;

namespace Xpand.ExpressApp.ConditionalObjectView.Model {
    [ModelInterfaceImplementor(typeof(IConditionalObjectViewRule), "Attribute")]
    [ModelEditorLogicRule(typeof(IModelLogicConditionalObjectView))]
    public interface IModelConditionalObjectViewRule : IConditionalObjectViewRule, IModelConditionalLogicRule<IConditionalObjectViewRule> {
        [Browsable(false)]
        IModelList<IModelObjectView> ObjectViews { get; }
    }
}
