using System.Linq;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.SystemModule;
using DevExpress.ExpressApp.Xpo;
using Xpand.ExpressApp.ModelDifference.DataStore.BaseObjects;
using Xpand.ExpressApp.ModelDifference.DataStore.Queries;
using Xpand.Persistent.Base.General;

namespace Xpand.ExpressApp.ModelDifference.Controllers {
    public class DeleteActiveUserModelController:ObjectViewController<ObjectView,ModelDifferenceObject> {
        private bool _lockingCheckEnabled;

        protected override void OnActivated(){
            base.OnActivated();
            var deleteObjectsViewController = Frame.GetController<DeleteObjectsViewController>();
            deleteObjectsViewController.Deleting+=OnDeleting;
            deleteObjectsViewController.DeleteAction.ExecuteCompleted+=DeleteActionOnExecuteCompleted;
        }

        protected override void OnDeactivated(){
            base.OnDeactivated();
            var deleteObjectsViewController = Frame.GetController<DeleteObjectsViewController>();
            deleteObjectsViewController.Deleting -= OnDeleting;
            deleteObjectsViewController.DeleteAction.ExecuteCompleted -= DeleteActionOnExecuteCompleted;
        }

        private void OnDeleting(object sender, DeletingEventArgs deletingEventArgs){
            if (deletingEventArgs.Objects.OfType<UserModelDifferenceObject>().Any(o => IsCurrentUserModel(o))){
                _lockingCheckEnabled = ((XPObjectSpace)ObjectSpace).LockingCheckEnabled;
                ((XPObjectSpace) ObjectSpace).LockingCheckEnabled = !((XPObjectSpace) ObjectSpace).LockingCheckEnabled;
            }
        }

        private bool IsCurrentUserModel(UserModelDifferenceObject userModelDifferenceObject){
            UserModelDifferenceObject activeModelDifference = new QueryUserModelDifferenceObject(userModelDifferenceObject.Session).GetActiveModelDifference(
                ApplicationHelper.Instance.Application.GetType().FullName, Name);
            return ReferenceEquals(activeModelDifference, userModelDifferenceObject);
        }

        private void DeleteActionOnExecuteCompleted(object sender, ActionBaseEventArgs actionBaseEventArgs){
            ((XPObjectSpace)ObjectSpace).LockingCheckEnabled=_lockingCheckEnabled;
        }
    }
}
