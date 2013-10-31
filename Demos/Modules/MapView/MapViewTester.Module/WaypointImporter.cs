using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Utils;
using MapViewTester.Module.BusinessObjects;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace MapViewTester.Module
{
    /// <summary>
    /// Importer class for importing test data.
    /// </summary>
    public class WaypointImporter
    {
        private readonly IObjectSpace objectSpace;
        public WaypointImporter(IObjectSpace objectSpace)
        {
            Guard.ArgumentNotNull(objectSpace, "objectSpace");
            this.objectSpace = objectSpace;
        }


        private static string GetSubElementValue(XElement element, string name)
        {
            var subElement = element.Elements().Where(e => e.Name.LocalName == name).FirstOrDefault();
            return subElement != null ? subElement.Value : null;
        }
        private void ImportXml(XDocument xmlDocument)
        {
            foreach (var wp in xmlDocument.Root.Descendants().Where(d => d.Name.LocalName == "wpt"))
            {
                Waypoint waypoint = objectSpace.CreateObject<Waypoint>();
                waypoint.Name = GetSubElementValue(wp, "name");
                waypoint.Comment = GetSubElementValue(wp, "cmt");
                waypoint.Longtitude = Convert.ToDecimal(wp.Attribute("lon").Value, CultureInfo.InvariantCulture);
                waypoint.Latitude = Convert.ToDecimal(wp.Attribute("lat").Value, CultureInfo.InvariantCulture);
            }
            objectSpace.CommitChanges();
        }

        public void ImportFile(string fileName)
        {
            ImportXml(XDocument.Load(fileName));
        }
    }
}
