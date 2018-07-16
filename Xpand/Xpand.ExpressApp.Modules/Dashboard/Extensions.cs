using System;
using System.IO;
using DevExpress.ExpressApp;

namespace Xpand.ExpressApp.Dashboard {
    public static class Extensions {
        public static string GetDashboardXml(this DevExpress.DashboardCommon.Dashboard dashboard){
            string xml;
            using (var ms = new MemoryStream()) {
                dashboard.SaveToXml(ms);
                ms.Position = 0;
                using (var sr = new StreamReader(ms)) {
                    xml = sr.ReadToEnd();
                    sr.Close();
                }
                ms.Close();
            }
            return xml;

        }

    }

    public class ParameterLessProxyCollection  {

        public ParameterLessProxyCollection(string type){
            Type = XafTypesInfo.Instance.FindTypeInfo(type).Type;
        }

        public Type Type{ get; }
    }
}
