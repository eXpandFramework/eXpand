using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Model;
using Xpand.Persistent.Base.General.Model;
using Xpand.Persistent.Base.Logic.Model;

namespace Xpand.ExpressApp.ModelArtifactState.ArtifactState.Model {
    [ModelAbstractClass]
    public interface IModelArtifactStateRule : IModelConditionalLogicRule {
        [Browsable(false)]
        IEnumerable<string> Modules { get; }
    }
    [DomainLogic(typeof(IModelArtifactStateRule))]
    public static class ArtifactStateRuleDomainLogic {
        public static IEnumerable<string> Get_Modules(IModelArtifactStateRule controllerStateRule) {
            return ((IModelApplicationModule)controllerStateRule.Application).ModulesList.Select(module => module.Name);
        }
    }

}
