using System;
using System.ComponentModel;
using System.Windows.Forms;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;

namespace Xpand.ExpressApp.Win.SystemModule {

    public class CloseFormController : WindowController{
        public event EventHandler<CancelEventArgs> Close;
        public event EventHandler<CancelEventArgs> Cancel;
        private Form _form;
        private bool _cancel;
        private static bool _modelEditing;
        private static bool _isloggingOff;
        private DevExpress.ExpressApp.Win.SystemModule.EditModelController _editModelController;

        protected virtual void OnCancel(CancelEventArgs e) {
            var handler = Cancel;
            if (handler != null) handler(this, e);
        }

        public Form Form{
            get { return _form; }
        }

        public static bool IsloggingOff{
            get { return _isloggingOff; }
        }

        public static bool IsModelEditing{
            get { return _modelEditing; }
        }

        public static bool CanCancel{
            get { return !IsloggingOff&&!IsModelEditing; }
        }

        protected virtual void OnClose(CancelEventArgs e) {
            var handler = Close;
            if (handler != null) handler(this, e);
        }

        protected override void OnFrameAssigned(){
            base.OnFrameAssigned();
            Frame.Disposing+=FrameOnDisposing;
            Frame.TemplateChanged+=FrameOnTemplateChanged;
            if (Frame.Context==TemplateContext.ApplicationWindow){
                Application.LoggingOff += ApplicationOnLoggingOff;
                Application.LoggedOff+=ApplicationOnLoggedOff;
                _editModelController = Frame.GetController<DevExpress.ExpressApp.Win.SystemModule.EditModelController>();
                _editModelController.EditModelAction.Executing += EditModelActionOnExecuting;
                _editModelController.EditModelAction.ExecuteCompleted += EditModelActionOnExecuteCompleted;
            }
        }

        private void FrameOnDisposing(object sender, EventArgs eventArgs){
            if (_form != null){
                Frame.TemplateChanged -= FrameOnTemplateChanged;
                Frame.Disposing-=FrameOnDisposing;
                _form.Closing -= OnClosing;
                _form.FormClosing-=FormOnFormClosing;
                if (Frame.Context == TemplateContext.ApplicationWindow) {
                    _editModelController.EditModelAction.Executing -= EditModelActionOnExecuting;
                    _editModelController.EditModelAction.ExecuteCompleted -= EditModelActionOnExecuteCompleted;
                }
            }
        }

        private void ApplicationOnLoggedOff(object sender, EventArgs eventArgs){
            var xafApplication = ((XafApplication) sender);
            xafApplication.LoggingOff -= ApplicationOnLoggingOff;
            xafApplication.LoggedOff -= ApplicationOnLoggedOff;
            _isloggingOff = false;
        }

        private void FrameOnTemplateChanged(object sender, EventArgs eventArgs){
            var form= Frame.Template as Form;
            if (form!=null){
                _form = form;
                _form.Closing+=OnClosing;
                _form.FormClosing+=FormOnFormClosing;
            }
        }

        private void ApplicationOnLoggingOff(object sender, LoggingOffEventArgs loggingOffEventArgs){
            _isloggingOff = true;
        }

        private void EditModelActionOnExecuting(object sender, CancelEventArgs cancelEventArgs){
            _modelEditing = true;
        }

        private void EditModelActionOnExecuteCompleted(object sender, ActionBaseEventArgs e){
            _modelEditing = false;
        }

        private void FormOnFormClosing(object sender, FormClosingEventArgs e){
            if (_cancel){
                _cancel = false;
                e.Cancel = e.CloseReason == CloseReason.UserClosing;
                if (e.Cancel){
                    OnClose(e);
                }
            }
        }

        private void OnClosing(object sender, CancelEventArgs e){
            if (CanCancel) {
                OnCancel(e);
                _cancel = e.Cancel;
            }
        }
    }
}
