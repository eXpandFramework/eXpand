using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using DevExpress.Web.ASPxClasses;
using DevExpress.Web.ASPxClasses.Internal;
using Xpand.ExpressApp.Web.Layout;

namespace Xpand.ExpressApp.MapView.Web
{
    public class MapControl : ASPxWebControl
    {
        private HtmlGenericControl div;

        public event EventHandler FocusedIndexChanged;

        protected override string GetStartupScript()
        {
            StringBuilder sb = new StringBuilder();

            sb.AppendLine("var mapOptions = {");
            sb.AppendLine("zoom: 4,");
            sb.AppendLine("center: new google.maps.LatLng(45.363882,13.044922),");
            sb.AppendLine("mapTypeId: google.maps.MapTypeId.ROADMAP }");

            sb.AppendFormat(" var map = new google.maps.Map(document.getElementById('{0}'), mapOptions);\r\n",
                            div.ClientID);

            sb.AppendLine("geocoder = new google.maps.Geocoder();");
            sb.AppendLine(@"var addMarkerClickEvent = function(marker, objectId) {
                                google.maps.event.addListener(marker, 'click', function() {");
            sb.AppendLine(@"var markerCallback = function (s,e) {");
            sb.AppendFormat(@"{0}
                    var parentSplitter = XpandHelper.GetElementParentControl(document.getElementById('{1}'));
                    var up = XpandHelper.GetFirstChildControl(parentSplitter.GetPane(1).GetElement().childNodes[0]);
                    if (up && up.GetMainElement()) {{ 
                        up.PerformCallback(objectId);}}", XpandLayoutManager.GetXpandHelperScript(), div.ClientID);

            sb.AppendLine("};");

            sb.AppendLine("var arg = 'd1:' + objectId;");
            sb.AppendLine(RenderUtils.GetCallbackEventReference(Page, this, "arg", "markerCallback", "'" + ClientID + "'",
                GetCallBackErrorHandlerName()) + ";");

            sb.AppendLine(" });};");
            sb.AppendLine("var createMarkerWithGeocode = function(address, objectId) {");
            sb.AppendLine(@" geocoder.geocode( { 'address': address}, function(results, status) {
                            if (status == google.maps.GeocoderStatus.OK) {
                              var marker = new google.maps.Marker({
                                  map: map,
                                  position: results[0].geometry.location
                              
                              });
                              addMarkerClickEvent(marker, objectId);  
                            } else {
                              alert('Geocode was not successful for the following reason: ' + status);
                            }
                            });}");



            if (DataSource != null)
            {
                IList list = DataSource as IList;
                if (list != null)
                {
                    for (int i = 0; i < list.Count; i++)
                    {
                        IMapAddress address = list[i] as IMapAddress;
                        if (address != null && !string.IsNullOrEmpty(address.Address))
                        {
                            sb.AppendFormat(CultureInfo.InvariantCulture,
                                "createMarkerWithGeocode('{0}', '{1}');\r\n", address.Address, i);
                        }
                    }
                }
            }
            return sb.ToString();
        }


        protected override void OnCustomDataCallback(CustomDataCallbackEventArgs e)
        {
            base.OnCustomDataCallback(e);
            int index;
            if (int.TryParse(e.Parameter, NumberStyles.Integer, CultureInfo.InvariantCulture, out index))
                FocusedIndex = index;
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
            div.Style.Add("width", "100%");
            div.Style.Add("height", "100%");
            Controls.Add(div);
        }


        private int focusedIndex = -1;

        public int FocusedIndex
        {
            get { return focusedIndex; }
            set
            {
                if (focusedIndex != value)
                {
                    focusedIndex = value;
                    if (FocusedIndexChanged != null)
                        FocusedIndexChanged(this, EventArgs.Empty);
                }
            }
        }

        public object FocusedObject
        {
            get
            {
                IList list = DataSource as IList;
                if (list != null && FocusedIndex >= 0 && FocusedIndex < list.Count)
                    return list[FocusedIndex];
                else
                    return null;
            }
        }
    }
}
