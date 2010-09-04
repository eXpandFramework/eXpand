using DevExpress.ExpressApp;
using Xpand.ExpressApp.IO.Observers;
using Xpand.Persistent.Base.ImportExport;

namespace Xpand.ExpressApp.IO.Controllers {
    public class SerializationConfigurationDetailViewController:ViewController<XpandDetailView>
    {
        public SerializationConfigurationDetailViewController() {
            TargetObjectType = typeof (ISerializationConfiguration);
        }
        protected override void OnActivated()
        {
            base.OnActivated();
            new SerializationConfigurationObserver(ObjectSpace);
        }
    }
}