using System;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.SystemModule;
using Xpand.ExpressApp.Dashboard.BusinessObjects;
using Xpand.ExpressApp.Dashboard.Services;
using ShowNavigationItemController = Xpand.ExpressApp.Security.Controllers.ShowNavigationItemController;

namespace Xpand.ExpressApp.XtraDashboard.Web.Controllers{
    public class DashboardViewerController : WindowController{
        protected override void OnFrameAssigned(){
            base.OnFrameAssigned();
            if (Frame.Context==TemplateContext.ApplicationWindow){
                Frame.GetController<ShowNavigationItemController>().CustomShowNavigationItem+=OnCustomShowNavigationItem;
                Frame.Disposing+=FrameOnDisposing;
            }
        }

        private void FrameOnDisposing(object sender, EventArgs e){
            Frame.Disposing-=FrameOnDisposing;
            Frame.GetController<ShowNavigationItemController>().CustomShowNavigationItem-=OnCustomShowNavigationItem;
        }

        private void OnCustomShowNavigationItem(object sender, CustomShowNavigationItemEventArgs e){
            if (e.ActionArguments.SelectedChoiceActionItem.Data is ViewShortcut viewShortcut&&viewShortcut.ViewId==DashboardDefinition.DashboardViewerDetailView){
                var objectSpace = Application.CreateObjectSpace(typeof(DashboardDefinition));
                var definition = objectSpace.GetObjectByKey<DashboardDefinition>(Guid.Parse(e.ActionArguments.SelectedChoiceActionItem.Id));
                var dashboard = definition.GetDashboard(Application, RuleMode.DesignTime,modeParametersEdited: () => EditDashboard(definition,e.ActionArguments.Action));
                if (dashboard!=null)
                    EditDashboard(definition,e.ActionArguments.Action);
                e.Handled = true;
            }
        }

        private void EditDashboard(DashboardDefinition dashboardDefinition, ActionBase actionBase){
            var modelView = (IModelDetailView) Application.Model.Views[DashboardDefinition.DashboardViewerDetailView];
            var objectSpace = Application.CreateObjectSpace(modelView.ModelClass.TypeInfo.Type);

            var detailView = Application.CreateDetailView(objectSpace, modelView, true, objectSpace.GetObject(dashboardDefinition));
            Application.ShowViewStrategy.ShowView(new ShowViewParameters(detailView),new ShowViewSource(Frame,actionBase ) );
        }


    }
}