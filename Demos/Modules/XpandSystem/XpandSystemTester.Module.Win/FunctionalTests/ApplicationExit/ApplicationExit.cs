using System;
using System.Linq;
using System.Windows.Forms;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.Win.Templates;
using DevExpress.Persistent.Base;
using Xpand.ExpressApp.Win.SystemModule;

namespace XpandSystemTester.Module.Win.FunctionalTests.ApplicationExit {
    public class ApplicationExit:ViewController {
        private readonly SingleChoiceAction _singleChoiceAction;
        private MainForm _mainForm;

        public ApplicationExit() {
            _singleChoiceAction = new SingleChoiceAction(this,GetType().Name,PredefinedCategory.ObjectsCreation){
                ItemType = SingleChoiceActionItemType.ItemIsOperation
            };
            _singleChoiceAction.Items.Add(new ChoiceActionItem("MinimizeOnExit", "MinimizeOnExit"));
            _singleChoiceAction.Items.Add(new ChoiceActionItem("HideOnExit", "HideOnExit"));
            _singleChoiceAction.Items.Add(new ChoiceActionItem("PromptOnExit", "PromptOnExit"));
            _singleChoiceAction.Execute+=SingleChoiceActionOnExecute;
        }

        public SingleChoiceAction SingleChoiceAction{
            get { return _singleChoiceAction; }
        }

        protected override void OnFrameAssigned() {
            base.OnFrameAssigned();
            Frame.TemplateChanged+=FrameOnTemplateChanged;
        }

        private void FrameOnTemplateChanged(object sender, EventArgs eventArgs){
            if (Frame.Template is MainForm)
                _mainForm = Frame.Template as MainForm;
        }

        private void SingleChoiceActionOnExecute(object sender, SimpleActionExecuteEventArgs simpleActionExecuteEventArgs){
            var id = SingleChoiceAction.SelectedItem.Id;
            ((IModelOptionsApplicationExit)Application.Model.Options).SetValue(id, true);
            _mainForm.Close();
            var value = new[] { "PromptOnExit", "MinimizeOnExit" }.Contains(id) ? _mainForm.WindowState != FormWindowState.Minimized : _mainForm.Visible;
            _singleChoiceAction.Enabled.SetItemValue("ActionOnFormIsNot", value);
            if (!_mainForm.Disposing&&!_mainForm.IsDisposed)
                _mainForm.Show();
        }
    }
}
