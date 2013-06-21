using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.Model;
using System.Linq;
using Xpand.ExpressApp.Core;
using Xpand.ExpressApp.Model;
using Xpand.Persistent.Base.General;


namespace Xpand.ExpressApp.Win {
    public class ModelEditorViewController : DevExpress.ExpressApp.Win.Core.ModelEditor.ModelEditorViewController {
        public ModelEditorViewController(IModelApplication modelApplication, ModelDifferenceStore diffstore)
            : base(modelApplication, diffstore) {

        }

        protected override void SubscribeEvents() {
            base.SubscribeEvents();
            AddNodeAction.ItemsChanged += AddNodeActionOnItemsChanged;
            SaveAction.ExecuteCompleted += SaveActionOnExecuteCompleted;
        }

        protected override void UnSubscribeEvents() {
            base.UnSubscribeEvents();
            AddNodeAction.ItemsChanged -= AddNodeActionOnItemsChanged;
            SaveAction.ExecuteCompleted -= SaveActionOnExecuteCompleted;
        }
        void SaveActionOnExecuteCompleted(object sender, ActionBaseEventArgs actionBaseEventArgs) {
            RuntimeMemberBuilder.AddFields(ModelApplication);
        }

        protected override void UpdateActionState() {
            base.UpdateActionState();
            if (CurrentModelNode != null) {
                var modelRuntimeMember = CurrentModelNode.ModelNode as IModelRuntimeMember;
                if (modelRuntimeMember != null) {
                    DeleteAction.Enabled.SetItemValue("CanDeleteNode", true);
                }
            }
        }

        void AddNodeActionOnItemsChanged(object sender, ItemsChangedEventArgs itemsChangedEventArgs) {
            var singleChoiceAction = (sender) as SingleChoiceAction;
            if ((singleChoiceAction != null && singleChoiceAction.Id == "Add") && (itemsChangedEventArgs.ChangedItemsInfo.Values.Contains(ChoiceActionItemChangesType.ItemsAdd | ChoiceActionItemChangesType.ItemsRemove))) {
                string name = CurrentModelNode.ModelNode.GetType().Name;
                switch (name) {
                    case "ModelLogicRules":
                        FilterModelLogicRules(singleChoiceAction);
                        break;
                    case "ModelBOModelClassMembers":
                        EnableBOModelClassMembersAddMenu();
                        break;
                }
            }
        }

        void EnableBOModelClassMembersAddMenu() {
            var childNodeTypes = Adapter.fastModelEditorHelper.GetListChildNodeTypes(CurrentModelNode.ModelNode.NodeInfo);
            foreach (var childNodeType in childNodeTypes) {
                AddNodeAction.Items.Add(new ChoiceActionItem(childNodeType.Key, childNodeType.Value));
            }
        }

        void FilterModelLogicRules(SingleChoiceAction singleChoiceAction) {
            var modelTreeListNode = CurrentModelNode.Parent;
            var typesInfo = modelTreeListNode.ModelNode.Application.GetTypesInfo();
            var type=modelTreeListNode.ModelNode.GetType().GetInterface("I" + modelTreeListNode.ModelNode.GetType().Name);
            var modelLogicRuleAttribute = typesInfo.FindTypeInfo(type).FindAttributes<ModelLogicRuleAttribute>().Single();
            for (int i = singleChoiceAction.Items.Count - 1; i > -1; i--) {
                if (!modelLogicRuleAttribute.RuleType.Name.EndsWith(singleChoiceAction.Items[i].Id))
                    singleChoiceAction.Items.RemoveAt(i);
            }
        }
    }
}