using System;
using System.Collections.Generic;
using System.Linq;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.StateMachine.Xpo;
using DevExpress.ExpressApp.Utils;
using Xpand.ExpressApp.StateMachine.Security;
using Xpand.ExpressApp.StateMachine.Security.Improved;
using StateMachineTransitionPermission = Xpand.ExpressApp.StateMachine.Security.Improved.StateMachineTransitionPermission;

namespace Xpand.ExpressApp.StateMachine.Controllers {
    public class StateMachineController:ViewController<ObjectView> {
        protected override void OnActivated() {
            base.OnActivated();
            Frame.GetController<DevExpress.ExpressApp.StateMachine.StateMachineController>().ChangeStateAction.ItemsChanged+=ChangeStateActionOnItemsChanged;
        }
        protected override void OnDeactivated() {
            base.OnDeactivated();
            Frame.GetController<DevExpress.ExpressApp.StateMachine.StateMachineController>().ChangeStateAction.ItemsChanged -= ChangeStateActionOnItemsChanged;
        }
        void ChangeStateActionOnItemsChanged(object sender, ItemsChangedEventArgs itemsChangedEventArgs) {
            foreach (var item in GetItems(itemsChangedEventArgs.ChangedItemsInfo)) {
                item.Active[typeof (StateMachineTransitionPermission).Name] = IsActive(item);
            }
        }

        IEnumerable<ChoiceActionItem> GetItems(Dictionary<object, ChoiceActionItemChangesType> changedItemsInfo) {
            return changedItemsInfo.Where(pair => pair.Value == ChoiceActionItemChangesType.Add).Select(pair => pair.Key as ChoiceActionItem).Where(item => item != null);
        }

        bool IsActive(ChoiceActionItem choiceActionItem) {
            var xpoStateMachine = (choiceActionItem.Data as XpoStateMachine);
            if (xpoStateMachine!=null) {
                var boolList = new BoolList();
                boolList.BeginUpdate();
                foreach (var item in choiceActionItem.Items) {
                    var xpoTransition = ((XpoTransition) item.Data);
                    item.Active[typeof (StateMachineTransitionPermission).Name] = IsActive(xpoTransition);
                    boolList.SetItemValue(xpoTransition.Oid.ToString(), item.Active.ResultValue);
                }
                boolList.EndUpdate();
                return boolList.ResultValue;
            }
            var transition = choiceActionItem.Data as XpoTransition;
            if (transition != null) {
                return IsActive(transition);
            }
            throw new NotImplementedException();
        }

        bool IsActive( XpoTransition xpoTransition) {
            var permission = new StateMachineTransitionPermission{
                Modifier = StateMachineTransitionModifier.Allow,
                StateCaption = xpoTransition.TargetState.Caption,
                StateMachineName = xpoTransition.SourceState.StateMachine.Name,
                Hide=false
            };
            return SecuritySystem.IsGranted(new StateMachineTransitionOperationRequest(permission));
        }
    }
}
