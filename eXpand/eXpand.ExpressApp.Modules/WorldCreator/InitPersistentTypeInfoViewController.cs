using DevExpress.ExpressApp;
using eXpand.ExpressApp.SystemModule;
using eXpand.ExpressApp.WorldCreator.Core;
using eXpand.Persistent.Base.PersistentMetaData;

namespace eXpand.ExpressApp.WorldCreator
{
    public class InitPersistentTypeInfoViewController : ViewController 
    {
        protected override void OnActivated()
        {
            base.OnActivated();
            Frame.GetController<TrackObjectConstructionsViewController>().ObjectCreated+=OnObjectCreated;
        }

        void OnObjectCreated(object sender, ObjectCreatedEventArgs objectCreatedEventArgs) {
            if (objectCreatedEventArgs.Object as IPersistentTypeInfo != null){
                var persistentClassInfo = ((IPersistentTypeInfo)objectCreatedEventArgs.Object);
                var worldCreatorModule= ((WorldCreatorModule) Application.Modules.FindModule(typeof (WorldCreatorModule)));
                persistentClassInfo.Init(worldCreatorModule.TypesInfo.CodeTemplateType);
            }
        }

        protected override void OnDeactivating()
        {
            base.OnDeactivating();
            Frame.GetController<TrackObjectConstructionsViewController>().ObjectCreated -= OnObjectCreated;
        }
    }
}
