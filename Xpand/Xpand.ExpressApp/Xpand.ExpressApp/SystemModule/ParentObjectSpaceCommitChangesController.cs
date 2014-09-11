using System;
using System.ComponentModel;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Xpo;
using Xpand.Persistent.Base.General.Model;

namespace Xpand.ExpressApp.SystemModule {
    public interface IModelClassParentObjectSpaceCommitChanges {
        [Category(AttributeCategoryNameProvider.Xpand)]
        bool CommitParentObjectSpaceChanges { get; set; }
    }
    [ModelInterfaceImplementor(typeof(IModelClassParentObjectSpaceCommitChanges), "ModelClass")]
    public interface IModelDetailViewParentObjectSpaceCommitChanges : IModelClassParentObjectSpaceCommitChanges {
    }
    public class ParentObjectSpaceCommitChangesController : ViewController<DetailView>, IModelExtender {
        protected override void OnActivated() {
            base.OnActivated();
            if (ObjectSpace is XPNestedObjectSpace && ((IModelDetailViewParentObjectSpaceCommitChanges)View.Model).CommitParentObjectSpaceChanges) {
                ObjectSpace.Committed += ObjectSpaceOnCommitted;
            }
        }
        protected override void OnDeactivated() {
            base.OnDeactivated();
            if (ObjectSpace is XPNestedObjectSpace) {
                ObjectSpace.Committed -= ObjectSpaceOnCommitted;
            }
        }
        void ObjectSpaceOnCommitted(object sender, EventArgs eventArgs) {
            ((XPNestedObjectSpace)ObjectSpace).ParentObjectSpace.CommitChanges();
        }

        void IModelExtender.ExtendModelInterfaces(ModelInterfaceExtenders extenders) {
            extenders.Add<IModelClass, IModelClassParentObjectSpaceCommitChanges>();
            extenders.Add<IModelDetailView, IModelDetailViewParentObjectSpaceCommitChanges>();
        }
    }
}
