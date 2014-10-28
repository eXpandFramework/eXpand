using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Model;
using Xpand.ExpressApp.ModelArtifactState.ActionState.Logic;
using Xpand.ExpressApp.ModelArtifactState.ArtifactState.Model;
using Xpand.Persistent.Base.Logic.Model;

namespace Xpand.ExpressApp.ModelArtifactState.ActionState.Model {
    [ModelInterfaceImplementor(typeof(IContextActionStateRule), "Attribute")]
    public interface IModelActionStateRule : IContextActionStateRule, IModelConditionalLogicRule<IActionStateRule>,
                                                 IModelArtifactStateRule {

        [Browsable(false)]
        IEnumerable<string> Actions { get; }
        
        [Browsable(false)]
        IEnumerable<string> ActionContexts { get; }
    
    }
    [DomainLogic(typeof(IModelActionStateRule))]
    public static class ActionStateRuleDomainLogic {
        public static IEnumerable<string> Get_ActionContexts(IModelActionStateRule modelActionStateRule){
            return ((IModelApplicationModelArtifactState) modelActionStateRule.Application).ModelArtifactState.ConditionalActionState.ActionContexts.Select(contexts => contexts.Id);
        }

        public static IEnumerable<string> Get_Actions(IModelActionStateRule modelActionStateRule) {
            return modelActionStateRule.Application.ActionDesign.Actions.Select(action => action.Id);
        }
    }

}