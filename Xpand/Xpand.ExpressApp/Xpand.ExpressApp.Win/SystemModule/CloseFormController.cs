using System;
using System.ComponentModel;
using System.Windows.Forms;
using DevExpress.ExpressApp;

namespace Xpand.ExpressApp.Win.SystemModule {

    public class CloseFormController : WindowController {
        public event EventHandler<CancelEventArgs> Close;
        public event EventHandler<CancelEventArgs> Cancel;
        private Form _form;
        private bool _cancel;
        private static bool _modelEditing;
        private static bool _isloggingOff;
        private DevExpress.ExpressApp.Win.SystemModule.EditModelController _editModelController;

        protected virtual void OnCancel(CancelEventArgs e) {
            var handler = Cancel;
            handler?.Invoke(this, e);
        }

        public Form Form => _form;

        public static bool IsloggingOff => _isloggingOff;

        public static bool IsModelEditing => _modelEditing;

        public static bool IsNotLoggingOffOrModelEditing => !IsloggingOff && !IsModelEditing;

        protected virtual void OnClose(CancelEventArgs e) {
            var handler = Close;
            handler?.Invoke(this, e);
        }

        protected override void OnFrameAssigned() {
            base.OnFrameAssigned();
            _modelEditing = false;
            Frame.Disposing += FrameOnDisposing;
            Frame.TemplateChanged += FrameOnTemplateChanged;
            if (Frame.Context == TemplateContext.ApplicationWindow) {
                Application.LoggingOff += ApplicationOnLoggingOff;
                Application.LoggedOff += ApplicationOnLoggedOff;
                _editModelController = Frame.GetController<DevExpress.ExpressApp.Win.SystemModule.EditModelController>();
                _editModelController.EditModelAction.Executing += EditModelActionOnExecuting;
            }
        }

        private void FrameOnDisposing(object sender, EventArgs eventArgs) {
            if (_form != null) {
                Frame.TemplateChanged -= FrameOnTemplateChanged;
                Frame.Disposing -= FrameOnDisposing;
                _form.FormClosing -= OnClosing;
                _form.FormClosing -= FormOnFormClosing;
                if (Frame.Context == TemplateContext.ApplicationWindow) {
                    _editModelController.EditModelAction.Executing -= EditModelActionOnExecuting;
                }
            }
        }

        private void ApplicationOnLoggedOff(object sender, EventArgs eventArgs) {
            var xafApplication = ((XafApplication)sender);
            xafApplication.LoggingOff -= ApplicationOnLoggingOff;
            xafApplication.LoggedOff -= ApplicationOnLoggedOff;
            _isloggingOff = false;
        }

        private void FrameOnTemplateChanged(object sender, EventArgs eventArgs) {
            if (Frame.Template is Form form) {
                _form = form;
                _form.FormClosing += OnClosing;
                _form.FormClosing += FormOnFormClosing;
            }
        }

        private void ApplicationOnLoggingOff(object sender, LoggingOffEventArgs loggingOffEventArgs) {
            _isloggingOff = true;
        }

        private void EditModelActionOnExecuting(object sender, CancelEventArgs cancelEventArgs) {
            _modelEditing = true;
        }

        private void FormOnFormClosing(object sender, FormClosingEventArgs e) {
            if (_cancel) {
                _cancel = false;
                if (e.CloseReason == CloseReason.UserClosing) {
                    OnClose(e);
                }
            }
        }

        private void OnClosing(object sender, CancelEventArgs e) {
            if (IsNotLoggingOffOrModelEditing) {
                OnCancel(e);
                _cancel = e.Cancel;
            }
        }
    }
}
