using DevExpress.ExpressApp;
using Xpand.Persistent.Base.General;

namespace Xpand.ExpressApp.StateMachine.Controllers {
    public class AdminRolesController:ViewController {
        protected override void OnActivated(){
            base.OnActivated();
            Frame.GetController<StatePropertyController>(statePropertyController => {
                statePropertyController.CustomStatePropertyIsEnabled += CustomStatePropertyIsEnabled;
                statePropertyController.CustomFilterEditorItems += CustomFilterEditorItems;
            });
        }


        private void CustomFilterEditorItems(object sender, StatePropertyFilterEditorItemsEventArgs e){
            e.Handled = e.StateMachine.CanExecuteAllTransitions();
        }

        protected override void OnDeactivated(){
            base.OnDeactivated();
            Frame.GetController<StatePropertyController>(statePropertyController => {
                statePropertyController.CustomStatePropertyIsEnabled -= CustomStatePropertyIsEnabled;
                statePropertyController.CustomFilterEditorItems -= CustomFilterEditorItems;
            });
        }

        private void CustomStatePropertyIsEnabled(object sender, StatePropertyEventArgs e){
            var canExecuteAllTransitions = e.StateMachine.CanExecuteAllTransitions();
            if (canExecuteAllTransitions){
                e.Handled = true;
                e.Enable = e.Handled;
            }
        }
    }
}
