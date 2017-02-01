using System.ComponentModel;
using System.Drawing;
using DevExpress.Utils;

namespace Xpand.ExpressApp.ModelDifference.Web {
    [ToolboxBitmap(typeof(ModelDifferenceAspNetModule))]
    [ToolboxItem(true)]
    [ToolboxTabName(XpandAssemblyInfo.TabAspNetModules)]
    public sealed class ModelDifferenceAspNetModule : ModelDifferenceBaseModule {

        public ModelDifferenceAspNetModule() {
            RequiredModuleTypes.Add(typeof(DevExpress.ExpressApp.Web.SystemModule.SystemAspNetModule));
            RequiredModuleTypes.Add(typeof(ModelDifferenceModule));
        }

    }
}