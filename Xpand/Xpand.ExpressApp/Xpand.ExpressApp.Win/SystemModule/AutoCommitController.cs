using System.ComponentModel;
using Xpand.ExpressApp.SystemModule;
using Xpand.ExpressApp.SystemModule.Actions;
using Xpand.Persistent.Base.General;

namespace Xpand.ExpressApp.Win.SystemModule {

    public class AutoCommitController: ExpressApp.SystemModule.AutoCommitController {

        protected override void OnActivated(){
            base.OnActivated();
            if (((IModelObjectViewAutoCommit)View.Model).AutoCommit) {
                Frame.GetController<AvailableActionListController>(controller => {
                    foreach (var action in controller.AvailableActions) {
                        action.Executing += ActionBaseOnExecuting;
                    }
                    controller.AvailableActionListChanged += OnAvailableActionListChanged;
                });
            }
        }

        private void OnAvailableActionListChanged(object sender, AvailableActionArgs e){
            if (!e.Added)
                e.ActionBase.Executing-=ActionBaseOnExecuting;
            else{
                e.ActionBase.Executing+=ActionBaseOnExecuting;
            }
        }

        protected override void OnDeactivated() {
            base.OnDeactivated();
            if (((IModelObjectViewAutoCommit)View.Model).AutoCommit) {
                CommitChanges();
                Frame.GetController<AvailableActionListController>(controller => {
                    foreach (var action in controller.AvailableActions){
                        action.Executing -= ActionBaseOnExecuting;
                    }
                });
            }
        }

        private void ActionBaseOnExecuting(object sender, CancelEventArgs cancelEventArgs) {
            CommitChanges();
        }
    }
}
