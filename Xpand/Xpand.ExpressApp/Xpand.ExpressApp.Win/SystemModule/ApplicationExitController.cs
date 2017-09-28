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

        public ApplicationExitController(){
            TargetWindowType=WindowType.Main;
        }

        protected override void OnActivated(){
            base.OnActivated();
            Frame.GetController<LogoffController>().LogoffAction.Executing+=LogoffActionOnExecuting;
            Frame.GetController<EditModelController>().EditModelAction.Executing+=EditModelActionOnExecuting;
        }

        protected override void OnDeactivated(){
            base.OnDeactivated();
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
                Frame.TemplateChanged += (sender, args) => ((Form) Frame.Template).Closing += OnClosing;
            }
        }

        private void OnClosing(object sender, CancelEventArgs e){
            if (!_isLoggingOff&&!_isEditingModel){
                var applicationExit = ((IModelOptionsApplicationExit)Application.Model.Options);
                e.Cancel = MinimizeOnClose(applicationExit);
                if (!e.Cancel)
                    e.Cancel = HideOnClose(applicationExit);
                if (!e.Cancel) {
                    e.Cancel = PromptOnExit(applicationExit);
                }
            }
        }

        private bool PromptOnExit(IModelOptionsApplicationExit applicationExit) {
            if (applicationExit.PromptOnExit){
                var promptOnExitTitle = CaptionHelper.GetLocalizedText(XpandSystemWindowsFormsModule.XpandWin, "PromptOnExitTitle");
                var promptOnExitMessage = CaptionHelper.GetLocalizedText(XpandSystemWindowsFormsModule.XpandWin, "PromptOnExitMessage");
                return WinApplication.Messaging.GetUserChoice(promptOnExitMessage, promptOnExitTitle, MessageBoxButtons.YesNo) != DialogResult.Yes;
            }
            return false;
        }

        private bool HideOnClose(IModelOptionsApplicationExit modelOptionsApplicationExit) {
            if (modelOptionsApplicationExit.HideOnExit) {
                ((Form) Application.MainWindow.Template).Hide();
                return true;
            }
            return false;
        }

        private bool MinimizeOnClose(IModelOptionsApplicationExit modelOptionsApplicationExit) {
            if (modelOptionsApplicationExit.MinimizeOnExit) {
                ((Form)Application.MainWindow.Template).WindowState = FormWindowState.Minimized;
                return true;
            }
            return false;
        }

        void IModelExtender.ExtendModelInterfaces(ModelInterfaceExtenders extenders) {
            extenders.Add<IModelOptions, IModelOptionsApplicationExit>();
        }

    }
}