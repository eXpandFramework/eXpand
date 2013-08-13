using System.ComponentModel;
using System.Drawing;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Updating;
using DevExpress.Utils;
using Xpand.ExpressApp.Logic;
using Xpand.ExpressApp.ModelArtifactState.ActionState;
using Xpand.ExpressApp.ModelArtifactState.ArtifactState.Model;
using Xpand.ExpressApp.ModelArtifactState.ControllerState;
using Xpand.ExpressApp.ModelArtifactState.ObjectViews;
using Xpand.ExpressApp.ModelArtifactState.ObjectViews.Model;
using Xpand.Persistent.Base.General;

namespace Xpand.ExpressApp.ModelArtifactState {
    [ToolboxBitmap(typeof(ModelArtifactStateModule))]
    [ToolboxItem(true)]
    [ToolboxTabName(XpandAssemblyInfo.TabWinWebModules)]
    public class ModelArtifactStateModule : XpandModuleBase, IModelXmlConverter {
        public ModelArtifactStateModule() {
            var logicInstallerManager = LogicInstallerManager.Instance;
            logicInstallerManager.RegisterInstallers(new ILogicInstaller[]{
                new ActionStateLogicInstaller(this),
                new ControllerStateLogicInstaller(this),
                new ObjectViewsLogicInstaller(this)
            });
        }

        public override void ExtendModelInterfaces(ModelInterfaceExtenders extenders) {
            base.ExtendModelInterfaces(extenders);
            extenders.Add<IModelApplication,IModelApplicationModelArtifactState>();
            extenders.Add<IModelApplication, IModelApplicationConditionalObjectView>();
        }

        void IModelXmlConverter.ConvertXml(ConvertXmlParameters parameters) {
            ConvertXml(parameters);
            if (parameters.XmlNodeName == "ConditionalDetailView") {
                parameters.NodeType = typeof(IModelLogicConditionalObjectView);
            } else if (parameters.XmlNodeName == "ConditionalDetailViewRule") {
                parameters.NodeType = typeof(IModelObjectViewRule);
            }
        }

    }
}