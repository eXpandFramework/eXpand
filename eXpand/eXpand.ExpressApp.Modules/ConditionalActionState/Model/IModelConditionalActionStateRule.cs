using System.Collections.Generic;
using System.ComponentModel;
using DevExpress.ExpressApp.Model;
using eXpand.ExpressApp.ConditionalActionState.Logic;
using eXpand.ExpressApp.Logic.Conditional.Model;

namespace eXpand.ExpressApp.ConditionalActionState.Model {
    [ModelInterfaceImplementor(typeof (IConditionalActionStateRule), "Attribute")]
    public interface IModelConditionalActionStateRule : IConditionalActionStateRule, IModelConditionalLogicRule<IConditionalActionStateRule>
    {

        [Browsable(false)]
        IEnumerable<string> Actions { get; }
        [Browsable(false)]
        IEnumerable<string> Modules { get; }
    }
}