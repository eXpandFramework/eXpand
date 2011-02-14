using System;
using System.ComponentModel;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Model;

namespace Xpand.ExpressApp.SystemModule {
    public interface IModelClassParentObjectSpaceCommitChanges {
        [Category("eXpand")]
        bool CommitParentObjectSpaceChanges { get; set; }
    }
    [ModelInterfaceImplementor(typeof(IModelClassParentObjectSpaceCommitChanges), "ModelClass")]
    public interface IModelDetailViewParentObjectSpaceCommitChanges : IModelClassParentObjectSpaceCommitChanges {
    }
    public class ParentObjectSpaceCommitChangesController : ViewController<DetailView>, IModelExtender {
        protected override void OnActivated() {
            base.OnActivated();
            if (ObjectSpace is NestedObjectSpace && ((IModelDetailViewParentObjectSpaceCommitChanges)View.Model).CommitParentObjectSpaceChanges) {
                ObjectSpace.Committed += ObjectSpaceOnCommitted;
            }
        }
        protected override void OnDeactivated() {
            base.OnDeactivated();
            if (ObjectSpace is NestedObjectSpace) {
                ObjectSpace.Committed -= ObjectSpaceOnCommitted;
            }
        }
        void ObjectSpaceOnCommitted(object sender, EventArgs eventArgs) {
            ((NestedObjectSpace)ObjectSpace).ParentObjectSpace.CommitChanges();
        }

        void IModelExtender.ExtendModelInterfaces(ModelInterfaceExtenders extenders) {
            extenders.Add<IModelClass, IModelClassParentObjectSpaceCommitChanges>();
            extenders.Add<IModelDetailView, IModelDetailViewParentObjectSpaceCommitChanges>();
        }
    }
}
