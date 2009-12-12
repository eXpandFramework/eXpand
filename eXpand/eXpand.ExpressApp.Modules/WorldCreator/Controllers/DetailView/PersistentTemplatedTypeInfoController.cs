using DevExpress.ExpressApp;
using eXpand.ExpressApp.WorldCreator.Core;
using eXpand.Persistent.Base.PersistentMetaData;

namespace eXpand.ExpressApp.WorldCreator.Controllers.DetailView {
    public class PersistentTemplatedTypeInfoController : ViewController<DevExpress.ExpressApp.DetailView> 
    {
        public PersistentTemplatedTypeInfoController() {
            TargetObjectType = typeof(IPersistentTemplatedTypeInfo);
        }

        protected override void OnActivated()
        {
            base.OnActivated();
            if (ObjectSpace.Session.IsNewObject(View.CurrentObject))
                ((IPersistentTemplatedTypeInfo)View.CurrentObject).Init(TypesInfo.Instance.CodeTemplateType);
        }
    }
}