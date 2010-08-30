using System.Collections.Generic;
using System.ComponentModel;
using DevExpress.ExpressApp.Model;
using eXpand.ExpressApp.ArtifactState.Model;
using eXpand.ExpressApp.ConditionalActionState.Logic;
using eXpand.ExpressApp.Logic.Conditional.Model;

namespace eXpand.ExpressApp.ConditionalActionState.Model {
    [ModelInterfaceImplementor(typeof (IActionStateRule), "Attribute")]
    public interface IModelActionStateRule : IActionStateRule, IModelConditionalLogicRule<IActionStateRule>,
                                                 IModelArtifactStateRule {

        [Browsable(false)]
        IEnumerable<string> Actions { get; }
    
    }
}