using System.ComponentModel;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Model;
using Xpand.Persistent.Base.General.Model;

namespace Xpand.ExpressApp.SystemModule {
    public interface IModelAutoCommit : IModelNode {
        [Category(AttributeCategoryNameProvider.Xpand)]
        bool AutoCommit { get; set; }
    }
    [ModelInterfaceImplementor(typeof(IModelAutoCommit), "ModelClass")]
    public interface IModelObjectViewAutoCommit : IModelAutoCommit {
    }

    public class AutoCommitController : ViewController<ObjectView>, IModelExtender {
        protected virtual void CommitChanges(){
            ObjectSpace.CommitChanges();
        }

        protected override void OnDeactivated(){
            base.OnDeactivated();
            if (((IModelObjectViewAutoCommit)View.Model).AutoCommit)
                CommitChanges();
        }

        void IModelExtender.ExtendModelInterfaces(ModelInterfaceExtenders extenders) {
            extenders.Add<IModelClass, IModelAutoCommit>();
            extenders.Add<IModelObjectView, IModelObjectViewAutoCommit>();
        }
    }
}