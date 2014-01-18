using System;
using System.Collections.Generic;
using System.Linq;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.StateMachine;
using DevExpress.ExpressApp.StateMachine.Xpo;
using DevExpress.ExpressApp.Utils;
using Fasterflect;

namespace Xpand.ExpressApp.StateMachine.Controllers {
    public class ChangeStateActionController:ViewController<ObjectView> {
        public event EventHandler<ChoiceActionItemArgs> RequestActiveState;

        protected virtual void OnRequestActiveState(ChoiceActionItemArgs e) {
            var handler = RequestActiveState;
            if (handler != null) handler(this, e);
        }

        bool IsActive(ChoiceActionItem choiceActionItem) {
            var xpoStateMachine = (choiceActionItem.Data as XpoStateMachine);
            if (xpoStateMachine != null) {
                var boolList = new BoolList();
                boolList.BeginUpdate();
                foreach (var item in choiceActionItem.Items) {
                    var xpoTransition = ((XpoTransition)item.Data);
                    var choiceActionItemArgs = new ChoiceActionItemArgs(xpoTransition, item.Active);
                    OnRequestActiveState(choiceActionItemArgs);
                    boolList.SetItemValue(xpoTransition.Oid.ToString(), item.Active.ResultValue);
                }
                boolList.EndUpdate();
                return boolList.ResultValue;
            }
            var transition = choiceActionItem.Data as XpoTransition;
            if (transition != null) {
                var choiceActionItemArgs = new ChoiceActionItemArgs(transition, choiceActionItem.Active);
                OnRequestActiveState(choiceActionItemArgs);
                return choiceActionItem.Active;
            }
            throw new NotImplementedException();
        }

        protected override void OnActivated() {
            base.OnActivated();
            Frame.GetController<StateMachineController>().ChangeStateAction.ItemsChanged += ChangeStateActionOnItemsChanged;
        }

        protected override void OnDeactivated() {
            base.OnDeactivated();
            Frame.GetController<StateMachineController>().ChangeStateAction.ItemsChanged -= ChangeStateActionOnItemsChanged;
        }

        void ChangeStateActionOnItemsChanged(object sender, ItemsChangedEventArgs itemsChangedEventArgs) {
            foreach (ChoiceActionItem item in GetItems(itemsChangedEventArgs.ChangedItemsInfo)) {
                var isActive = IsActive(item);
                item.Active[typeof(ChangeStateActionController).Name] = isActive;
                var stateMachineController = Frame.GetController<StateMachineController>();
                var detailView = View as DetailView;
                if (detailView != null) {
                    var panelActions = (Dictionary<object, List<SimpleAction>>)stateMachineController.GetFieldValue("panelActions");
                    foreach (string key in panelActions.Keys) {
                        var actionContainer = detailView.FindItem(key) as ActionContainerViewItem;
                        if (actionContainer!=null){
                            var action = actionContainer.Actions.FirstOrDefault(@base => @base.Caption==item.Id);
                            if (action != null) action.Active[typeof (ChangeStateActionController).Name] = isActive;
                        }
                    }
                }
            }

        }

        IEnumerable<ChoiceActionItem> GetItems(Dictionary<object, ChoiceActionItemChangesType> changedItemsInfo) {
            return changedItemsInfo.Where(pair => pair.Value == ChoiceActionItemChangesType.Add).Select(pair => pair.Key as ChoiceActionItem).Where(item => item != null);
        }

    }

    public class ChoiceActionItemArgs : EventArgs {
        readonly XpoTransition _transition;

        public ChoiceActionItemArgs(XpoTransition transition, BoolList active) {
            _transition = transition;
            Active=active;
        }

        public XpoTransition Transition {
            get { return _transition; }
        }

        public BoolList Active { get; set; }
    }
}
