using System;
using System.Linq;
using System.Web.UI.WebControls;
using DevExpress.DashboardCommon;
using DevExpress.DashboardWeb;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Model.Core;
using DevExpress.ExpressApp.Web.Editors;
using Xpand.ExpressApp.Dashboard;
using Xpand.ExpressApp.Dashboard.BusinessObjects;
using Xpand.ExpressApp.Dashboard.Filter;

namespace Xpand.ExpressApp.XtraDashboard.Web.PropertyEditors {
    [PropertyEditor(typeof(String), false)]
    public class DashboardDesignerEditor : WebPropertyEditor, IComplexViewItem {
        private IObjectSpace _objectSpace;
        private XafApplication _application;
        private ASPxDashboardDesigner _dashboardDesigner;

        static DashboardDesignerEditor() {
            DashboardService.SetDashboardStorage(new DatabaseDashboardStorage());
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

        private ASPxDashboardDesigner GetASPxDashboardDesigner() {
            _dashboardDesigner = new ASPxDashboardDesigner {
                DashboardId = GetDashboardId(),
                Width = Unit.Percentage(100),
                AllowOpenDashboard = false,
                AllowCreateNewDashboard = false,
                IncludeDashboardIdToUrl = false,
                IncludeDashboardStateToUrl = false
            };

            UnSubscribe();
            var dashboardDesignerStorage = DashboardService.DashboardStorage;
            DashboardService.DataApi.DataLoading += DashboardDesignerStorageOnDataLoading;
            var databaseDashboardStorage = ((DatabaseDashboardStorage)dashboardDesignerStorage);
            databaseDashboardStorage.RequestDashboardXml += OnRequestDashboardXml;
            databaseDashboardStorage.RequestObjectSpace += DatabaseDashboardStorageOnRequestObjectSpace;
            databaseDashboardStorage.RequestDashboardInfos += DatabaseDashboardStorageOnRequestDashboardInfos;
            return _dashboardDesigner;
        }

        private void DashboardDesignerStorageOnDataLoading(object sender, ServiceDataLoadingEventArgs e) {
            var modelApplication = (ModelApplicationBase)_application.Model;
            var typeWrapper = Definition.DashboardTypes.FirstOrDefault(t => t.GetDefaultCaption(modelApplication) == e.DataSourceName);
            if (typeWrapper != null) {
                var dsType = typeWrapper.Type;
                e.Data = _objectSpace.CreateDashboardDataSource(dsType);
            }
        }

        private string GetDashboardId() {
            return Definition.Name;
        }

        private void UnSubscribe() {
            DashboardService.DataApi.DataLoading -= DashboardDesignerStorageOnDataLoading;
            var databaseDashboardStorage = ((DatabaseDashboardStorage)DashboardService.DashboardStorage);
            databaseDashboardStorage.RequestDashboardXml -= OnRequestDashboardXml;
            databaseDashboardStorage.RequestObjectSpace -= DatabaseDashboardStorageOnRequestObjectSpace;
            databaseDashboardStorage.RequestDashboardInfos -= DatabaseDashboardStorageOnRequestDashboardInfos;
        }

        public override void BreakLinksToControl(bool unwireEventsOnly) {
            if (_dashboardDesigner != null && unwireEventsOnly) {
                UnSubscribe();
            }
            base.BreakLinksToControl(unwireEventsOnly);
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



        private void OnRequestDashboardXml(object sender, RequestDashboardXmlArgs requestDashboardXmlArgs) {
            string xml;
            using (var objectSpace = _application.CreateObjectSpace(ObjectTypeInfo.Type)) {
                xml = objectSpace.GetObject(Definition).GetXml(FilterEnabled.DesignTime, objectSpace.CreateDashboardDataSource, _application);
            }
            requestDashboardXmlArgs.Xml = xml;
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
            _objectSpace = objectSpace;
        }
    }
}
