using System;
using System.ComponentModel;
using System.Windows.Forms;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Win.Templates;
using DevExpress.XtraEditors;

namespace Xpand.ExpressApp.Win.SystemModule{
    public class CloseWindowController:WindowController{
        public event FormClosingEventHandler FormClosing;

        protected virtual void OnFormClosing(object sender, FormClosingEventArgs e){
            FormClosingEventHandler handler = FormClosing;
            if (handler != null) handler(sender, e);
        }

        public event EventHandler<HandledEventArgs> CanClose;

        protected virtual void OnCanClose(HandledEventArgs e){
            EventHandler<HandledEventArgs> handler = CanClose;
            if (handler != null) handler(this, e);
        }

        private static bool _editing;
        bool _isLoggingOff;
        private static bool _mainFormClosing;

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
                mainForm.Shown += (o, eventArgs) => { _mainFormClosing = false; };
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
            _mainFormClosing = !cancelEventArgs.Cancel;
        }

        void ApplicationOnLoggedOff(object sender, EventArgs eventArgs) {
            _isLoggingOff = false;
        }

        void ApplicationOnLoggingOff(object sender, LoggingOffEventArgs loggingOffEventArgs) {
            _isLoggingOff = !loggingOffEventArgs.Cancel;
        }

        void FormOnClosing(object sender, CancelEventArgs cancelEventArgs) {
            if (!_editing && !_isLoggingOff&&!_mainFormClosing)
                cancelEventArgs.Cancel = true;
        }
        
        private void FormOnFormClosing(object sender, FormClosingEventArgs e) {
            if (!_editing && !_isLoggingOff&&!_mainFormClosing) {
                e.Cancel =  e.CloseReason == CloseReason.UserClosing;
                if (e.Cancel)
                    OnFormClosing(sender,e);
            }
        }
    }
}