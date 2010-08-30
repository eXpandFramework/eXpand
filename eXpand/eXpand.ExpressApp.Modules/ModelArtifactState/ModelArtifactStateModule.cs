using DevExpress.ExpressApp.Model;
using eXpand.ExpressApp.ArtifactState.Model;
using eXpand.ExpressApp.ConditionalActionState;
using eXpand.ExpressApp.ConditionalControllerState;

namespace eXpand.ExpressApp.ModelArtifactState {
    public class ModelArtifactStateModule:ModuleBase {
        public ModelArtifactStateModule() {
            RequiredModuleTypes.Add(typeof(ConditionalControllerStateModule));
            RequiredModuleTypes.Add(typeof(ConditionalActionStateModule));
        }
        public override void ExtendModelInterfaces(ModelInterfaceExtenders extenders)
        {
            base.ExtendModelInterfaces(extenders);
            extenders.Add<IModelApplication, IModelApplicationModelArtifactState>();
        }

    }
}