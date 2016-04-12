using System;
using System.Collections.Generic;
using System.Windows.Forms;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.Model;
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
            
            SaveAction.ExecuteCompleted += SaveActionOnExecuteCompleted;
        }

        protected override void UnSubscribeEvents() {
            base.UnSubscribeEvents();
            SaveAction.ExecuteCompleted -= SaveActionOnExecuteCompleted;
        }
        void SaveActionOnExecuteCompleted(object sender, ActionBaseEventArgs actionBaseEventArgs) {
            if (!SaveAction.Enabled)
                RuntimeMemberBuilder.CreateRuntimeMembers(ModelApplication);
        }

        protected override void UpdateActionState() {
            try{
                base.UpdateActionState();
            }
            catch (NullReferenceException){
            }
            if (CurrentModelNode != null) {
                var modelRuntimeMember = CurrentModelNode.ModelNode as IModelMemberEx;
                if (modelRuntimeMember != null) {
                    DeleteAction.Enabled.SetItemValue("CanDeleteNode", true);
                }
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