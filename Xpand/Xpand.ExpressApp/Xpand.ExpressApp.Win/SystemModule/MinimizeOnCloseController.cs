using System;
using System.ComponentModel;
using System.Windows.Forms;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.Model;
using DevExpress.XtraEditors;

namespace Xpand.ExpressApp.Win.SystemModule {
    public interface IModelOptionsMinimizeOnCloseOptions : IModelNode {
        [Category("eXpand")]
        [Description("Minimize application on closing instead of closing")]
        bool MinimizeOnClose { get; set; }
    }

    public class MinimizeOnCloseController : WindowController, IModelExtender {
        private static bool _editing;
        bool _isLoggingOff;

        protected override void OnFrameAssigned() {
            base.OnFrameAssigned();
            Frame.TemplateChanged += FrameOnTemplateChanged;
            Frame.Disposing+=FrameOnDisposing;
        }

        void FrameOnDisposing(object sender, EventArgs eventArgs) {
            Frame.Disposing-=FrameOnDisposing;
            Frame.TemplateChanged-=FrameOnTemplateChanged;
        }

        private void FrameOnTemplateChanged(object sender, EventArgs args) {
            if (Frame.Context == TemplateContext.ApplicationWindow &&
                ((IModelOptionsMinimizeOnCloseOptions)Application.Model.Options).MinimizeOnClose) {
                var form = Frame.Template as XtraForm;
                if (form != null) {
                    Application.LoggingOff+=ApplicationOnLoggingOff;
                    Application.LoggedOff+=ApplicationOnLoggedOff;
                    form.FormClosing += FormOnFormClosing;
                    form.Closing += FormOnClosing;
                    SimpleAction action =
                    Frame.GetController<DevExpress.ExpressApp.Win.SystemModule.EditModelController>().EditModelAction;
                    action.Executing += (o, eventArgs) => _editing = true;
                    action.ExecuteCompleted += (o, eventArgs) => _editing = false;
                }
            }
        }

        void ApplicationOnLoggedOff(object sender, EventArgs eventArgs) {
            _isLoggingOff = false;
        }

        void ApplicationOnLoggingOff(object sender, LoggingOffEventArgs loggingOffEventArgs) {
            _isLoggingOff = !loggingOffEventArgs.Cancel;
        }

        void FormOnClosing(object sender, CancelEventArgs cancelEventArgs) {
            if (!_editing&&!_isLoggingOff)
                cancelEventArgs.Cancel = true;
        }

        private void FormOnFormClosing(object sender, FormClosingEventArgs e) {
            if (!_editing && !_isLoggingOff) {
                if (Application != null)
                    e.Cancel = ((IModelOptionsMinimizeOnCloseOptions)Application.Model.Options).MinimizeOnClose && e.CloseReason == CloseReason.UserClosing;

                if (e.Cancel)
                    ((XtraForm)sender).Hide();
            }
        }

        void IModelExtender.ExtendModelInterfaces(ModelInterfaceExtenders extenders) {
            extenders.Add<IModelOptions, IModelOptionsMinimizeOnCloseOptions>();
        }
    }
}
