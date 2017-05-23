using System;
using System.ComponentModel;
using System.Linq;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Model.Core;
using DevExpress.ExpressApp.Model.NodeGenerators;
using DevExpress.ExpressApp.Win.SystemModule;
using DevExpress.XtraBars.Docking;
using Xpand.Persistent.Base.General.Model;
using Xpand.Persistent.Base.General.Web;

namespace Xpand.Persistent.Base.General.Controllers {
    public interface IModelOptionsNavigationContainer{
        [Category(AttributeCategoryNameProvider.XpandNavigation)]
        [Description("Overrides NavigationAlwaysVisibleOnStartup")]
        bool? HideNavigationOnStartup { get; set; }
        [Category(AttributeCategoryNameProvider.XpandNavigation)]
        [DefaultValue(true)]
        bool NavigationAlwaysVisibleOnStartup { get; set; }
    }

    public class NavigationContainerController:WindowController,IModelExtender {
        public const string ToggleNavigationId = "ToggleNavigation";

        public NavigationContainerController() {
            ToggleNavigation = new SimpleAction(this, ToggleNavigationId, "Hidden");
            TargetWindowType=WindowType.Main;
        }

        public SimpleAction ToggleNavigation{ get; }

        public void ExtendModelInterfaces(ModelInterfaceExtenders extenders){
            extenders.Add<IModelOptions,IModelOptionsNavigationContainer>();
        }
    }

    public class ToggleNavigationActionUpdater : ModelNodesGeneratorUpdater<ModelActionsNodesGenerator> {
        public override void UpdateNode(ModelNode node){
            var modelAction = node.Application.ActionDesign.Actions[NavigationContainerController.ToggleNavigationId] as IModelActionClientScript;
            if (modelAction != null) {
                modelAction.ClientScript = "OnClick('LPcell','separatorImage',true);";
            }
        }
    }

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
            Application.CustomizeTemplate -= Application_CustomizeTemplate;
            var dockManagerHolder = ((IDockManagerHolder)e.Template);
            dockManagerHolder.DockManager.Load += DockManagerOnLoad;
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


