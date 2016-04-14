using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI.WebControls;
using System.Xml.Linq;
using DevExpress.DashboardCommon.Native.DashboardRestfulService;
using DevExpress.DashboardWeb.Designer;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Model.Core;
using DevExpress.ExpressApp.Web.Editors;
using Xpand.ExpressApp.Dashboard;
using Xpand.ExpressApp.Dashboard.BusinessObjects;
using Xpand.ExpressApp.Dashboard.Filter;

namespace Xpand.ExpressApp.XtraDashboard.Web.PropertyEditors {
    public class DatabaseDashboardStorage : IDashboardStorage{
        public event EventHandler<RequestObjectSpaceArgs> RequestObjectSpace;
        public event EventHandler<RequestDashboardXmlArgs> RequestDashboardXml;
        public event EventHandler<RequestDashboardIdsArgs> RequestDashboardIds;
        public string CreateNewDashboard() {
            throw new NotImplementedException();
        }

        public XDocument GetDashboard(string id) {
            var args = new RequestDashboardXmlArgs();
            OnRequestDashboarXml(args);
            return XDocument.Parse(args.Xml);
        }

        public IEnumerable<string> GetDashboardIDs(){
            var args = new RequestDashboardIdsArgs();
            OnRequestDashboardIds(args);
            return args.Ids;
        }

        public void UpdateDashboard(string id, XDocument document) {
            var args = new RequestObjectSpaceArgs();
            OnRequestObjectSpace(args);
            using (var objectSpace = args.ObjectSpace){
                var dashboard = objectSpace.GetObjectsQuery<DashboardDefinition>().First(definition => definition.Name == id);
                dashboard.Xml = document.ToString();
                objectSpace.CommitChanges();
            }
        }

        protected virtual void OnRequestDashboarXml(RequestDashboardXmlArgs e){
            var handler = RequestDashboardXml;
            if (handler != null) handler(this, e);
        }

        protected virtual void OnRequestDashboardIds(RequestDashboardIdsArgs e){
            EventHandler<RequestDashboardIdsArgs> handler = RequestDashboardIds;
            if (handler != null) handler(this, e);
        }

        protected virtual void OnRequestObjectSpace(RequestObjectSpaceArgs e){
            var handler = RequestObjectSpace;
            if (handler != null) handler(this, e);
        }
    }

    public class RequestObjectSpaceArgs : EventArgs{
        public IObjectSpace ObjectSpace { get; set; }
    }

    public class RequestDashboardIdsArgs : EventArgs{
        public string[] Ids { get; set; }
    }

    public class RequestDashboardXmlArgs : EventArgs{
        public string Xml { get; set; }
    }

    [PropertyEditor(typeof(String), false)]
    public class DashboardDesignerEditor:WebPropertyEditor,IComplexViewItem {
        private IObjectSpace _objectSpace;
        private XafApplication _application;
        private ASPxDashboardDesigner _dashboardDesigner;

        static DashboardDesignerEditor(){
            ASPxDashboardDesigner.Storage.SetDashboardStorage(new DatabaseDashboardStorage());
        }
        public DashboardDesignerEditor(Type objectType, IModelMemberViewItem model) : base(objectType, model){
        }

        protected override object GetControlValueCore(){
            return null;
        }

        protected override void OnCurrentObjectChanged(){
            base.OnCurrentObjectChanged();
            if (_dashboardDesigner != null)
                _dashboardDesigner.DashboardId = GetDashboardId();
        }

        private ASPxDashboardDesigner GetASPxDashboardDesigner(){
            _dashboardDesigner = new ASPxDashboardDesigner{
                DashboardId = GetDashboardId(),
                Width = Unit.Percentage(100),
                AllowOpenDashboard = false,
                AllowCreateNewDashboard = false,
                IncludeDashboardIdToUrl = false,
                IncludeDashboardStateToUrl = false
            };

            UnSubscribe();
            var dashboardDesignerStorage = ASPxDashboardDesigner.Storage;
            dashboardDesignerStorage.DataLoading += DashboardDesignerStorageOnDataLoading;
            var databaseDashboardStorage = ((DatabaseDashboardStorage) dashboardDesignerStorage.DashboardStorage);
            databaseDashboardStorage.RequestDashboardXml+=OnRequestDashboardXml;
            databaseDashboardStorage.RequestObjectSpace+=DatabaseDashboardStorageOnRequestObjectSpace;
            databaseDashboardStorage.RequestDashboardIds+=DatabaseDashboardStorageOnRequestDashboardIds;
            return _dashboardDesigner;
        }

        private string GetDashboardId(){
            return Definition.Name;
        }

        private void UnSubscribe(){
            ASPxDashboardDesigner.Storage.DataLoading -= DashboardDesignerStorageOnDataLoading;
            var databaseDashboardStorage = ((DatabaseDashboardStorage) ASPxDashboardDesigner.Storage.DashboardStorage);
            databaseDashboardStorage.RequestDashboardXml -= OnRequestDashboardXml;
            databaseDashboardStorage.RequestObjectSpace -= DatabaseDashboardStorageOnRequestObjectSpace;
            databaseDashboardStorage.RequestDashboardIds -= DatabaseDashboardStorageOnRequestDashboardIds;
        }

        public override void BreakLinksToControl(bool unwireEventsOnly){
            if (_dashboardDesigner!=null&&unwireEventsOnly){
                UnSubscribe();
            }
            base.BreakLinksToControl(unwireEventsOnly);
        }

        private void DatabaseDashboardStorageOnRequestDashboardIds(object sender, RequestDashboardIdsArgs e){
            string[] strings;
            using (var objectSpace = _application.CreateObjectSpace()){
                strings = objectSpace.GetObjectsQuery<DashboardDefinition>().Select(definition => definition.Name).ToArray();
            }
            e.Ids = strings;
        }

        private void DatabaseDashboardStorageOnRequestObjectSpace(object sender, RequestObjectSpaceArgs e){
            e.ObjectSpace = _application.CreateObjectSpace();
        }

        private void DashboardDesignerStorageOnDataLoading(object sender, ConfigureServiceDataLoadingEventArgs e){
            var modelApplication = (ModelApplicationBase)_application.Model;
            var typeWrapper = Definition.DashboardTypes.FirstOrDefault(t => t.GetDefaultCaption(modelApplication) == e.DataSourceName);
            if (typeWrapper != null) {
                var dsType = typeWrapper.Type;
                e.Data = _objectSpace.CreateDashboardDataSource(dsType);
            }
        }


        private void OnRequestDashboardXml(object sender, RequestDashboardXmlArgs requestDashboardXmlArgs){
            string xml;
            using (var objectSpace = _application.CreateObjectSpace()){
                xml = objectSpace.GetObject(Definition).GetXml(FilterEnabled.DesignTime, objectSpace.CreateDashboardDataSource, _application);
            }
            requestDashboardXmlArgs.Xml= xml;
        }

        IDashboardDefinition Definition{
            get { return CurrentObject as IDashboardDefinition; }
        }

        protected override WebControl CreateEditModeControlCore(){
            return GetASPxDashboardDesigner();
        }

        protected override WebControl CreateViewModeControlCore(){
            return GetASPxDashboardDesigner();
        }

        protected override void ReadEditModeValueCore(){
        }

        public void Setup(IObjectSpace objectSpace, XafApplication application){
            _application = application;
            _objectSpace = objectSpace;
        }
    }
}
