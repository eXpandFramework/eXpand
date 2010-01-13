using DevExpress.ExpressApp;
using eXpand.ExpressApp.IO.Observers;
using eXpand.Persistent.Base.ImportExport;

namespace eXpand.ExpressApp.IO.Controllers {
    public class SerializationConfigurationDetailViewController:ViewController<DetailView>
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