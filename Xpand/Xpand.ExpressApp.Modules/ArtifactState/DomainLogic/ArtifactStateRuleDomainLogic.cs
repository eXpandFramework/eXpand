using System.Collections.Generic;
using System.Linq;
using DevExpress.ExpressApp.DC;
using Xpand.ExpressApp.ArtifactState.Model;
using Xpand.Persistent.Base.General.Model;

namespace Xpand.ExpressApp.ArtifactState.DomainLogic {
    [DomainLogic(typeof(IModelArtifactStateRule))]
    public static class ArtifactStateRuleDomainLogic {
        public static IEnumerable<string> Get_Modules(IModelArtifactStateRule controllerStateRule)
        {
            return ((IModelApplicationModule) controllerStateRule.Application).ModulesList.Select(module => module.Name);
        }
    }
}