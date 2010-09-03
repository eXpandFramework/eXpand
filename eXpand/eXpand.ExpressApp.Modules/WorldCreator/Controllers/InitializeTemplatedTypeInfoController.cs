using DevExpress.ExpressApp;
using eXpand.ExpressApp.WorldCreator.Core;
using eXpand.Persistent.Base.PersistentMetaData;
using eXpand.ExpressApp.Core;

namespace eXpand.ExpressApp.WorldCreator.Controllers {
    public class InitializeTemplatedTypeInfoController : ViewController<XpandDetailView> 
    {
        public InitializeTemplatedTypeInfoController() {
            TargetObjectType = typeof(IPersistentTemplatedTypeInfo);
        }
        protected override void OnViewControlsCreated()
        {
            base.OnViewControlsCreated();
            if (ObjectSpace.Session.IsNewObject(View.CurrentObject))
                ((IPersistentTemplatedTypeInfo)View.CurrentObject).Init(WCTypesInfo.Instance.FindBussinessObjectType<ICodeTemplate>());

        }
    }
}