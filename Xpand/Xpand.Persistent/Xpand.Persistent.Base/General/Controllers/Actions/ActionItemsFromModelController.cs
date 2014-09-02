using System;
using System.ComponentModel;
using System.Linq;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.Model;
using Xpand.Persistent.Base.General.Model;

namespace Xpand.Persistent.Base.General.Controllers.Actions {
    [ModelAbstractClass]
    public interface IModelActionItemsFromModel : IModelAction {
        [Category(AttributeCategoryNameProvider.Xpand)]
        bool ItemsFromModel { get; set; }
    }

    public class ActionItemsFromModelController:ViewController,IModelExtender{
        public event EventHandler<CustomizeActionItemArgs> CustomizeActionItem;

        protected virtual void OnCustomizeActionItem(CustomizeActionItemArgs e){
            var handler = CustomizeActionItem;
            if (handler != null) handler(this, e);
        }

        protected override void OnViewControllersActivated(){
            base.OnViewControllersActivated();
            var modelActions = Application.Model.ActionDesign.Actions.Cast<IModelActionItemsFromModel>().Where(model => model.ItemsFromModel);
            var choiceActionItems = modelActions.Where(model => model.ChoiceActionItems != null).SelectMany(model => model.ChoiceActionItems);
            var actions = Frame.Actions<SingleChoiceAction>(choiceActionItems).ToDictionary(@base => @base.Id, @base => @base);
            if (actions.Any()) {
                foreach (var choiceActionItem in choiceActionItems) {
                    var key = choiceActionItem.GetParent<IModelAction>().Id;
                    if (actions.ContainsKey(key)) {
                        var singleChoiceAction = actions[key];
                        if (singleChoiceAction.Items.FindItemByID(choiceActionItem.Id) == null) {
                            var actionItem = new ChoiceActionItem(choiceActionItem);
                            OnCustomizeActionItem(new CustomizeActionItemArgs(actionItem));
                            singleChoiceAction.Items.Add(actionItem);
                        }
                    }
                }
            }
        }

        public void ExtendModelInterfaces(ModelInterfaceExtenders extenders){
            extenders.Add<IModelAction, IModelActionItemsFromModel>();
        }
    }

    public class CustomizeActionItemArgs : EventArgs{
        private readonly ChoiceActionItem _actionItem;

        public CustomizeActionItemArgs(ChoiceActionItem actionItem){
            _actionItem = actionItem;
        }

        public ChoiceActionItem ActionItem{
            get { return _actionItem; }
        }
    }
}
