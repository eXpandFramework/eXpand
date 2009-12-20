using DevExpress.ExpressApp;
using eXpand.ExpressApp.WorldCreator.Observers;
using eXpand.Persistent.Base.PersistentMetaData;

namespace eXpand.ExpressApp.WorldCreator.Controllers
{
    public class PersistentReferenceMemberInfoController:ViewController
    {
        public PersistentReferenceMemberInfoController() {
            TargetObjectType = typeof (IPersistentReferenceMemberInfo);
        }
        protected override void OnActivated()
        {
            base.OnActivated();
            new PersistentReferenceMemberInfoObserver(ObjectSpace);         
        }
    }
}
