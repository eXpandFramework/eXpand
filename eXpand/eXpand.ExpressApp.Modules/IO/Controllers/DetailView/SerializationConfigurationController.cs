using DevExpress.ExpressApp;
using eXpand.ExpressApp.IO.Observers;
using eXpand.Persistent.Base.ImportExport;

namespace eXpand.ExpressApp.IO.Controllers.DetailView {
    public class SerializationConfigurationController:ViewController<DevExpress.ExpressApp.DetailView>
    {
        public SerializationConfigurationController() {
            TargetObjectType = typeof (ISerializationConfiguration);
        }
        protected override void OnActivated()
        {
            base.OnActivated();
            new SerializationConfigurationObserver(ObjectSpace);
        }
    }
}