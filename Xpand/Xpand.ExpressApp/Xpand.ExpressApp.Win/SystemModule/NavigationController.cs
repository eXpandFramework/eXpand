using System;
using System.Linq;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.Win.SystemModule;
using DevExpress.XtraBars.Docking;
using Xpand.Persistent.Base.General.Controllers;

namespace Xpand.ExpressApp.Win.SystemModule {



    public class NavigationContainerWinController : NavigationContainerController {

        public NavigationContainerWinController() {
            ToggleNavigation.Execute += ToggleNavigationOnExecute;
        }

        protected override void OnFrameAssigned() {
            base.OnFrameAssigned();
            if (Frame.Context == TemplateContext.ApplicationWindow)
                Application.CustomizeTemplate += Application_CustomizeTemplate;
        }

        private void ToggleNavigationOnExecute(object sender, SimpleActionExecuteEventArgs simpleActionExecuteEventArgs) {
            var dockManager = ((IDockManagerHolder) Application.MainWindow.Template).DockManager;
            var dockPanel = dockManager.Panels.First(panel => panel.Name=="dockPanelNavigation");
            dockPanel.Visibility = dockPanel.Visibility == DockVisibility.Visible ? DockVisibility.Hidden : DockVisibility.Visible;
            System.Windows.Forms.Application.DoEvents();
        }

        private void Application_CustomizeTemplate(object sender, CustomizeTemplateEventArgs e) {
            ((XafApplication) sender).CustomizeTemplate -= Application_CustomizeTemplate;
            if ((e.Template) is IDockManagerHolder dockManagerHolder) dockManagerHolder.DockManager.Load += DockManagerOnLoad;
        }

        private void DockManagerOnLoad(object sender, EventArgs eventArgs) {
            var hideNavigationOnStartup = ((IModelOptionsNavigationContainer)Application.Model.Options).HideNavigationOnStartup;
            if (hideNavigationOnStartup != null && hideNavigationOnStartup.Value){
                var navigationPanel = ((DockManager) sender).Panels.First(panel => panel.Name == "dockPanelNavigation");
                navigationPanel.Visibility =  DockVisibility.AutoHide;
            }
        }
    }
}


