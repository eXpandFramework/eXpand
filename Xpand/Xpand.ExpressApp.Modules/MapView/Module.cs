using System.ComponentModel;
using DevExpress.ExpressApp.Model;
using DevExpress.Utils;
using Xpand.Persistent.Base.General;

namespace Xpand.ExpressApp.MapView {
    [ToolboxItem(true)]
    [ToolboxTabName(XpandAssemblyInfo.TabWinWebModules)]
    public sealed class MapViewModule : XpandModuleBase {
        public override void ExtendModelInterfaces(ModelInterfaceExtenders extenders) {
            base.ExtendModelInterfaces(extenders);
            extenders.Add<IModelClass,IModelClassMapView>();
            extenders.Add<IModelListView,IModelListViewMapView>();
        }
    }
}
