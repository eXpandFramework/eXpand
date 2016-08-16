using System.ComponentModel;
using System.Drawing;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Model.Core;
using DevExpress.ExpressApp.Updating;
using DevExpress.Utils;
using Xpand.ExpressApp.Logic;
using Xpand.ExpressApp.ModelArtifactState.ActionState;
using Xpand.ExpressApp.ModelArtifactState.ArtifactState.Model;
using Xpand.ExpressApp.ModelArtifactState.ControllerState;
using Xpand.ExpressApp.ModelArtifactState.ObjectViews;
using Xpand.ExpressApp.ModelArtifactState.ObjectViews.Model;
using Xpand.Persistent.Base.General;
using Xpand.Persistent.Base.Logic;
using Xpand.Persistent.Base.Security;

namespace Xpand.ExpressApp.ModelArtifactState {
    [ToolboxBitmap(typeof(ModelArtifactStateModule))]
    [ToolboxItem(true)]
    [ToolboxTabName(XpandAssemblyInfo.TabWinWebModules)]
    public class ModelArtifactStateModule : XpandModuleBase, IModelXmlConverter,ISecurityModuleUser {
        public ModelArtifactStateModule() {
            RequiredModuleTypes.Add(typeof(Validation.XpandValidationModule));
            LogicInstallerManager.RegisterInstallers(new ILogicInstaller[]{
                new ActionStateLogicInstaller(this),
                new ControllerStateLogicInstaller(this),
                new ObjectViewsLogicInstaller(this)
            });
        }

        public override void Setup(ApplicationModulesManager moduleManager){
            base.Setup(moduleManager);
            this.AddSecurityObjectsToAdditionalExportedTypes("Xpand.Persistent.BaseImpl.ModelArtifact");
        }

        public override void AddGeneratorUpdaters(ModelNodesGeneratorUpdaters updaters) {
            base.AddGeneratorUpdaters(updaters);
            updaters.Add(new ObjectViewActionExecutionContextsGroupNodeUpdater());
        }

        public override void ExtendModelInterfaces(ModelInterfaceExtenders extenders) {
            base.ExtendModelInterfaces(extenders);
            extenders.Add<IModelApplication,IModelApplicationModelArtifactState>();
            extenders.Add<IModelApplication, IModelApplicationConditionalObjectView>();
        }

        void IModelXmlConverter.ConvertXml(ConvertXmlParameters parameters) {
            ConvertXml(parameters);
            switch (parameters.XmlNodeName) {
                case "ConditionalDetailView":
                    parameters.NodeType = typeof(IModelLogicConditionalObjectView);
                    break;
                case "ConditionalDetailViewRule":
                    parameters.NodeType = typeof(IModelObjectViewRule);
                    break;
                case "ConditionalObjectViewRule":
                    parameters.NodeType = typeof(IModelObjectViewRule);
                    break;
            }
        }

    }
}