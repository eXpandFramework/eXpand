using System;
using System.ComponentModel;
using System.Linq;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Model.Core;
using DevExpress.ExpressApp.Model.NodeGenerators;
using DevExpress.ExpressApp.Web;
using DevExpress.ExpressApp.Win.SystemModule;
using DevExpress.XtraBars.Docking;
using Xpand.Persistent.Base.General.Controllers.Actions;
using Xpand.Persistent.Base.General.Model;

namespace Xpand.Persistent.Base.General.Controllers {
    public interface IModelOptionsNavigationContainer{
        [Category(AttributeCategoryNameProvider.Xpand)]
        bool HideNavigationOnStartup { get; set; }
    }

    public class NavigationContainerController:ViewController,IModelExtender {
        public const string ToggleNavigationId = "ToggleNavigation";
        private readonly SimpleAction _toggleNavigation;

        protected NavigationContainerController() {
            _toggleNavigation = new SimpleAction(this, ToggleNavigationId, "Hidden");
        }

        public SimpleAction ToggleNavigation{
            get { return _toggleNavigation; }
        }

        public void ExtendModelInterfaces(ModelInterfaceExtenders extenders){
            extenders.Add<IModelOptions,IModelOptionsNavigationContainer>();
        }
    }
    public class ToggleToolboxUpdater : ModelNodesGeneratorUpdater<ModelActionsNodesGenerator> {
        public override void UpdateNode(ModelNode node){
            var modelAction = node.Application.ActionDesign.Actions[NavigationContainerController.ToggleNavigationId] as IModelActionClientScript;
            if (modelAction != null) {
                modelAction.ClientScript = "OnClick('LPcell','separatorImage',true);";
            }
        }
    }

    public class NavigationContainerWinController : NavigationContainerController {
        public NavigationContainerWinController(){
            ToggleNavigation.Execute += ToggleNavigationOnExecute;
        }

        protected override void OnFrameAssigned(){
            base.OnFrameAssigned();
            if (((IModelOptionsNavigationContainer)Application.Model.Options).HideNavigationOnStartup)
                Application.CustomizeTemplate += Application_CustomizeTemplate;
        }

        private void ToggleNavigationOnExecute(object sender, SimpleActionExecuteEventArgs simpleActionExecuteEventArgs){
            var navigationPanel = GetNavigationPanel((IDockManagerHolder)Application.MainWindow.Template);
            navigationPanel.Visibility = navigationPanel.Visibility==DockVisibility.Visible ? DockVisibility.Hidden : DockVisibility.Visible;
            System.Windows.Forms.Application.DoEvents();
        }

        private void Application_CustomizeTemplate(object sender, CustomizeTemplateEventArgs e) {
            if ((Frame.Template == null || Frame.Template == e.Template) && e.Template is IDockManagerHolder) {
                Application.CustomizeTemplate -= Application_CustomizeTemplate;
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
        public NavigationContainerWebController(){
        }

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

