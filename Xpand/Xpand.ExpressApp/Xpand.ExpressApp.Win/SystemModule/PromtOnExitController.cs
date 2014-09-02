//using System;
//using System.ComponentModel;
//using System.Windows.Forms;
//using DevExpress.ExpressApp;
//using DevExpress.ExpressApp.Actions;
//using DevExpress.ExpressApp.Model;
//using DevExpress.ExpressApp.Utils;
//using DevExpress.ExpressApp.Win;
//using DevExpress.ExpressApp.Win.SystemModule;
//using DevExpress.XtraBars.Ribbon;
//using DevExpress.XtraEditors;
//
//namespace Xpand.ExpressApp.Win.SystemModule {
//    public interface IModelOptionsPromptOnExit {
//        [Category("eXpand.OnClose")]
//        bool PromptOnExit { get; set; }
//    }
//    public class PromtOnExitController : WindowController, IModelExtender {
//        static bool _enableEventHandling = true;
//        volatile bool _editing;
//        bool _isLoggingOff;
//
//        public PromtOnExitController() {
//            TargetWindowType=WindowType.Main;
//        }
//
//        protected override void OnActivated() {
//            base.OnActivated();
//            Application.LoggingOff+=ApplicationOnLoggingOff;
//            Application.LoggedOff+=ApplicationOnLoggedOff;
//            var editModelAction = Frame.GetController<EditModelController>().EditModelAction;
//            editModelAction.Executing += EditModelActionOnExecuting;
//            editModelAction.ExecuteCompleted += EditModelActionOnExecuteCompleted;
//        }
//
//        void ApplicationOnLoggedOff(object sender, EventArgs eventArgs) {
//            var xafApplication = ((XafApplication) sender);
//            _isLoggingOff = false;
//            xafApplication.LoggingOff-=ApplicationOnLoggingOff;
//            xafApplication.LoggedOff-=ApplicationOnLoggedOff;
//        }
//
//        void ApplicationOnLoggingOff(object sender, LoggingOffEventArgs loggingOffEventArgs) {
//            _isLoggingOff = true;
//        }
//
//        void EditModelActionOnExecuteCompleted(object sender, ActionBaseEventArgs actionBaseEventArgs) {
//            _editing = false;
//        }
//
//        void EditModelActionOnExecuting(object sender, CancelEventArgs cancelEventArgs) {
//            _editing = true;
//        }
//
//        void OnWindowClosing(Object sender, CancelEventArgs e) {
//            if (_editing || _isLoggingOff) {
//                return;
//            }
//            if (!_enableEventHandling) return;
//            var ea = (FormClosingEventArgs)e;
//            if ((ea.CloseReason == CloseReason.UserClosing && Window.IsMain && CanPrompt())) {
//                var promptOnExitTitle = CaptionHelper.GetLocalizedText(XpandSystemWindowsFormsModule.XpandWin, "PromptOnExitTitle");
//                var promptOnExitMessage = CaptionHelper.GetLocalizedText(XpandSystemWindowsFormsModule.XpandWin, "PromptOnExitMessage");
//                bool yes = XtraMessageBox.Show(promptOnExitMessage, promptOnExitTitle, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes;
//                e.Cancel = !yes;
//            }
//        }
//
//        bool CanPrompt() {
//            var modelOptionsWin = ((IModelOptionsWin) Application.Model.Options);
//            return modelOptionsWin.FormStyle != RibbonFormStyle.Ribbon || modelOptionsWin.UIType != UIType.TabbedMDI ||
//                   ((WinShowViewStrategyBase) Application.ShowViewStrategy).Explorers.Count == 1;
//        }
//
//        void OnWindowClosed(object sender, EventArgs e) {
//            ((WinWindow)sender).Closing -= OnWindowClosing;
//            ((WinWindow)sender).Closed -= OnWindowClosed;
//        }
//
//        void OnWindowTemplateChanged(object sender, EventArgs e) {
//            Window.TemplateChanged -= OnWindowTemplateChanged;
//            _enableEventHandling = ((IModelOptionsPromptOnExit)Application.Model.Options).PromptOnExit;
//            ((WinWindow)Window).Closing += OnWindowClosing;
//            ((WinWindow)Window).Closed += OnWindowClosed;
//        }
//
//        protected override void OnWindowChanging(Window window) {
//            base.OnWindowChanging(window);
//            window.TemplateChanged += OnWindowTemplateChanged;
//        }
//
//        void IModelExtender.ExtendModelInterfaces(ModelInterfaceExtenders extenders) {
//            extenders.Add<IModelOptions, IModelOptionsPromptOnExit>();
//        }
//    }
//}