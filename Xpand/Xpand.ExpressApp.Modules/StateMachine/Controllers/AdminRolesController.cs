using System;
using DevExpress.ExpressApp;

namespace Xpand.ExpressApp.StateMachine.Controllers {
    public class AdminRolesController:ViewController {
        protected override void OnActivated(){
            base.OnActivated();
            var enableStatePropertyController = Frame.GetController<StatePropertyController>();
            enableStatePropertyController.CustomStatePropertyIsEnabled+=CustomStatePropertyIsEnabled;
            enableStatePropertyController.CustomFilterEditorItems+=CustomFilterEditorItems;
        }

        private void CustomFilterEditorItems(object sender, StatePropertyFilterEditorItemsEventArgs e){
            e.Handled = e.StateMachine.CanExecuteAllTransitions();
        }

        protected override void OnDeactivated(){
            base.OnDeactivated();
            var enableStatePropertyController = Frame.GetController<StatePropertyController>();
            enableStatePropertyController.CustomStatePropertyIsEnabled -= CustomStatePropertyIsEnabled;
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
