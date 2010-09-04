using System.Collections.Generic;
using System.Linq;
using DevExpress.ExpressApp.DC;
using eXpand.ExpressApp.ArtifactState.Model;
using eXpand.ExpressApp.SystemModule;

namespace eXpand.ExpressApp.ArtifactState.DomainLogic {
    [DomainLogic(typeof(IModelArtifactStateRule))]
    public static class ArtifactStateRuleDomainLogic {
        public static IEnumerable<string> Get_Modules(IModelArtifactStateRule controllerStateRule)
        {
            return ((IModelApplicationModule) controllerStateRule.Application).ModulesList.Select(module => module.Name);
        }
    }
}