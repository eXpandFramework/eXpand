using System;
using System.IO;
using System.Text;
using System.Web;
using System.Xml.Linq;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Utils;
using DevExpress.ExpressApp.Web.SystemModule;
using Xpand.ExpressApp.Dashboard.BusinessObjects;
using Xpand.ExpressApp.Dashboard.Controllers;
using Xpand.ExpressApp.Dashboard.Services;
using Xpand.ExpressApp.XtraDashboard.Web.BusinessObjects;

namespace Xpand.ExpressApp.XtraDashboard.Web.Controllers {
    public class DashboarDesignerController : DashboardDesignerController {
        protected override void DashboardEditExecute(object sender, SimpleActionExecuteEventArgs e){
            base.DashboardEditExecute(sender, e);
            var definition = ((IDashboardDefinition) View.CurrentObject);
            var dashboard = definition.GetDashboard(Application, RuleMode.DesignTime,modeParametersEdited: EditDashboard);
            if (dashboard!=null)
                EditDashboard();
            
        }

        private void EditDashboard(){
            var modelView = (IModelDetailView) Application.Model.Views[DashboardDefinition.DashboardDesignerDetailView];
            var objectSpace = Application.CreateObjectSpace(modelView.ModelClass.TypeInfo.Type);
            var definition = (IDashboardDefinition) objectSpace.GetObject(View.CurrentObject);
            var detailView = Application.CreateDetailView(objectSpace, modelView, true, definition);
            
            detailView.Closed+=CreatedViewOnClosed;
            Application.ShowViewStrategy.ShowViewInPopupWindow(detailView);
        }

        private void CreatedViewOnClosed(object sender, EventArgs eventArgs){
            ((View) sender).Closed-=CreatedViewOnClosed;
            ObjectSpace.Refresh();
        }

        protected override void DashbardExportXMLExecute(object sender, SimpleActionExecuteEventArgs e){
            base.DashbardExportXMLExecute(sender, e);
            var stream = new MemoryStream();
            var document = XDocument.Parse(((IDashboardDefinition) View.CurrentObject).Xml);
            document.Save(stream);
            HttpContext.Current.Response.ClearHeaders();
            ResponseWriter.WriteFileToResponse(stream, CaptionHelper.GetClassCaption(View.ObjectTypeInfo.Type.FullName) + ".xml");
        }

        protected override void DashboardImportXMLExecute(object sender, SimpleActionExecuteEventArgs e){
            base.DashboardImportXMLExecute(sender, e);
            var showViewParameters = e.ShowViewParameters;
            var objectSpace = Application.CreateObjectSpace(typeof(DashboardFileData));
            var fileData = objectSpace.CreateObject<DashboardFileData>();
            showViewParameters.CreatedView = Application.CreateDetailView(objectSpace, fileData);
            showViewParameters.TargetWindow=TargetWindow.NewModalWindow;
            showViewParameters.CreatedView.Closing+=CreatedViewOnClosing;
        }

        private void CreatedViewOnClosing(object sender, EventArgs eventArgs){
            var content = ((DashboardFileData) ((View) sender).CurrentObject).FileData.Content;
            ((IDashboardDefinition) View.CurrentObject).Xml = Encoding.UTF8.GetString(content);
            ObjectSpace.CommitChanges();
        }
    }
}
