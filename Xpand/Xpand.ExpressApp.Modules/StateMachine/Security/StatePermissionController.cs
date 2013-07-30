using System;
using System.Security;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Security;
using DevExpress.ExpressApp.StateMachine;
using Xpand.ExpressApp.Security.Core;
using Xpand.ExpressApp.StateMachine.Security.Improved;

namespace Xpand.ExpressApp.StateMachine.Security {
    public interface IModelOptionsStateMachine {
        bool PermissionsForActionState { get; set; }
    }

    public class StateMachineController : DevExpress.ExpressApp.StateMachine.StateMachineController {
        protected override void UpdateActionState() {
            base.UpdateActionState();

        }
    }
    public class StatePermissionController : ViewController, IModelExtender {
        void IModelExtender.ExtendModelInterfaces(ModelInterfaceExtenders extenders) {
            extenders.Add<IModelOptions, IModelOptionsStateMachine>();
        }

        protected override void OnActivated() {
            base.OnActivated();
            var stateMachineController = Frame.GetController<StateMachineController>();
            if (((IModelOptionsStateMachine)Application.Model.Options).PermissionsForActionState) {
                var singleChoiceAction = stateMachineController.ChangeStateAction;
                singleChoiceAction.ItemsChanged += OnItemsChanged;
            }
            stateMachineController.TransitionExecuting += OnTransitionExecuting;
        }

        void OnItemsChanged(object sender, ItemsChangedEventArgs itemsChangedEventArgs) {
            var changedItemsInfo = itemsChangedEventArgs.ChangedItemsInfo;
            foreach (var actionItemChangesType in changedItemsInfo) {
                if (actionItemChangesType.Key is ChoiceActionItem && actionItemChangesType.Value == ChoiceActionItemChangesType.Add) {
                    var choiceActionItem = ((ChoiceActionItem)actionItemChangesType.Key);
                    var transition = choiceActionItem.Data as ITransition;
                    if (transition != null)
                        choiceActionItem.Active["Permission"] = !IsGranted(transition.TargetState);
                }
            }
        }

        void OnTransitionExecuting(object sender, ExecuteTransitionEventArgs executeTransitionEventArgs) {
            var targetState = executeTransitionEventArgs.Transition.TargetState;
            if (!IsGranted(targetState))
                throw new UserFriendlyException("Permissions are not granted for transitioning to the " + targetState.Caption);
        }

        bool IsGranted(IState state) {
            if (!((IRoleTypeProvider)SecuritySystem.Instance).IsNewSecuritySystem())
                return IsGranted(new StateMachineTransitionPermission(StateMachineTransitionModifier.Allow, state.Caption, state.StateMachine.Name));
            var stateMachineTransitionPermission = new Improved.StateMachineTransitionPermission {
                Modifier = StateMachineTransitionModifier.Allow,
                StateCaption = state.Caption,
                StateMachineName = state.StateMachine.Name
            };
            return SecuritySystem.IsGranted(new StateMachineTransitionOperationRequest(stateMachineTransitionPermission));
        }

        bool IsGranted(IPermission permission) {
            var securityComplex = ((SecurityBase)SecuritySystem.Instance);
            bool isGrantedForNonExistentPermission = securityComplex.IsGrantedForNonExistentPermission;
            securityComplex.IsGrantedForNonExistentPermission = true;
            bool granted = SecuritySystem.IsGranted(permission);
            securityComplex.IsGrantedForNonExistentPermission = isGrantedForNonExistentPermission;
            return granted;
        }
    }
}