using DevExpress.ExpressApp;
using eXpand.ExpressApp.ImportExport.Observers;
using eXpand.Persistent.Base.ImportExport;

namespace eXpand.ExpressApp.ImportExport.Controllers.DetailView
{
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
