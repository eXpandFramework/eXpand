using DevExpress.ExpressApp;
using Xpand.ExpressApp.WorldCreator.Observers;
using Xpand.Persistent.Base.PersistentMetaData;

namespace Xpand.ExpressApp.WorldCreator.Controllers
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
