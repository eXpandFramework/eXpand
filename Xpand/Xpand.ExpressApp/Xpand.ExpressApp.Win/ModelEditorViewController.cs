using System.Collections.Generic;
using System.Windows.Forms;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.Model;
using System.Linq;
using DevExpress.ExpressApp.Utils;
using DevExpress.ExpressApp.Win;
using DevExpress.ExpressApp.Win.Core.ModelEditor;
using DevExpress.ExpressApp.Win.SystemModule;
using Xpand.Persistent.Base.RuntimeMembers;
using Xpand.Persistent.Base.RuntimeMembers.Model;
using Xpand.Utils.Helpers;


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
            if (!SaveAction.Enabled)
                RuntimeMemberBuilder.CreateRuntimeMembers(ModelApplication);
        }

        protected override void UpdateActionState() {
            base.UpdateActionState();
            if (CurrentModelNode != null) {
                var modelRuntimeMember = CurrentModelNode.ModelNode as IModelMemberEx;
                if (modelRuntimeMember != null) {
                    DeleteAction.Enabled.SetItemValue("CanDeleteNode", true);
                }
            }
        }

        void AddNodeActionOnItemsChanged(object sender, ItemsChangedEventArgs itemsChangedEventArgs) {
            var singleChoiceAction = (sender) as SingleChoiceAction;
            if ((singleChoiceAction != null && singleChoiceAction.Id == "Add") && (itemsChangedEventArgs.ChangedItemsInfo.Values.Contains(ChoiceActionItemChangesType.ItemsAdd | ChoiceActionItemChangesType.ItemsRemove))) {
                if (CurrentModelNode != null) {
                    string name = CurrentModelNode.ModelNode.GetType().Name;
                    switch (name) {
                        case "ModelBOModelClassMembers":
                            EnableBOModelClassMembersAddMenu();
                            break;
                    }
                }
            }
        }

        void EnableBOModelClassMembersAddMenu() {
            var childNodeTypes = Adapter.fastModelEditorHelper.GetListChildNodeTypes(CurrentModelNode.ModelNode.NodeInfo).Where(pair => pair.Key!="Member");
            foreach (var childNodeType in childNodeTypes) {
                AddNodeAction.Items.Add(new ChoiceActionItem(childNodeType.Key, childNodeType.Value));
            }
        }

        public static Form CreateModelEditorForm(WinApplication winApplication) {
            var modelDifferenceStore = (ModelDifferenceStore)typeof(XafApplication).Invoke(winApplication, "CreateUserModelDifferenceStore");
            var controller = new ModelEditorViewController(winApplication.Model,  modelDifferenceStore);
            var modelDifferencesStore = (ModelDifferenceStore) typeof(XafApplication).Invoke(winApplication, "CreateModelDifferenceStore");
            if (modelDifferencesStore != null) {
                var modulesDiffStoreInfo = new List<ModuleDiffStoreInfo> { new ModuleDiffStoreInfo(null, modelDifferencesStore, "Model") };
                controller.SetModuleDiffStore(modulesDiffStoreInfo);
            }
            return new ModelEditorForm(controller, new SettingsStorageOnModel(((IModelApplicationModelEditor)winApplication.Model).ModelEditorSettings));
        }
    }
}