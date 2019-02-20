using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.Web;
using DevExpress.Persistent.Base;
using Xpand.ExpressApp.Dashboard.BusinessObjects;

namespace Xpand.ExpressApp.XtraDashboard.Web.Controllers {
    public class DisplayViewInNewTabController : ObjectViewController<DetailView, IDashboardDefinition> {
        public DisplayViewInNewTabController() {
            TargetViewId = DashboardDefinition.DashboardViewerDetailView;
            DisplayViewInNewTabControllerAction = new SimpleAction(this, "DisplayViewInNewTab", PredefinedCategory.View);
            DisplayViewInNewTabControllerAction.SetClientScript($"window.open('{WebApplication.NoHeaderPopupWindowTemplatePage}'+ window.location.hash,'_blank');", false);
            DisplayViewInNewTabControllerAction.Caption = "Show view in a separate tab";
            DisplayViewInNewTabControllerAction.Active["SupportTabs"] = WebApplication.EnableMultipleBrowserTabsSupport;
            DisplayViewInNewTabControllerAction.ImageName = "Action_Fullscreen";
        }

        public SimpleAction DisplayViewInNewTabControllerAction { get; }
    }
}