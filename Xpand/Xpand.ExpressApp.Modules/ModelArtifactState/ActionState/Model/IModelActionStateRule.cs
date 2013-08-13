using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Model;
using Xpand.ExpressApp.ModelArtifactState.ActionState.Logic;
using Xpand.ExpressApp.ModelArtifactState.ArtifactState.Model;
using Xpand.Persistent.Base.Logic.Model;

namespace Xpand.ExpressApp.ModelArtifactState.ActionState.Model {
    [ModelInterfaceImplementor(typeof (IActionStateRule), "Attribute")]
    public interface IModelActionStateRule : IActionStateRule, IModelConditionalLogicRule<IActionStateRule>,
                                                 IModelArtifactStateRule {

        [Browsable(false)]
        IEnumerable<string> Actions { get; }
    
    }
    [DomainLogic(typeof(IModelActionStateRule))]
    public static class ActionStateRuleDomainLogic {
        public static IEnumerable<string> Get_Actions(IModelActionStateRule modelActionStateRule) {
            return modelActionStateRule.Application.ActionDesign.Actions.Select(action => action.Id);
        }
    }

}