using System.ComponentModel;
using System.Linq;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.Model;
using Xpand.Persistent.Base.General.Model;

namespace Xpand.Persistent.Base.General.Controllers.Actions {
    [ModelAbstractClass]
    public interface IModelChoiceActionItemActive : IModelChoiceActionItem {
        [Category(AttributeCategoryNameProvider.Xpand)]
        bool? Active { get; set; }     
    }

    public class ActionModelChoiceItemController:ViewController,IModelExtender {

        public ActionModelChoiceItemController(){
            TargetViewNesting=Nesting.Root;
        }

        protected override void OnDeactivated(){
            base.OnDeactivated();
            foreach (var action in Frame.Actions<SingleChoiceAction>()) {
                action.ItemsChanged -= SingleChoiceActionOnItemsChanged;
            }
        }

        protected override void OnViewControlsCreated(){
            base.OnViewControlsCreated();
            foreach (var action in Frame.Actions<SingleChoiceAction>()) {
                action.ItemsChanged+=SingleChoiceActionOnItemsChanged;
                foreach (var item in action.Items){
                    UpdateItem(item,action);
                }
            }
        }

        private void UpdateItem(ChoiceActionItem item, SingleChoiceAction action){
            var nodePath = item.GetIdPath();
            var modelChoiceActionItemActive = action.Model.ChoiceActionItems.FindNodeByPath(nodePath) as IModelChoiceActionItemActive;
            if (modelChoiceActionItemActive != null && modelChoiceActionItemActive.Active!=null) {
                item.Active.BeginUpdate();
                item.Active[GetType().Name] = modelChoiceActionItemActive.Active.Value;
                item.Active.EndUpdate();
            }
        }

        private void SingleChoiceActionOnItemsChanged(object sender, ItemsChangedEventArgs e){
            var choiceActionItemChangesTypes = e.ChangedItemsInfo;
            foreach (var actionItem in choiceActionItemChangesTypes.Where(pair => pair.Value==ChoiceActionItemChangesType.Add).Select(pair => pair.Key).OfType<ChoiceActionItem>()){
                UpdateItem(actionItem,(SingleChoiceAction) sender);
            }
        }

        public void ExtendModelInterfaces(ModelInterfaceExtenders extenders){
            extenders.Add<IModelChoiceActionItem,IModelChoiceActionItemActive>();
        }
    }
}
