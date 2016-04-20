using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using DevExpress.DashboardWeb;
using DevExpress.ExpressApp;
using Xpand.ExpressApp.Dashboard.BusinessObjects;

namespace Xpand.ExpressApp.XtraDashboard.Web.PropertyEditors{
    public class RequestObjectSpaceArgs : EventArgs {
        public IObjectSpace ObjectSpace { get; set; }
    }

    public class RequestDashboardInfosArgs : EventArgs {
        public IEnumerable<DashboardInfo> DashboardInfos { get; set; }
    }

    public class RequestDashboardXmlArgs : EventArgs {
        public string Xml { get; set; }
    }
    public class DatabaseDashboardStorage : IDashboardStorage {
        public event EventHandler<RequestObjectSpaceArgs> RequestObjectSpace;
        public event EventHandler<RequestDashboardXmlArgs> RequestDashboardXml;
        public event EventHandler<RequestDashboardInfosArgs> RequestDashboardInfos;

        protected virtual void OnRequestDashboarXml(RequestDashboardXmlArgs e) {
            var handler = RequestDashboardXml;
            if (handler != null) handler(this, e);
        }

        protected virtual void OnRequestDashboardInfos(RequestDashboardInfosArgs e) {
            EventHandler<RequestDashboardInfosArgs> handler = RequestDashboardInfos;
            if (handler != null) handler(this, e);
        }

        protected virtual void OnRequestObjectSpace(RequestObjectSpaceArgs e) {
            var handler = RequestObjectSpace;
            if (handler != null) handler(this, e);
        }

        public IEnumerable<DashboardInfo> GetAllDashboardsInfo() {
            var args = new RequestDashboardInfosArgs();
            OnRequestDashboardInfos(args);
            return args.DashboardInfos;
        }

        public XDocument LoadDashboard(string id) {
            var args = new RequestDashboardXmlArgs();
            OnRequestDashboarXml(args);
            return XDocument.Parse(args.Xml);
        }

        public void SaveDashboard(string id, XDocument dashboard) {
            var args = new RequestObjectSpaceArgs();
            OnRequestObjectSpace(args);
            using (var objectSpace = args.ObjectSpace) {
                var dashboardDefintion = objectSpace.GetObjectsQuery<DashboardDefinition>().First(definition => definition.Name == id);
                dashboardDefintion.Xml = dashboard.ToString();
                objectSpace.CommitChanges();
            }
        }
    }
}