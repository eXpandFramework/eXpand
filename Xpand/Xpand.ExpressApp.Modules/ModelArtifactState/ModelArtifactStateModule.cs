using System.ComponentModel;
using System.Drawing;
using DevExpress.Utils;
using Xpand.ExpressApp.ConditionalActionState;
using Xpand.ExpressApp.ConditionalControllerState;

namespace Xpand.ExpressApp.ModelArtifactState {
    [ToolboxBitmap(typeof(ModelArtifactStateModule))]
    [ToolboxItem(true)]
    [ToolboxTabName(XpandAssemblyInfo.TabWinWebModules)]
    public class ModelArtifactStateModule : XpandModuleBase {
        public ModelArtifactStateModule() {
            RequiredModuleTypes.Add(typeof(ConditionalControllerStateModule));
            RequiredModuleTypes.Add(typeof(ConditionalActionStateModule));
        }
    }
}