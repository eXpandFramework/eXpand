using DevExpress.ExpressApp;
using Xpand.ExpressApp.WorldCreator.System;
using Xpand.Persistent.Base.General;
using Xpand.Persistent.Base.PersistentMetaData;

namespace Xpand.ExpressApp.WorldCreator.Controllers{
    public class InitializeTemplatedTypeInfoController : ViewController<DetailView>{
        public InitializeTemplatedTypeInfoController(){
            TargetObjectType = typeof (IPersistentTemplatedTypeInfo);
        }

        protected override void OnViewControlsCreated(){
            base.OnViewControlsCreated();
            if (ObjectSpace.IsNewObject(View.CurrentObject))
                ((IPersistentTemplatedTypeInfo) View.CurrentObject).Init(
                    XafTypesInfo.Instance.FindBussinessObjectType<ICodeTemplate>());
        }
    }
}