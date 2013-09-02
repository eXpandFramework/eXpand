using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using DevExpress.Web.ASPxClasses;

namespace Xpand.ExpressApp.MapView.Web
{
    public class MapControl : ASPxWebControl
    {
        private HtmlGenericControl div;
        protected override string GetStartupScript()
        {
            StringBuilder sb =  new StringBuilder();
            
            sb.AppendLine("var mapOptions = {");
            sb.AppendLine("zoom: 4,");
            sb.AppendLine("center: new google.maps.LatLng(45.363882,13.044922),");
            sb.AppendLine("mapTypeId: google.maps.MapTypeId.ROADMAP }");

            sb.AppendFormat(" var map = new google.maps.Map(document.getElementById('{0}'), mapOptions);\r\n",
                            div.ClientID);

            sb.AppendLine("geocoder = new google.maps.Geocoder();");
            sb.AppendLine("var createMarkerWithGeocode = function(address) {");
            sb.AppendLine(@" geocoder.geocode( { 'address': address}, function(results, status) {
                            if (status == google.maps.GeocoderStatus.OK) {
                              var marker = new google.maps.Marker({
                                  map: map,
                                  position: results[0].geometry.location
                              });
                            } else {
                              alert('Geocode was not successful for the following reason: ' + status);
                            }
                            });}");


            
            if (DataSource != null)
            {
                IEnumerable enumerable = DataSource as IEnumerable;
                if (enumerable != null)
                {
                    foreach (var item in enumerable)
                    {
                        IMapAddress address = item as IMapAddress;
                        if (address != null && !string.IsNullOrEmpty(address.Address))
                        {
                            sb.AppendFormat("createMarkerWithGeocode('{0}');\r\n", address.Address);
                        }
                    }
                }
            }
            return sb.ToString();
        }


        public object DataSource { get; set; }

        protected override bool HasFunctionalityScripts()
        {
            return true;
        }

        protected override void CreateChildControls()
        {
            div = new HtmlGenericControl("div");
            div.ID = "MapContent";
            div.Style.Add("width","100%");
            div.Style.Add("height", "100%");
            Controls.Add(div);
        }
    }
}
