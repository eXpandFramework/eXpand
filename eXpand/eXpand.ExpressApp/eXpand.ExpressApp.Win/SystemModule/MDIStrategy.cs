using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows.Forms;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.SystemModule;
using DevExpress.ExpressApp.Win;

namespace eXpand.ExpressApp.Win.SystemModule {
    public class MDIStrategy : WinShowViewStrategyBase {
        readonly List<WinWindow> childWindows = new List<WinWindow>();
        readonly List<WinWindow> delayedToShow = new List<WinWindow>();
        int modalCount;

        public MDIStrategy(XafApplication application) : base(application) {
        }

        WinWindow FindChildWindowByForm(Form form) {
            return childWindows.FirstOrDefault(window => window.Form == form);
        }

        bool CanCloseMDIParentWindow() {
            return childWindows.All(window => window.View == null || window.View.CanClose());
        }

        void window_Closing(object sender, CancelEventArgs e) {
            e.Cancel = true;
        }

        void window_Closed(object sender, EventArgs e) {
            var window = (WinWindow)sender;
            window.Form.FormClosing -= Form_FormClosing;
            window.Closing -= window_Closing;
            window.Closed -= window_Closed;
            window.Form.Activated -= Form_Activated;
            childWindows.Remove(window);
        }

        void Form_FormClosing(object sender, FormClosingEventArgs e) {
            var form = sender as Form;
            WinWindow window = FindChildWindowByForm(form);
            e.Cancel = false;
            if (e.CloseReason == CloseReason.MdiFormClosing) {
                if (window.Form.MdiParent.MdiChildren[0] == window.Form) {
                    e.Cancel = !CanCloseMDIParentWindow();
                }
                if (!e.Cancel) {
                    try {
                        window.Form.FormClosing -= Form_FormClosing;
                        window.View.SynchronizeInfo();
                        if (!window.View.Close(false)) {
                            window.Form.FormClosing += Form_FormClosing;
                        }
                    }
                    catch {
                        window.Form.FormClosing += Form_FormClosing;
                        throw;
                    }
                }
            }
            else {
                e.Cancel = !window.CanClose();
            }
        }

        //B39290
        void Form_Activated(object sender, EventArgs e) {
            var controller = Application.MainWindow.GetController<ShowNavigationItemController>();
            if (controller != null) {
                WinWindow window = FindChildWindowByForm((Form) sender);
                if (window != null) {
                    controller.UpdateSelectedItem(window.View);
                }
            }
        }

        // B135637
        protected override void ShowViewInModalWindow(ShowViewParameters parameters, ShowViewSource showViewSource) {
            modalCount++;
            try {
                base.ShowViewInModalWindow(parameters, showViewSource);
            }
            finally {
                modalCount--;
            }
        }

        protected override void ShowViewFromCommonView(ShowViewParameters parameters, ShowViewSource showViewSource) {
            WinWindow existWindow = FindWindowByView(parameters.CreatedView);
            if (existWindow != null) {
                parameters.CreatedView.Dispose();
                parameters.CreatedView = existWindow.View;
                existWindow.Show();
            }
            else {
                ShowViewInNewWindow(parameters, showViewSource);
            }
        }

        protected override void ShowViewCore(ShowViewParameters parameters, ShowViewSource showViewSource) {
            if (parameters.TargetWindow == TargetWindow.Current && showViewSource.SourceFrame == MainWindow) {
                parameters.TargetWindow = TargetWindow.Default;
            }
            if (modalCount > 0) {
                parameters.TargetWindow = TargetWindow.NewModalWindow;
            }
            base.ShowViewCore(parameters, showViewSource);
        }

        protected override void ShowViewFromLookupView(ShowViewParameters parameters, ShowViewSource showViewSource) {
            ShowViewInModalWindow(parameters, showViewSource);
        }

        protected override void BeforeShowWindow(WinWindow window) {
            if (window != MainWindow && window.Context != TemplateContext.PopupWindow) {
                if (window.Form.MdiParent != MainWindow.Form) {
                    window.Form.MdiParent = MainWindow.Form;
                    window.Closing += window_Closing;
                    window.Form.FormClosing += Form_FormClosing;
                    window.Form.Activated += Form_Activated;
                    window.Closed += window_Closed;
                    childWindows.Add(window);
                }
            }
        }

        protected override void ShowWindow(WinWindow window) {
            if (!window.IsMain && MainWindow == null) {
                delayedToShow.Add(window);
            }
            else {
                base.ShowWindow(window);
            }
        }

        public override void ShowStartupWindow() {
            delayedToShow.Clear();
            try {
                base.ShowStartupWindow();
            }
            finally {
                System.Windows.Forms.Application.DoEvents();
                foreach (WinWindow child in delayedToShow) {
                    ShowWindow(child);
                }
            }
        }
    }
}