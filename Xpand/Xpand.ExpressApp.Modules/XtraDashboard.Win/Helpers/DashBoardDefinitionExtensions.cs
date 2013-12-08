using System;
using System.IO;
using System.Linq;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Model.Core;
using DevExpress.ExpressApp.Utils;
using DevExpress.Persistent.Base;
using Xpand.ExpressApp.Dashboard.BusinessObjects;
using Fasterflect;

namespace Xpand.ExpressApp.XtraDashboard.Win.Helpers {
    public static class DashBoardDefinitionExtensions {
        public static DevExpress.DashboardCommon.Dashboard CreateDashBoard(this IDashboardDefinition template, IObjectSpace objectSpace, bool filter) {
            var dashboard = new DevExpress.DashboardCommon.Dashboard();
            try {
                LoadFromXml(template.Xml, dashboard);
                foreach (var typeWrapper in template.DashboardTypes.Select(wrapper => new{wrapper.Type,Caption=GetCaption(wrapper)})) {
                    var wrapper = typeWrapper;
                    var dsource = dashboard.DataSources.FirstOrDefault(source => source.Name.Equals(wrapper.Caption));
                    if (dsource!=null) {
                        dsource.Data = objectSpace.GetObjects(wrapper.Type);
                    } else if (!dashboard.DataSources.Contains(ds => ds.Name.Equals(wrapper.Caption)))
                        dashboard.AddDataSource(typeWrapper.Caption, objectSpace.GetObjects(typeWrapper.Type));
                }
                if (filter)
                    Filter(template, dashboard);
            } catch (Exception e) {
                dashboard.Dispose();
                Tracing.Tracer.LogError(e);
            }
            return dashboard;
        }

        static string GetCaption(ITypeWrapper typeWrapper) {
            var modelApplicationBase = ((ModelApplicationBase) CaptionHelper.ApplicationModel);
            var currentAspect = modelApplicationBase.CurrentAspect;
            modelApplicationBase.SetCurrentAspect("");
            var caption = typeWrapper.Caption;
            modelApplicationBase.SetCurrentAspect(currentAspect);
            return caption;
        }

        static void Filter(IDashboardDefinition template, DevExpress.DashboardCommon.Dashboard dashboard) {
            var types = template.DashboardTypes.Select(GetCaption);
            for (int i = dashboard.DataSources.Count - 1; i >= 0; i--)
                if (!types.Contains(dashboard.DataSources[i].Name))
                    dashboard.DataSources.RemoveAt(i);
        }

        static void LoadFromXml(string xml, DevExpress.DashboardCommon.Dashboard dashboard) {
            if (xml != null) {
                using (var me = new MemoryStream()) {
                    var sw = new StreamWriter(me);
                    sw.Write(xml);
                    sw.Flush();
                    me.Seek(0, SeekOrigin.Begin);
                    dashboard.LoadFromXml(me);
                    sw.Close();
                    me.Close();
                }
            }
        }
        /// <summary>
        /// Returns a _private_ Property Value from a given Object. Uses Reflection.
        /// Throws a ArgumentOutOfRangeException if the Property is not found.
        /// </summary>
        /// <typeparam name="T">Type of the Property</typeparam>
        /// <param name="obj">Object from where the Property Value is returned</param>
        /// <param name="propName">Propertyname as string.</param>
        /// <returns>PropertyValue</returns>
        public static T GetPrivatePropertyValue<T>(this object obj, string propName) {
            if (obj == null)
                throw new ArgumentNullException("obj");
            return (T) obj.GetPropertyValue(propName);
        }

    }
}
