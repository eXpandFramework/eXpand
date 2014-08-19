using System;
using System.ComponentModel;
using System.Linq;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
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
        private readonly SimpleAction _toggleNavigation;

        public NavigationContainerController() {
            TargetWindowType=WindowType.Main;
            _toggleNavigation = new SimpleAction(this, "ToggleNavigation", "Hidden");
            _toggleNavigation.Execute+=ToggleNavigationOnExecute;
        }

        protected virtual void ToggleNavigationOnExecute(object sender, SimpleActionExecuteEventArgs e){
            
        }

        public SimpleAction ToggleNavigation{
            get { return _toggleNavigation; }
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

        protected override void ToggleNavigationOnExecute(object sender, SimpleActionExecuteEventArgs e){
            base.ToggleNavigationOnExecute(sender, e);
            var navigationPanel = GetNavigationPanel((IDockManagerHolder) Frame.Template);
            navigationPanel.Visible = !navigationPanel.Visible;
        }

        protected override void OnDeactivated() {
            Application.CustomizeTemplate -= Application_CustomizeTemplate;
            base.OnDeactivated();
        }

        private void Application_CustomizeTemplate(object sender, CustomizeTemplateEventArgs e) {
            if ((Window.Template == null || Window.Template == e.Template) && e.Template is IDockManagerHolder){
                var dockManagerHolder = ((IDockManagerHolder)e.Template);
                var dockPanel = GetNavigationPanel(dockManagerHolder);
                if (dockPanel != null) 
                    dockPanel.Visibility = DockVisibility.AutoHide;
            }
        }

        private DockPanel GetNavigationPanel(IDockManagerHolder dockManagerHolder){
            return dockManagerHolder.DockManager.Panels.FirstOrDefault(panel => panel.Name == "dockPanelNavigation");
        }
    }


    public class NavigationContainerWebController : NavigationContainerController {
        protected override void OnFrameAssigned(){
            base.OnFrameAssigned();
            if (((IModelOptionsNavigationContainer)Application.Model.Options).HideNavigationOnStartup)
                WebWindow.CurrentRequestPage.PreRender+=CurrentRequestPageOnInit;
            
        }

        protected override void ToggleNavigationOnExecute(object sender, SimpleActionExecuteEventArgs e){
            base.ToggleNavigationOnExecute(sender, e);
            const string script = "OnClick('LPcell','separatorImage',true);";
            WebWindow.CurrentRequestWindow.RegisterClientScript("separatorClick", script);
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

