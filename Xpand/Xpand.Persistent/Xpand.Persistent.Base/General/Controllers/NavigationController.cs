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

    public class NavigationContainerController:ViewController,IModelExtender {
        private readonly SimpleAction _toggleNavigation;
        public const string ToggleNavigationId = "ToggleNavigation";

        public NavigationContainerController() {
            _toggleNavigation = new SimpleAction(this, ToggleNavigationId, "Hidden");
        }

        public SimpleAction ToggleNavigation        {
            get { return _toggleNavigation; }
        }

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
        public NavigationContainerWinController(){
            ToggleNavigation.Execute += ToggleNavigationOnExecute;
        }

        protected override void OnFrameAssigned(){
            base.OnFrameAssigned();
            if (((IModelOptionsNavigationContainer)Application.Model.Options).HideNavigationOnStartup.HasValue)
                Application.CustomizeTemplate += Application_CustomizeTemplate;
        }

        private void ToggleNavigationOnExecute(object sender, SimpleActionExecuteEventArgs simpleActionExecuteEventArgs){
            var navigationPanel = GetNavigationPanel((IDockManagerHolder)Application.MainWindow.Template);
            navigationPanel.Visibility = navigationPanel.Visibility==DockVisibility.Visible ? DockVisibility.Hidden : DockVisibility.Visible;
            System.Windows.Forms.Application.DoEvents();
        }

        private void Application_CustomizeTemplate(object sender, CustomizeTemplateEventArgs e) {
            if (Frame != null && ((Frame.Template == null || Frame.Template == e.Template) && e.Template is IDockManagerHolder)) {
                Application.CustomizeTemplate -= Application_CustomizeTemplate;
                var dockManagerHolder = ((IDockManagerHolder)e.Template);
                var dockPanel = GetNavigationPanel(dockManagerHolder);
                if (dockPanel != null){
                    var hideNavigationOnStartup = ((IModelOptionsNavigationContainer)Application.Model.Options).HideNavigationOnStartup;
                    dockPanel.Visibility = hideNavigationOnStartup != null && hideNavigationOnStartup.Value?DockVisibility.AutoHide:DockVisibility.Visible;
                }
            }
        }

        private DockPanel GetNavigationPanel(IDockManagerHolder dockManagerHolder){
            return dockManagerHolder.DockManager.Panels.FirstOrDefault(panel => panel.Name == "dockPanelNavigation");
        }
    }
}


