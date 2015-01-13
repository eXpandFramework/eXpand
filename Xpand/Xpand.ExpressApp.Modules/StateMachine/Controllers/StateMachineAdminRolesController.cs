using System;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.StateMachine;
using Xpand.Persistent.Base.General;
using System.Linq;

namespace Xpand.ExpressApp.StateMachine.Controllers {
    public class StateMachineAdminRolesController : StateMachineControllerBase<ObjectView> {
        bool _changeStateActionOnItemsChanged;
        StateMachineController _stateMachineController;

        protected override void OnFrameAssigned(){
            base.OnFrameAssigned();
            if (Application.CanBuildSecurityObjects()) {
                _stateMachineController = Frame.GetController<StateMachineController>();
                _stateMachineController.TransitionExecuting += StateMachineControllerOnTransitionExecuting;
                _stateMachineController.ChangeStateAction.ItemsChanged += ChangeStateActionOnItemsChanged;
                Frame.Disposing += FrameOnDisposing;
            }
        }

        private void FrameOnDisposing(object sender, EventArgs eventArgs){
            Frame.Disposing-=FrameOnDisposing;
            _stateMachineController.TransitionExecuting -= StateMachineControllerOnTransitionExecuting;
            _stateMachineController.ChangeStateAction.ItemsChanged -= ChangeStateActionOnItemsChanged;
        }

        void ChangeStateActionOnItemsChanged(object sender, ItemsChangedEventArgs itemsChangedEventArgs) {
            if (!_changeStateActionOnItemsChanged&&View!=null) {
                var keyValuePairs =itemsChangedEventArgs.ChangedItemsInfo.Any(pair =>
                        pair.Value == ChoiceActionItemChangesType.ItemsAdd ||
                        pair.Value == ChoiceActionItemChangesType.ItemsRemove);
                if (keyValuePairs) {
                    _changeStateActionOnItemsChanged = true;
                    var stateMachineAction = _stateMachineController.ChangeStateAction;
                    Object viewCurrentObject = (View.SelectedObjects.Count == 1) ? View.SelectedObjects[0] : null;
                    if (viewCurrentObject != null) {
                        var stateMachines = GetStateMachines().Where(machine => machine.CanExecuteTransition());
                        foreach (var stateMachine in stateMachines) {
                            var item = stateMachineAction.Items.Find(stateMachine);
                            if (item != null){
                                item.Items.Clear();
                                foreach (var transition in stateMachine.States.SelectMany(state => state.Transitions)) {
                                    item.Items.Add(new ChoiceActionItem(transition.Caption, transition));
                                }
                            }
                        }
                    }
                    _changeStateActionOnItemsChanged = false;
                }
            }
        }

        void StateMachineControllerOnTransitionExecuting(object sender, ExecuteTransitionEventArgs executeTransitionEventArgs) {
            if (executeTransitionEventArgs.Transition.TargetState.StateMachine.CanExecuteTransition()){
                executeTransitionEventArgs.Cancel = true;
                new StateMachineLogic(ObjectSpace).ProcessTransition(View.CurrentObject,
                    executeTransitionEventArgs.Transition.TargetState.StateMachine.StatePropertyName,
                    ObjectSpace.GetObject(executeTransitionEventArgs.Transition.TargetState));
            }
            
        }


    }
}