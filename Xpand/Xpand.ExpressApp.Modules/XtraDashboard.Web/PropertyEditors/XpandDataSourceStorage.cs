using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using DevExpress.DashboardCommon;
using DevExpress.DashboardWeb;
using DevExpress.ExpressApp.Model;

namespace Xpand.ExpressApp.XtraDashboard.Web.PropertyEditors{
    public class XpandDataSourceStorage : IDataSourceStorage {
        readonly Dictionary<string, XDocument> _dataSources = new Dictionary<string, XDocument>();
        public XpandDataSourceStorage(IModelApplication application) {
            Dictionary<Type, string> typesCaptions = application.BOModel.ToDictionary(modelClass=>modelClass.TypeInfo.Type,modelClass => modelClass.Caption);
            foreach(Type dashboardType in typesCaptions.Keys) {
                DashboardObjectDataSource dataSource = new DashboardObjectDataSource{DataSource = dashboardType};
                string name = typesCaptions[dashboardType];
                dataSource.Name = name;
                _dataSources.Add($"{dashboardType.FullName}", new XDocument(dataSource.SaveToXml()));
            }
        }
        public IEnumerable<string> GetDataSourcesID() {
            return _dataSources.Keys;
        }
        public XDocument GetDataSource(string dataSourceID) {
            return _dataSources[dataSourceID];
        }
    }
}