using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Web.UI.WebControls;
using System.Xml;
using System.Xml.Linq;
using DevExpress.DashboardCommon;
using DevExpress.DashboardWeb;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Dashboards;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Web.Editors;
using DevExpress.Persistent.Base;
using DevExpress.Web;
using Xpand.ExpressApp.Dashboard;
using Xpand.ExpressApp.Dashboard.BusinessObjects;
using Xpand.ExpressApp.Dashboard.Services;

namespace Xpand.ExpressApp.XtraDashboard.Web.PropertyEditors {
    [PropertyEditor(typeof(String), false)]
    public class DashboardDesignerEditor : WebPropertyEditor, IComplexViewItem {
        private XafApplication _application;
        private ASPxDashboard _dashboardDesigner;
        private bool _dashboardHasParameters;
        private IObjectSpace _objsectSpace;
        private DevExpress.DashboardCommon.Dashboard _dashboard;

        static DashboardDesignerEditor() {
            DashboardConfigurator.PassCredentials = true;
            
        }
        public DashboardDesignerEditor(Type objectType, IModelMemberViewItem model) : base(objectType, model) {
        }

        protected override object GetControlValueCore() {
            return null;
        }

        private ASPxDashboard GetASPxDashboardDesigner() {
            _dashboardDesigner = new ASPxDashboard {
                Width = Unit.Percentage(100),
                AllowOpenDashboard = false,
                AllowCreateNewDashboard = false,
                IncludeDashboardIdToUrl = false,
                IncludeDashboardStateToUrl = false,
                WorkingMode = GetWorkingMode()
            };
            var xpandDashboardDataProvider = ((XpandDashboardDataProvider) DashboardsModule.DataProvider);
            var dashboardCollectionDataSourceFillService =(IXpandDashboardDataSourceFillService)AttachService(xpandDashboardDataProvider);
            dashboardCollectionDataSourceFillService.FillService.LoadBeforeParameters += (sender, args) =>
                args.Handled = new[]{RuleMode.Always, RuleMode.DesignTime}.Contains(Definition.EditParameters);
            _dashboard = Definition.GetDashboard(_application, RuleMode.DesignTime, dashboardCollectionDataSourceFillService,
                editParameters:dashboard1 => {
                    _dashboardHasParameters=dashboard1.Parameters.Any();
                });
            _dashboardDesigner.DashboardId = _dashboard.Title.Text;
            var dashboardStorage = new XpandDataSourceStorage(_application.Model);
            _dashboardDesigner.SetDataSourceStorage(dashboardStorage);
            _dashboardDesigner.CustomJSProperties += DashboardDesigner_CustomJSProperties;
            _dashboardDesigner.ClientSideEvents.DashboardChanged =
                @"function(dashboardControl, e) {
                    if (dashboardControl.cpDashboardHasParameters)
                        dashboardControl.ShowParametersDialog();                                                                                                                        
                  }";
            _dashboardDesigner.DashboardAdding += DashboardAdding;
            _dashboardDesigner.DashboardLoading += DashboardLoading;
            _dashboardDesigner.DashboardSaving += DashboardSaving;


            return _dashboardDesigner;
        }

        public new ASPxDashboard Control => _dashboardDesigner;

        protected virtual IObjectDataSourceCustomFillService AttachService(XpandDashboardDataProvider xpandDashboardDataProvider){
            return xpandDashboardDataProvider.AttachService(_dashboardDesigner.ServiceContainer, (IDashboardData) Definition);
        }

        protected virtual WorkingMode GetWorkingMode(){
            return WorkingMode.Designer;
        }

        protected void DashboardAdding(object sender, DashboardAddingWebEventArgs e) {
            var dashboardData = (IDashboardDefinition) View.CurrentObject;
            dashboardData.Xml=e.DashboardXml.ToString();
            e.DashboardId = e.DashboardName;
            e.Handled = true;
        }

        protected void DashboardSaving(object sender, DashboardSavingWebEventArgs e) {
            var dashboardData = (IDashboardDefinition) View.CurrentObject;
            dashboardData.Xml = e.DashboardXml.ToString();
            _objsectSpace.CommitChanges();
            e.Handled = true;
        }

        protected void DashboardLoading(object sender, DashboardLoadingWebEventArgs e) {
            var dashboardData = (IDashboardDefinition) View.CurrentObject;
            if(dashboardData != null) {
                using (var xmlStream = new MemoryStream(Encoding.UTF8.GetBytes(_dashboard.GetDashboardXml())))
                using (var xmlReader = new XmlTextReader(xmlStream)){
                    e.DashboardXml=XDocument.Load(xmlReader);
                }
            }
        }

        private void DashboardDesigner_CustomJSProperties(object sender, CustomJSPropertiesEventArgs e){
            e.Properties["cpDashboardHasParameters"] = _dashboardHasParameters;
        }

        protected IDashboardDefinition Definition => CurrentObject as IDashboardDefinition;

        protected override WebControl CreateEditModeControlCore() {
            return GetASPxDashboardDesigner();
        }

        protected override WebControl CreateViewModeControlCore() {
            return GetASPxDashboardDesigner();
        }

        protected override void ReadEditModeValueCore() {
        }

        public void Setup(IObjectSpace objectSpace, XafApplication application){
            _objsectSpace = objectSpace;
            _application = application;
        }
    }
}
