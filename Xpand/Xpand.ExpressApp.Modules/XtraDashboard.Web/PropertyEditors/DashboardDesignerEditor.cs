using System;
using System.Linq;
using System.Web.UI.WebControls;
using DevExpress.DashboardWeb;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Web.Editors;
using Xpand.ExpressApp.Dashboard;
using Xpand.ExpressApp.Dashboard.BusinessObjects;
using Xpand.ExpressApp.Dashboard.Filter;

namespace Xpand.ExpressApp.XtraDashboard.Web.PropertyEditors {
    [PropertyEditor(typeof(String), false)]
    public class DashboardDesignerEditor : WebPropertyEditor, IComplexViewItem {
        private XafApplication _application;
        private ASPxDashboard _dashboardDesigner;

        static DashboardDesignerEditor() {
            DashboardConfigurator.PassCredentials = true;
            
        }
        public DashboardDesignerEditor(Type objectType, IModelMemberViewItem model) : base(objectType, model) {
        }

        protected override object GetControlValueCore() {
            return null;
        }

        protected override void OnCurrentObjectChanged() {
            base.OnCurrentObjectChanged();
            if (_dashboardDesigner != null)
                _dashboardDesigner.DashboardId = GetDashboardId();
        }

        private ASPxDashboard GetASPxDashboardDesigner() {
            _dashboardDesigner = new ASPxDashboard {
                DashboardId = GetDashboardId(),
                Width = Unit.Percentage(100),
                AllowOpenDashboard = false,
                AllowCreateNewDashboard = false,
                IncludeDashboardIdToUrl = false,
                IncludeDashboardStateToUrl = false
            };
            var dashboardStorage = new DatabaseDashboardStorage();
            _dashboardDesigner.SetDashboardStorage(dashboardStorage);
            dashboardStorage.RequestDashboardXml += OnRequestDashboardXml;
            dashboardStorage.RequestObjectSpace += DatabaseDashboardStorageOnRequestObjectSpace;
            dashboardStorage.RequestDashboardInfos += DatabaseDashboardStorageOnRequestDashboardInfos;
            return _dashboardDesigner;
        }


        private string GetDashboardId() {
            return ObjectTypeInfo.KeyMember.GetValue(Definition).ToString().ToUpper();
        }

        private void DatabaseDashboardStorageOnRequestDashboardInfos(object sender, RequestDashboardInfosArgs e) {
            DashboardInfo[] dashboardInfos;
            using (var objectSpace = _application.CreateObjectSpace(ObjectTypeInfo.Type)) {
                dashboardInfos = objectSpace.GetObjectsQuery<DashboardDefinition>()
                        .Select(definition => new DashboardInfo { ID = definition.Oid.ToString(), Name = definition.Name })
                        .ToArray();
            }
            e.DashboardInfos = dashboardInfos;
        }

        private void DatabaseDashboardStorageOnRequestObjectSpace(object sender, RequestObjectSpaceArgs e) {
            e.ObjectSpace = _application.CreateObjectSpace(ObjectTypeInfo.Type);
        }



        private void OnRequestDashboardXml(object sender, RequestDashboardXmlArgs requestDashboardXmlArgs){
            requestDashboardXmlArgs.Xml = GetDashboardXml();
        }

        private string GetDashboardXml(){
            string xml;
            using (var objectSpace = _application.CreateObjectSpace(ObjectTypeInfo.Type)){
                xml = objectSpace.GetObject(Definition).GetXml(RuleMode.DesignTime, objectSpace.CreateDashboardDataSource,
                    _application);
            }
            return xml;
        }

        IDashboardDefinition Definition => CurrentObject as IDashboardDefinition;

        protected override WebControl CreateEditModeControlCore() {
            return GetASPxDashboardDesigner();
        }

        protected override WebControl CreateViewModeControlCore() {
            return GetASPxDashboardDesigner();
        }

        protected override void ReadEditModeValueCore() {
        }

        public void Setup(IObjectSpace objectSpace, XafApplication application) {
            _application = application;
        }
    }
}
