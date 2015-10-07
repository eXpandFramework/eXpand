//using System;
//using System.ComponentModel;
//using System.Linq;
//using DevExpress.ExpressApp;
//using DevExpress.ExpressApp.Actions;
//using DevExpress.ExpressApp.StateMachine;
//using Fasterflect;
//using Xpand.Persistent.Base.General;
//
//namespace Xpand.ExpressApp.StateMachine.Controllers {
//    public class StateMachineAdminRolesController : StateMachineControllerBase<ObjectView> {
//        StateMachineController _stateMachineController;
//
//        protected override void OnFrameAssigned(){
//            base.OnFrameAssigned();
//            if (Application.CanBuildSecurityObjects()) {
//                _stateMachineController = Frame.GetController<StateMachineController>();
//                _stateMachineController.ActionStateUpdated+=StateMachineControllerOnActionStateUpdated;
//                _stateMachineController.ChangeStateAction.Executing+=ChangeStateActionOnExecuting;
//                Frame.Disposing += FrameOnDisposing;
//            }
//        }
//
//        private void FrameOnDisposing(object sender, EventArgs eventArgs){
//            Frame.Disposing-=FrameOnDisposing;
//            _stateMachineController.ActionStateUpdated-=StateMachineControllerOnActionStateUpdated;
//            _stateMachineController.ChangeStateAction.Executing -= ChangeStateActionOnExecuting;
//        }
//
//        private void ChangeStateActionOnExecuting(object sender, CancelEventArgs cancelEventArgs){
//            var state = _stateMachineController.ChangeStateAction.SelectedItem.Data as IState;
//            if (state != null){
//                cancelEventArgs.Cancel =state.StateMachine.CanExecuteAllTransitions();
//                if (cancelEventArgs.Cancel){
//                    
//                    var transition = state.StateMachine.States.SelectMany(state1 => state.Transitions).FirstOrDefault(tr => tr.TargetState==state);
//                    if (transition != null)
//                        _stateMachineController.CallMethod("ExecuteTransition", View.CurrentObject, transition);
//                }
//            }
//        }
//
//        private void StateMachineControllerOnActionStateUpdated(object sender, EventArgs eventArgs){
//            Object viewCurrentObject = (View != null && View.SelectedObjects.Count == 1) ? View.SelectedObjects[0] : null;
//            if (viewCurrentObject != null) {
//                var changeStateAction = _stateMachineController.ChangeStateAction;
//                var stateMachines = GetStateMachines().Where(machine => machine.CanExecuteAllTransitions()).ToArray();
//                foreach (IStateMachine stateMachine in stateMachines) {
//                    var choiceActionItem = changeStateAction.Items.FirstOrDefault(item => item.Caption==stateMachine.Name);
//                    if (choiceActionItem != null){
//                        choiceActionItem.Items.Clear();
//                        var choiceActionItems = stateMachine.States.Select(state => new ChoiceActionItem(state.Caption,state)).ToArray();
//                        choiceActionItem.Items.AddRange(choiceActionItems);
//                    }
//                }
//            }
//
//        }
//    }
//}