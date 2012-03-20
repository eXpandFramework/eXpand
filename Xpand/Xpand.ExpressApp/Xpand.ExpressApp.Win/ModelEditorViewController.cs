using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.Model;
using System.Linq;


namespace Xpand.ExpressApp.Win {
    public class ModelEditorViewController : DevExpress.ExpressApp.Win.Core.ModelEditor.ModelEditorViewController {
        public ModelEditorViewController(IModelApplication modelApplication, ModelDifferenceStore diffstore)
            : base(modelApplication, diffstore) {
            AddNodeAction.ItemsChanged += AddNodeActionOnItemsChanged;
        }

        void AddNodeActionOnItemsChanged(object sender, ItemsChangedEventArgs itemsChangedEventArgs) {
            var singleChoiceAction = (sender) as SingleChoiceAction;
            if (singleChoiceAction != null && (singleChoiceAction.Id == "Add" && itemsChangedEventArgs.ChangedItemsInfo.Values.Contains(ChoiceActionItemChangesType.ItemsAdd))) {
                if (CurrentModelNode.ModelNode.GetType().Name == "ModelLogicRules") {
                    var modelTreeListNode = CurrentModelNode.Parent;
                    for (int i = singleChoiceAction.Items.Count - 1; i > -1; i--) {
                        var value = modelTreeListNode.ModelNode.Id.Replace("Conditional", "");
                        if (!singleChoiceAction.Items[i].Id.StartsWith(value))
                            singleChoiceAction.Items.RemoveAt(i);
                    }
                }
            }
        }
    }
}