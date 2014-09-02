using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using DevExpress.Persistent.Base;

namespace ViewVariantsTester.Module.FunctionalTests.Actions {
    public class Actions : ObjectViewController<ObjectView, ActionsObject> {
        private const string ChangeColumnCaption = "ChangeColumnCaption";
        private const string RestoreColumnCaption = "RestoreColumnCaption";
        private readonly SingleChoiceAction _actionsAction;
        private string _columnCaption;

        public Actions() {
            _actionsAction = new SingleChoiceAction(this, "Actions", PredefinedCategory.View);
            ActionsAction.Items.Add(new ChoiceActionItem(ChangeColumnCaption, null));
            ActionsAction.Items.Add(new ChoiceActionItem(RestoreColumnCaption, null));
            ActionsAction.ItemType = SingleChoiceActionItemType.ItemIsOperation;
            _actionsAction.Execute += ActionsActionOnExecute;
        }

        public SingleChoiceAction ActionsAction {
            get { return _actionsAction; }
        }

        public XpandEasyTestController EasyTestControler {
            get { return Frame.GetController<XpandEasyTestController>(); }
        }

        private void ActionsActionOnExecute(object sender, SingleChoiceActionExecuteEventArgs e) {
            if (e.SelectedChoiceActionItem.Id == RestoreColumnCaption) {
                EasyTestControler.ChangeColumnCaption(_columnCaption);
                _columnCaption = null;
            }
            else if (e.SelectedChoiceActionItem.Id == ChangeColumnCaption) {
                _columnCaption = EasyTestControler.ChangeColumnCaption("Changed");
            }
        }
    }
}
