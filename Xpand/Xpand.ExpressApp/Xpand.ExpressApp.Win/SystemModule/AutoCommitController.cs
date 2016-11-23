using System.ComponentModel;
using Xpand.ExpressApp.SystemModule;
using Xpand.ExpressApp.SystemModule.Actions;

namespace Xpand.ExpressApp.Win.SystemModule {

    public class AutoCommitController: ExpressApp.SystemModule.AutoCommitController {
        private AvailableActionListController _availableActionListController;

        protected override void OnActivated(){
            base.OnActivated();
            if (((IModelObjectViewAutoCommit)View.Model).AutoCommit) {
                _availableActionListController = Frame.GetController<AvailableActionListController>();
                foreach (var action in _availableActionListController.AvailableActions){
                    action.Executing+=ActionBaseOnExecuting;
                }
                _availableActionListController.AvailableActionListChanged += OnAvailableActionListChanged;
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
                foreach (var action in _availableActionListController.AvailableActions) {
                    action.Executing -= ActionBaseOnExecuting;
                }
            }
        }

        private void ActionBaseOnExecuting(object sender, CancelEventArgs cancelEventArgs) {
            CommitChanges();
        }
    }
}
