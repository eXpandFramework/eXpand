using System.ComponentModel;
using System.Windows.Forms;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Model;
using DevExpress.XtraEditors;

namespace Xpand.ExpressApp.Win.SystemModule {
    public interface IModelOptionsMinimizeOnCloseOptions : IModelNode {
        [Category("eXpand")]
        [Description("Minimize application on closing instead of closing")]
        bool MinimizeOnClose { get; set; }
        [Category("eXpand")]
        [Description("Hide application on closing instead of closing")]
        bool HideOnClose { get; set; }
    }

    public class MinimizeOnCloseController : WindowController, IModelExtender {
        private CloseWindowController _closeWindowController;

        protected override void OnActivated(){
            base.OnActivated();
            _closeWindowController = Frame.GetController<CloseWindowController>();
            _closeWindowController.CanClose+=OnCanClose;
        }

        protected override void OnDeactivated(){
            base.OnDeactivated();
            _closeWindowController.CanClose -= OnCanClose;
            _closeWindowController.FormClosing -= CloseWindowControllerOnFormClosing;
        }

        private void CloseWindowControllerOnFormClosing(object sender, FormClosingEventArgs formClosingEventArgs){
            if (((IModelOptionsMinimizeOnCloseOptions)Application.Model.Options).MinimizeOnClose)
                ((XtraForm)sender).WindowState = FormWindowState.Minimized;
            if (((IModelOptionsMinimizeOnCloseOptions) Application.Model.Options).HideOnClose)
                ((XtraForm)sender).Hide();
        }

        private void OnCanClose(object sender, HandledEventArgs handledEventArgs){
            if (!handledEventArgs.Handled){
                bool handled = Frame.Context == TemplateContext.ApplicationWindow &&((IModelOptionsMinimizeOnCloseOptions) Application.Model.Options).MinimizeOnClose;
                handledEventArgs.Handled = handled;
                if (handled)
                    _closeWindowController.FormClosing += CloseWindowControllerOnFormClosing;
            }
        }

        void IModelExtender.ExtendModelInterfaces(ModelInterfaceExtenders extenders) {
            extenders.Add<IModelOptions, IModelOptionsMinimizeOnCloseOptions>();
        }
    }
}
