using System.ComponentModel;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Model;
using DevExpress.Xpo;
using Xpand.Persistent.Base.General;
using Xpand.Persistent.Base.General.Model;

namespace Xpand.ExpressApp.SystemModule {
    public interface IModelOptionsXpoSession:IModelNode {
        [Category(AttributeCategoryNameProvider.Xpand+".Session")]
        bool? TrackPropertiesModifications{ get; set; }
        [Category(AttributeCategoryNameProvider.Xpand + ".Session")]
        OptimisticLockingReadBehavior? OptimisticLockingReadBehavior { get; set; }
        
    }
    public interface IModelClassXpoSession: IModelOptionsXpoSession {
        [Category(AttributeCategoryNameProvider.Xpand + ".Session")]
        LockingOption? LockingOption { get; set; }
    }
    [ModelInterfaceImplementor(typeof(IModelClassXpoSession), "ModelClass")]
    public interface IModelObjectViewXpoSession : IModelClassXpoSession {
        
    }
    public class XpoSessionController:ViewController<ObjectView>,IModelExtender{
        protected override void OnActivated(){
            base.OnActivated();
            var model = ((IModelObjectViewXpoSession) View.Model);
            var trackPropertiesModifications = model.TrackPropertiesModifications;
            if (trackPropertiesModifications.HasValue)
                ObjectSpace.Session().TrackPropertiesModifications = trackPropertiesModifications.Value;
            if (model.OptimisticLockingReadBehavior.HasValue)
                ObjectSpace.Session().OptimisticLockingReadBehavior = model.OptimisticLockingReadBehavior.Value;
            if (model.LockingOption.HasValue)
                ObjectSpace.Session().LockingOption = model.LockingOption.Value;
        }

        public void ExtendModelInterfaces(ModelInterfaceExtenders extenders){
            extenders.Add<IModelOptions,IModelOptionsXpoSession>();
            extenders.Add<IModelClass,IModelClassXpoSession>();
            extenders.Add<IModelObjectView,IModelObjectViewXpoSession>();
        }
    }
}
