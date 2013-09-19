using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Model;

namespace Xpand.ExpressApp.MapView {
    public sealed partial class MapViewModule : ModuleBase {
        public MapViewModule() {
            InitializeComponent();
        }
        public override void ExtendModelInterfaces(ModelInterfaceExtenders extenders) {
            base.ExtendModelInterfaces(extenders);
            extenders.Add<IModelClass,IModelClassMapView>();
            extenders.Add<IModelListView,IModelListViewMapView>();
        }
    }
}
