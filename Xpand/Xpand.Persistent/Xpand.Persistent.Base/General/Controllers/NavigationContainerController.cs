using System.ComponentModel;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.Model;
using Xpand.Persistent.Base.General.Model;

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
}