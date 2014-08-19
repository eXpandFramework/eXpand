using System;
using System.ComponentModel;
using System.Linq;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Web;
using DevExpress.ExpressApp.Win.SystemModule;
using DevExpress.XtraBars.Docking;
using Xpand.Persistent.Base.General.Model;

namespace Xpand.Persistent.Base.General.Controllers {
    public interface IModelOptionsNavigationContainer{
        [Category(AttributeCategoryNameProvider.Xpand)]
        bool HideNavigationOnStartup { get; set; }
    }
    public class NavigationContainerController:WindowController,IModelExtender {
        public NavigationContainerController() {
            TargetWindowType=WindowType.Main;
        }

        public void ExtendModelInterfaces(ModelInterfaceExtenders extenders){
            extenders.Add<IModelOptions,IModelOptionsNavigationContainer>();
        }
    }

    public class NavigationContainerWinController : NavigationContainerController {
        protected override void OnActivated() {
            base.OnActivated();
            if (((IModelOptionsNavigationContainer)Application.Model.Options).HideNavigationOnStartup)
                Application.CustomizeTemplate += Application_CustomizeTemplate;
        }

        protected override void OnDeactivated() {
            Application.CustomizeTemplate -= Application_CustomizeTemplate;
            base.OnDeactivated();
        }

        private void Application_CustomizeTemplate(object sender, CustomizeTemplateEventArgs e) {
            if ((Window.Template == null || Window.Template == e.Template) && e.Template is IDockManagerHolder) {
                var dockPanel = ((IDockManagerHolder)e.Template).DockManager.RootPanels.FirstOrDefault(panel => panel.Name == "dockPanelNavigation");
                if (dockPanel != null) dockPanel.Visibility = DockVisibility.AutoHide;
            }
        }
    }


    public class NavigationContainerWebController : WindowController {
        protected override void OnFrameAssigned(){
            base.OnFrameAssigned();
            if (((IModelOptionsNavigationContainer)Application.Model.Options).HideNavigationOnStartup)
                WebWindow.CurrentRequestPage.PreRender+=CurrentRequestPageOnInit;
            
        }

        private void CurrentRequestPageOnInit(object sender, EventArgs eventArgs){
            const string script = @"window.onload = function () {
                                        var displayChanged;
                                        var cellElement = document.getElementById('LPcell');
                                        var interval = setInterval(function () {
                                            if (cellElement.style.display != 'none') {
                                                if (displayChanged)
                                                    clearInterval(interval);
                                                displayChanged = true;
                                            }
                                            OnClick('LPcell', 'separatorImage', true);
                                        }, 10);
                                    }";
            WebWindow.CurrentRequestWindow.RegisterStartupScript(GetType().Name, script);
        }
    }
}

