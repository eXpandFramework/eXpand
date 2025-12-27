using System.ComponentModel;
using System.Windows.Forms;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.SystemModule;
using DevExpress.ExpressApp.Utils;
using DevExpress.ExpressApp.Win;

namespace Xpand.ExpressApp.Win.SystemModule{
    public interface IModelOptionsApplicationExit : IModelNode {
        [Category("eXpand.ApplicationExit")]
        bool MinimizeOnExit { get; set; }
        [Category("eXpand.ApplicationExit")]
        bool HideOnExit { get; set; }
        [Category("eXpand.ApplicationExit")]
        bool PromptOnExit { get; set; }
    }

    public class ApplicationExitController:WindowController, IModelExtender{
        private bool _isLoggingOff;
        private bool _isEditingModel;
        private bool _isTrayExit;

        public ApplicationExitController() => TargetWindowType=WindowType.Main;

        protected override void OnActivated(){
            base.OnActivated();
            Frame.GetController<LogoffController>().LogoffAction.Executing+=LogoffActionOnExecuting;
            Frame.GetController<EditModelController>().EditModelAction.Executing+=EditModelActionOnExecuting;
            Frame.GetController<NotifyIconController>().TrayExitAction.Executing+=TrayExitActionOnExecuting;
        }

        private void TrayExitActionOnExecuting(object o, CancelEventArgs cancelEventArgs){
            _isTrayExit = true;
        }

        protected override void OnDeactivated(){
            base.OnDeactivated();
            Frame.GetController<NotifyIconController>().TrayExitAction.Executing -= TrayExitActionOnExecuting;
            Frame.GetController<LogoffController>().LogoffAction.Executing -= LogoffActionOnExecuting;
            Frame.GetController<EditModelController>().EditModelAction.Executing -= EditModelActionOnExecuting;
        }

        private void EditModelActionOnExecuting(object o, CancelEventArgs e){
            _isEditingModel = !e.Cancel;
        }

        private void LogoffActionOnExecuting(object o, CancelEventArgs e){
            _isLoggingOff = !e.Cancel;
        }

        protected override void OnFrameAssigned(){
            base.OnFrameAssigned();
            if (Frame.Context == TemplateContext.ApplicationWindowContextName){
                Frame.TemplateChanged += (_, _) => ((Form) Frame.Template).FormClosing += OnClosing;
            }
        }

        private void OnClosing(object sender, CancelEventArgs e){
            if (!_isLoggingOff&&!_isEditingModel){
                MinimizeOnClose(e);
                HideOnClose(e);
                PromptOnExit(e);
            }
        }

        private void PromptOnExit(CancelEventArgs e) {
            var applicationExit = ((IModelOptionsApplicationExit)Application.Model.Options);
            if (applicationExit.PromptOnExit){
                var promptOnExitTitle = CaptionHelper.GetLocalizedText(XpandSystemWindowsFormsModule.XpandWin, "PromptOnExitTitle");
                var promptOnExitMessage = CaptionHelper.GetLocalizedText(XpandSystemWindowsFormsModule.XpandWin, "PromptOnExitMessage");
                if (WinApplication.Messaging.GetUserChoice(promptOnExitMessage, promptOnExitTitle,
                        MessageBoxButtons.YesNo) != DialogResult.Yes){
                    e.Cancel = true;
                }
            }
        }

        private void HideOnClose(CancelEventArgs e) {
            var applicationExit = ((IModelOptionsApplicationExit)Application.Model.Options);
            if (applicationExit.HideOnExit&&!_isTrayExit) {
                ((Form) Application.MainWindow.Template).Hide();
                e.Cancel = true;

            }
        }

        private void MinimizeOnClose(CancelEventArgs e) {
            var applicationExit = ((IModelOptionsApplicationExit)Application.Model.Options);
            if (applicationExit.MinimizeOnExit) {
                ((Form)Application.MainWindow.Template).WindowState = FormWindowState.Minimized;
                e.Cancel = true;
            }
        }

        void IModelExtender.ExtendModelInterfaces(ModelInterfaceExtenders extenders) {
            extenders.Add<IModelOptions, IModelOptionsApplicationExit>();
        }

    }
}