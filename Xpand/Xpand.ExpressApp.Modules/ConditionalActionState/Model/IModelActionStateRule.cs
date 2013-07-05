using System.Collections.Generic;
using System.ComponentModel;
using DevExpress.ExpressApp.Model;
using Xpand.ExpressApp.ArtifactState.Model;
using Xpand.ExpressApp.ConditionalActionState.Logic;
using Xpand.Persistent.Base.Logic.Model;

namespace Xpand.ExpressApp.ConditionalActionState.Model {
    [ModelInterfaceImplementor(typeof (IActionStateRule), "Attribute")]
    public interface IModelActionStateRule : IActionStateRule, IModelConditionalLogicRule<IActionStateRule>,
                                                 IModelArtifactStateRule {

        [Browsable(false)]
        IEnumerable<string> Actions { get; }
    
    }
}