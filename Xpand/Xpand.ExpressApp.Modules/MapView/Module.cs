using DevExpress.ExpressApp.Model;
using Xpand.Persistent.Base.General;

namespace Xpand.ExpressApp.MapView {
    public sealed class MapViewModule : XpandModuleBase {
        public override void ExtendModelInterfaces(ModelInterfaceExtenders extenders) {
            base.ExtendModelInterfaces(extenders);
            extenders.Add<IModelClass,IModelClassMapView>();
            extenders.Add<IModelListView,IModelListViewMapView>();
        }
    }
}
