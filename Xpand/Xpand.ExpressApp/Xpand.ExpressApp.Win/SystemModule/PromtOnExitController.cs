using System;
using System.ComponentModel;
using System.Windows.Forms;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Win;
using DevExpress.XtraEditors;

namespace Xpand.ExpressApp.Win.SystemModule {
    public interface IModelOptionsPromptOnExit {
        [Category("eXpand")]
        bool PromptOnExit { get; set; }
    }
    public class PromtOnExitController : WindowController, IModelExtender {
        static bool enableEventHandling = true;

        void OnWindowClosing(Object sender, CancelEventArgs e) {
            if (!enableEventHandling) return;
            var ea = (FormClosingEventArgs)e;
            if (ea.CloseReason == CloseReason.UserClosing && Window.IsMain) {
                bool yes = XtraMessageBox.Show("You are about to exit the application. Do you want to proceed?", "Exit",
                                               MessageBoxButtons.YesNo, MessageBoxIcon.Information) ==
                           DialogResult.Yes;
                e.Cancel = !yes;
                if (yes) {
                    enableEventHandling = false;
                    ((WinWindow)sender).Close();
                    enableEventHandling = true;
                }
                e.Cancel = true;
            }
            if (ea.CloseReason == CloseReason.MdiFormClosing && !Window.IsMain) {
                e.Cancel = true;
            }
        }

        void OnWindowClosed(object sender, EventArgs e) {
            ((WinWindow)sender).Closing -= OnWindowClosing;
            ((WinWindow)sender).Closed -= OnWindowClosed;
        }

        void OnWindowTemplateChanged(object sender, EventArgs e) {
            Window.TemplateChanged -= OnWindowTemplateChanged;
            enableEventHandling = ((IModelOptionsPromptOnExit)Application.Model.Options).PromptOnExit;
            ((WinWindow)Window).Closing += OnWindowClosing;
            ((WinWindow)Window).Closed += OnWindowClosed;
        }

        protected override void OnWindowChanging(Window window) {
            base.OnWindowChanging(window);
            window.TemplateChanged += OnWindowTemplateChanged;
        }

        void IModelExtender.ExtendModelInterfaces(ModelInterfaceExtenders extenders) {
            extenders.Add<IModelOptions, IModelOptionsPromptOnExit>();
        }
    }
}