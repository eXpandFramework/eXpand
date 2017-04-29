using System;
using System.Linq;
using System.Windows.Forms;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using DevExpress.Persistent.Base;
using Xpand.ExpressApp.Win.SystemModule;

namespace SystemTester.Module.Win.FunctionalTests.ApplicationExit {
    public class ApplicationExit:ObjectViewController<ObjectView,ApplicationExitObject> {
        private readonly SingleChoiceAction _singleChoiceAction;

        public ApplicationExit() {
            _singleChoiceAction = new SingleChoiceAction(this,GetType().Name,PredefinedCategory.ObjectsCreation){
                ItemType = SingleChoiceActionItemType.ItemIsOperation,
                TargetViewType = ViewType.ListView
            };
            _singleChoiceAction.Items.Add(new ChoiceActionItem("MinimizeOnExit", "MinimizeOnExit"));
            _singleChoiceAction.Items.Add(new ChoiceActionItem("HideOnExit", "HideOnExit"));
            _singleChoiceAction.Items.Add(new ChoiceActionItem("PromptOnExit", "PromptOnExit"));
            _singleChoiceAction.Execute+=SingleChoiceActionOnExecute;
        }

        public SingleChoiceAction SingleChoiceAction => _singleChoiceAction;

        private void SingleChoiceActionOnExecute(object sender, SimpleActionExecuteEventArgs simpleActionExecuteEventArgs){
            var id = SingleChoiceAction.SelectedItem.Id;
            ((IModelOptionsApplicationExit)Application.Model.Options).SetValue(id, true);
            var mainForm = ((Form) Application.MainWindow.Template);
            mainForm.Close();
            var value = new[] { "PromptOnExit", "MinimizeOnExit" }.Contains(id) ? mainForm.WindowState != FormWindowState.Minimized : mainForm.Visible;
            _singleChoiceAction.Enabled.SetItemValue("ActionOnFormIsNot", value);
            if (!mainForm.Disposing&&!mainForm.IsDisposed)
                mainForm.Show();
        }
    }
}

