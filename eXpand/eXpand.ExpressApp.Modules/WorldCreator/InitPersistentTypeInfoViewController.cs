using DevExpress.ExpressApp;
using eXpand.Persistent.Base.PersistentMetaData;
using eXpand.Utils.Helpers;

namespace eXpand.ExpressApp.WorldCreator
{
    public class InitPersistentTypeInfoViewController : ViewController 
    {
        public InitPersistentTypeInfoViewController()
        {
            TargetObjectType = typeof (IPersistentTypeInfo);
        }
        protected override void OnActivated()
        {
            base.OnActivated();
            Frame.GetController<TrackObjectConstructionsViewController>().ObjectCreated+=OnObjectCreated;
        }

        void OnObjectCreated(object sender, ObjectCreatedEventArgs objectCreatedEventArgs) {
            if (objectCreatedEventArgs.Object as IPersistentTypeInfo != null){
                var persistentClassInfo = ((IPersistentTypeInfo)objectCreatedEventArgs.Object);
                var worldCreatorModule= ((WorldCreatorModule) Application.Modules.FindModule(typeof (WorldCreatorModule)));
                var codeTemplateType = worldCreatorModule.TypesInfo.PersistentTypesInfoType.GetProperty(persistentClassInfo.GetPropertyName(x=>x.CodeTemplate)).PropertyType;
                if (codeTemplateType.IsGenericType)
                    codeTemplateType = codeTemplateType.GetGenericArguments()[0];
                persistentClassInfo.Init(codeTemplateType);
            }
        }

        protected override void OnDeactivating()
        {
            base.OnDeactivating();
            Frame.GetController<TrackObjectConstructionsViewController>().ObjectCreated -= OnObjectCreated;
        }
    }
}
