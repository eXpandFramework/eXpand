using System;
using System.Collections.Generic;
using System.Linq;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.StateMachine;
using DevExpress.ExpressApp.Utils;
using Xpand.Utils.Linq;

namespace Xpand.ExpressApp.StateMachine.Controllers {
    public class ChangeStateActionController : ViewController<ObjectView> {
        private StateMachineController _stateMachineController;
        private StateMachineCacheController _stateMachineCacheController;
        public event EventHandler<ChoiceActionItemArgs> RequestActiveState;

        protected virtual void OnRequestActiveState(ChoiceActionItemArgs e) {
            var handler = RequestActiveState;
            if (handler != null) handler(this, e);
        }

        bool IsActive(ChoiceActionItem choiceActionItem) {
            var iStateMachine = (choiceActionItem.Data as IStateMachine);
            if (iStateMachine != null) {
                var boolList = new BoolList(true, BoolListOperatorType.Or);
                boolList.BeginUpdate();
                foreach (var item in choiceActionItem.Items) {
                    var iTransition = ((ITransition)item.Data);
                    var choiceActionItemArgs = new ChoiceActionItemArgs(iTransition, item.Active);
                    OnRequestActiveState(choiceActionItemArgs);
                    boolList.SetItemValue(ObjectSpace.GetKeyValueAsString(iTransition), item.Active.ResultValue);
                }
                boolList.EndUpdate();
                return boolList.ResultValue;
            }
            var transition = choiceActionItem.Data as ITransition;
            if (transition != null) {
                var choiceActionItemArgs = new ChoiceActionItemArgs(transition, choiceActionItem.Active);
                OnRequestActiveState(choiceActionItemArgs);
                return choiceActionItem.Active;
            }
            throw new NotImplementedException();
        }

        protected override void OnActivated() {
            base.OnActivated();
            _stateMachineCacheController = Frame.GetController<StateMachineCacheController>();
            _stateMachineController = Frame.GetController<StateMachineController>();
            _stateMachineController.ChangeStateAction.ItemsChanged += ChangeStateActionOnItemsChanged;
            ObjectSpace.ObjectChanged += ObjectSpaceOnObjectChanged;
        }

        protected override void OnDeactivated() {
            base.OnDeactivated();
            _stateMachineController.ChangeStateAction.ItemsChanged -= ChangeStateActionOnItemsChanged;
            ObjectSpace.ObjectChanged -= ObjectSpaceOnObjectChanged;
        }

        private void ObjectSpaceOnObjectChanged(object sender, ObjectChangedEventArgs objectChangedEventArgs){
            if (_stateMachineCacheController.CachedStateMachines.Any()){
                _stateMachineController.UpdateActionState();
                foreach (var item in _stateMachineController.ChangeStateAction.Items.GetItems<ChoiceActionItem>(@base => @base.Items)){
                    UpdatePanelActions(item.Id, item.Active[typeof(ChangeStateActionController).Name]);        
                }
            }
        }

        void ChangeStateActionOnItemsChanged(object sender, ItemsChangedEventArgs itemsChangedEventArgs) {
            foreach (ChoiceActionItem item in GetItems(itemsChangedEventArgs.ChangedItemsInfo)) {
                var isActive = IsActive(item);
                item.Active[typeof(ChangeStateActionController).Name] = isActive;
                UpdatePanelActions(item.Id, isActive);
            }
        }

        private void UpdatePanelActions(string itemId, bool isActive){
            var detailView = View as DetailView;
            if (detailView != null){
                foreach (string key in _stateMachineController.PanelActions().Keys){
                    var actionContainer = detailView.FindItem(key) as ActionContainerViewItem;
                    if (actionContainer != null){
                        var action = actionContainer.Actions.FirstOrDefault(@base => @base.Caption == itemId);
                        if (action != null) action.Active[typeof (ChangeStateActionController).Name] = isActive;
                    }
                }
            }
        }

        IEnumerable<ChoiceActionItem> GetItems(Dictionary<object, ChoiceActionItemChangesType> changedItemsInfo) {
            return changedItemsInfo.Where(pair => pair.Value == ChoiceActionItemChangesType.Add).Select(pair => pair.Key as ChoiceActionItem).Where(item => item != null);
        }

    }

    public class ChoiceActionItemArgs : EventArgs {
        readonly ITransition _transition;

        public ChoiceActionItemArgs(ITransition transition, BoolList active) {
            _transition = transition;
            Active = active;
        }

        public ITransition Transition {
            get { return _transition; }
        }

        public BoolList Active { get; set; }
    }
}
