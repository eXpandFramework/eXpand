using System;
using System.ComponentModel;
using System.Windows.Forms;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Win.Templates;
using DevExpress.XtraEditors;

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
            Frame.Disposing += FrameOnDisposing;
        }

        void FrameOnDisposing(object sender, EventArgs eventArgs) {
            Frame.Disposing -= FrameOnDisposing;
            Frame.TemplateChanged -= FrameOnTemplateChanged;
        }

        private void FrameOnTemplateChanged(object sender, EventArgs args) {
            var mainForm = Frame.Template as MainForm;
            if (mainForm != null){
                mainForm.Closing += MainFormOnClosing;
                mainForm.Shown += (o, eventArgs) => { MainFormClosing = false; };
            }
            var handledEventArgs = new HandledEventArgs();
            OnCanClose(handledEventArgs);
            if (handledEventArgs.Handled) {
                var form = Frame.Template as XtraForm;
                if (form != null) {
                    Application.LoggingOff += ApplicationOnLoggingOff;
                    Application.LoggedOff += ApplicationOnLoggedOff;
                    form.FormClosing += FormOnFormClosing;
                    form.Closing += FormOnClosing;
                    var editModelAction = Frame.GetController<DevExpress.ExpressApp.Win.SystemModule.EditModelController>().EditModelAction;
                    editModelAction.Executing += (o, eventArgs) => _editing = true;
                    editModelAction.ExecuteCompleted += (o, eventArgs) => _editing = false;
                }
            }
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

        void FormOnClosing(object sender, CancelEventArgs cancelEventArgs) {
            if (!_editing && !_isLoggingOff){
                var eventArgs = new CancelEventArgs();
                OnQueryCanClose(eventArgs);
                cancelEventArgs.Cancel = eventArgs.Cancel || !Frame.View.CanClose();
            }
        }

        private void FormOnFormClosing(object sender, FormClosingEventArgs e) {
            if (!_editing && !_isLoggingOff) {
                e.Cancel =  e.CloseReason == CloseReason.UserClosing;
                if (e.Cancel)
                    OnFormClosing(sender,e);
            }
        }
    }
}