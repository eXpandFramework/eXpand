using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using DevExpress.Persistent.Base;

namespace SystemTester.Module.FunctionalTests.Actions {
    public class Actions:ObjectViewController<ObjectView,ActionsObject> {
        private const string ChangeColumnCaption = "ChangeColumnCaption";
        private const string RestoreColumnCaption = "RestoreColumnCaption";
        private readonly SingleChoiceAction _actionsAction;
        private string _columnCaption;

        public Actions(){
            ChangeModelAction();
            HiddenAction();
            ItemsFromModelAction();
            _actionsAction = new SingleChoiceAction(this,"Actions",PredefinedCategory.View);
            ActionsAction.Items.Add(new ChoiceActionItem(ChangeColumnCaption, null));
            ActionsAction.Items.Add(new ChoiceActionItem(RestoreColumnCaption, null));
            ActionsAction.ItemType = SingleChoiceActionItemType.ItemIsOperation;
            _actionsAction.Execute+=ActionsActionOnExecute;
        }

        public SingleChoiceAction ActionsAction{
            get { return _actionsAction; }
        }

        public XpandEasyTestController EasyTestControler{
            get { return Frame.GetController<XpandEasyTestController>(); }
        }

        private void ActionsActionOnExecute(object sender, SingleChoiceActionExecuteEventArgs e){
            if (e.SelectedChoiceActionItem.Id == RestoreColumnCaption){
                EasyTestControler.ChangeColumnCaption(_columnCaption);
                _columnCaption = null;
            }
            else if (e.SelectedChoiceActionItem.Id == ChangeColumnCaption){
                _columnCaption = EasyTestControler.ChangeColumnCaption("Changed");
            }
        }

        private void ItemsFromModelAction(){
            var itemsFromModelAction = new SingleChoiceAction(this, "ItemsFromModelAction", PredefinedCategory.View){
                ItemType = SingleChoiceActionItemType.ItemIsOperation
            };
            itemsFromModelAction.Execute += SingleChoiceActionOnExecute;
        }

        private void HiddenAction(){
            var hiddenAction = new SingleChoiceAction(this, "HiddenAction", PredefinedCategory.View){
                ItemType = SingleChoiceActionItemType.ItemIsOperation
            };
            hiddenAction.Items.Add(new ChoiceActionItem("CodeItem", null));
            hiddenAction.Execute += SingleChoiceActionOnExecute;
        }

        private void ChangeModelAction(){
            var changeModelAction = new SingleChoiceAction(this, "ChangeModelAction", PredefinedCategory.View){
                ItemType = SingleChoiceActionItemType.ItemIsOperation
            };
            changeModelAction.Execute += SingleChoiceActionOnExecute;
        }

        private void SingleChoiceActionOnExecute(object sender, SingleChoiceActionExecuteEventArgs singleChoiceActionExecuteEventArgs){
            
        }
    }
}
