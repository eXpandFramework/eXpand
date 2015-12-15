using System.ComponentModel;
using DevExpress.ExpressApp;
using Xpand.Persistent.Base.PersistentMetaData;

namespace Xpand.ExpressApp.WorldCreator.Controllers {
    public class NeedCompilationController:ObjectViewController<ObjectView,IPersistentAssemblyInfo>{
        protected override void OnActivated(){
            base.OnActivated();
            ObjectSpace.Committing+=ObjectSpaceOnCommitting;
        }

        protected override void OnDeactivated(){
            base.OnDeactivated();
            ObjectSpace.Committing -= ObjectSpaceOnCommitting;
        }

        private void ObjectSpaceOnCommitting(object sender, CancelEventArgs cancelEventArgs){
            var persistentAssemblyInfo = View.CurrentObject as IPersistentAssemblyInfo;
            if (persistentAssemblyInfo != null) persistentAssemblyInfo.NeedCompilation = true;
        }
    }
}
