using System;
using System.ComponentModel;
using System.Windows.Forms;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Utils;
using DevExpress.ExpressApp.Win;
using DevExpress.ExpressApp.Win.SystemModule;
using DevExpress.XtraEditors;

namespace Xpand.ExpressApp.Win.SystemModule {
    public interface IModelOptionsPromptOnExit {
        [Category("eXpand")]
        bool PromptOnExit { get; set; }
    }
    public class PromtOnExitController : WindowController, IModelExtender {
        static bool enableEventHandling = true;
        volatile bool _editing;

        protected override void OnActivated() {
            base.OnActivated();
            var editModelAction = Frame.GetController<EditModelController>().EditModelAction;
            editModelAction.Executing += EditModelActionOnExecuting;
            editModelAction.ExecuteCompleted += EditModelActionOnExecuteCompleted;
        }

        void EditModelActionOnExecuteCompleted(object sender, ActionBaseEventArgs actionBaseEventArgs) {
            _editing = false;
        }

        void EditModelActionOnExecuting(object sender, CancelEventArgs cancelEventArgs) {
            _editing = true;
        }


        void OnWindowClosing(Object sender, CancelEventArgs e) {
            if (_editing) {
                return;
            }
            if (!enableEventHandling) return;
            var ea = (FormClosingEventArgs)e;
            if ((ea.CloseReason == CloseReason.UserClosing && Window.IsMain)) {
                var promptOnExitTitle = CaptionHelper.GetLocalizedText(XpandSystemWindowsFormsModule.XpandWin, "PromptOnExitTitle");
                var promptOnExitMessage = CaptionHelper.GetLocalizedText(XpandSystemWindowsFormsModule.XpandWin, "PromptOnExitMessage");
                bool yes = XtraMessageBox.Show(promptOnExitMessage, promptOnExitTitle, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes;
                e.Cancel = !yes;
                if (yes) {
                    enableEventHandling = false;
                    ((WinWindow)sender).Close();
                }
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