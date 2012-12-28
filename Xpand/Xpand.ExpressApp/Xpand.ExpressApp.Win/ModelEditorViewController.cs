using System.Reflection;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.Model;
using System.Linq;
using DevExpress.ExpressApp.Win.Core.ModelEditor;


namespace Xpand.ExpressApp.Win {
    public class ModelEditorViewController : DevExpress.ExpressApp.Win.Core.ModelEditor.ModelEditorViewController {
        public ModelEditorViewController(IModelApplication modelApplication, ModelDifferenceStore diffstore)
            : base(modelApplication, diffstore) {
            AddNodeAction.ItemsChanged += AddNodeActionOnItemsChanged;
        }

        void AddNodeActionOnItemsChanged(object sender, ItemsChangedEventArgs itemsChangedEventArgs) {
            var singleChoiceAction = (sender) as SingleChoiceAction;
            if (singleChoiceAction != null && (singleChoiceAction.Id == "Add" && itemsChangedEventArgs.ChangedItemsInfo.Values.Contains(ChoiceActionItemChangesType.ItemsAdd | ChoiceActionItemChangesType.ItemsRemove))) {
                string name = CurrentModelNode.ModelNode.GetType().Name;
                if (name == "ModelLogicRules") {
                    var modelTreeListNode = CurrentModelNode.Parent;
                    for (int i = singleChoiceAction.Items.Count - 1; i > -1; i--) {
                        var value = modelTreeListNode.ModelNode.Id.Replace("Conditional", "");
                        if (!singleChoiceAction.Items[i].Id.StartsWith(value))
                            singleChoiceAction.Items.RemoveAt(i);
                    }
                } else if (name == "ModelBOModelClassMembers") {
                    var adapter = (ExtendModelInterfaceAdapter)GetType().GetProperty("Adapter", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(this, null);
                    var childNodeTypes = adapter.fastModelEditorHelper.GetListChildNodeTypes(CurrentModelNode.ModelNode.NodeInfo);
                    foreach (var childNodeType in childNodeTypes) {
                        AddNodeAction.Items.Add(new ChoiceActionItem(childNodeType.Key, childNodeType.Value));
                    }
                }
            }
        }
    }
}