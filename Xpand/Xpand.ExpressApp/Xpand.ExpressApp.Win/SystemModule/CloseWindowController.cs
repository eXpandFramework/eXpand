using System;
using System.ComponentModel;
using System.Windows.Forms;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.Win.SystemModule;
using DevExpress.ExpressApp.Win.Templates;
using DevExpress.XtraEditors;
using Xpand.Persistent.Base.General;

namespace Xpand.ExpressApp.Win.SystemModule{
    public class CloseWindowController:WindowController{
        public event EventHandler<FormClosingEventArgs> FormClosing;
        public event EventHandler<HandledEventArgs> CanClose;
        public event EventHandler<CancelEventArgs> QueryCanClose;
        
        protected virtual void OnQueryCanClose(CancelEventArgs e) {
            var handler = QueryCanClose;
            if (handler != null) handler(this, e);
        }

        protected virtual void OnFormClosing(object sender, FormClosingEventArgs e){
            EventHandler<FormClosingEventArgs> handler = FormClosing;
            if (handler != null) handler(sender, e);
        }

        protected virtual void OnCanClose(HandledEventArgs e){
            EventHandler<HandledEventArgs> handler = CanClose;
            if (handler != null) handler(this, e);
        }

        private static bool _editing;
        bool _isLoggingOff;
        public static bool MainFormClosing;


        protected override void OnFrameAssigned() {
            base.OnFrameAssigned();
            Frame.TemplateChanged += FrameOnTemplateChanged;
            Frame.TemplateViewChanged+=FrameOnTemplateViewChanged;
            Frame.Disposing += FrameOnDisposing;
        }

        void FrameOnDisposing(object sender, EventArgs eventArgs) {
            Frame.TemplateViewChanged -= FrameOnTemplateViewChanged;
            Frame.Disposing -= FrameOnDisposing;
            Frame.TemplateChanged -= FrameOnTemplateChanged;
            Frame.GetController<WinModificationsController>().SaveAndCloseAction.Execute -= SaveAndCloseActionOnExecute;
        }

        private void FrameOnTemplateViewChanged(object sender, EventArgs eventArgs){
            var form = Frame.Template as XtraForm;
            if (form!=null){
                form.FormClosing -= FormOnFormClosing;
                form.Closing -= OnClosing;
                var handledEventArgs = new HandledEventArgs();
                OnCanClose(handledEventArgs);
                if (handledEventArgs.Handled){
                    form.FormClosing += FormOnFormClosing;
                    form.Closing += OnClosing;
                }
            }
        }

        private void FrameOnTemplateChanged(object sender, EventArgs args) {
            var mainForm = Frame.Template as MainForm;
            if (mainForm != null){
                mainForm.Closing -= MainFormOnClosing;
                mainForm.Closing += MainFormOnClosing;
                mainForm.Shown += (o, eventArgs) => { MainFormClosing = false; };
            }
            
            var form = Frame.Template as XtraForm;
            if (form != null) {
                Frame.GetController<WinModificationsController>().SaveAndCloseAction.Execute += SaveAndCloseActionOnExecute;
                Application.LoggingOff += ApplicationOnLoggingOff;
                Application.LoggedOff += ApplicationOnLoggedOff;
                    
                var editModelAction = Frame.GetController<DevExpress.ExpressApp.Win.SystemModule.EditModelController>().EditModelAction;
                editModelAction.Executing += (o, eventArgs) => _editing = true;
                editModelAction.ExecuteCompleted += (o, eventArgs) => _editing = false;
            }
        }

        private void SaveAndCloseActionOnExecute(object sender, SimpleActionExecuteEventArgs e){
            if (Frame != null) ((XtraForm) Frame.Template).Close();
        }

        private void MainFormOnClosing(object sender, CancelEventArgs cancelEventArgs){
            MainFormClosing = !cancelEventArgs.Cancel;
        }

        void ApplicationOnLoggedOff(object sender, EventArgs eventArgs) {
            _isLoggingOff = false;
        }

        void ApplicationOnLoggingOff(object sender, LoggingOffEventArgs loggingOffEventArgs) {
            _isLoggingOff = !loggingOffEventArgs.Cancel;
        }

        void OnClosing(object sender, CancelEventArgs cancelEventArgs) {
            if (!_editing && !_isLoggingOff){
                cancelEventArgs.Cancel = true;
            }
        }

        private void FormOnFormClosing(object sender, FormClosingEventArgs e) {
            if (!_editing && !_isLoggingOff) {
                if (Frame.View!=null){
                    if (Frame.View.ObjectSpace.IsModified){
                        var confirmationResult = Frame.Application.AskConfirmation(ConfirmationType.NeedSaveChanges);
                        if (confirmationResult == ConfirmationResult.Cancel)
                            return;
                        if (confirmationResult == ConfirmationResult.Yes)
                            Frame.View.ObjectSpace.CommitChanges();
                        else if (confirmationResult == ConfirmationResult.No)
                            Frame.View.ObjectSpace.RollbackSilent();
                    }
                    e.Cancel = e.CloseReason == CloseReason.UserClosing;
                    if (e.Cancel)
                        OnFormClosing(sender, e);
                }
            }
        }
    }
}