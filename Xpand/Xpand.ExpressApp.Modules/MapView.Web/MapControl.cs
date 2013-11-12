using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Text;
using System.Web.UI.HtmlControls;
using DevExpress.Web.ASPxClasses;
using DevExpress.Web.ASPxClasses.Internal;
using Xpand.ExpressApp.Web.Layout;
using System.Linq;

namespace Xpand.ExpressApp.MapView.Web
{
    public class MapControl : ASPxWebControl
    {
        private const int infoWidthDefaultValue = 300;
        private HtmlGenericControl div;
        private int infoWindowWidth = infoWidthDefaultValue;
        public event EventHandler FocusedIndexChanged;
        internal event EventHandler<MapViewInfoEventArgs> MapViewInfoNeeded;

        private void OnMapViewInfoNeeded(MapViewInfoEventArgs e)
        {
            var handler = MapViewInfoNeeded;
            if (handler != null) handler(this, e);
        }


        internal bool PerformCallbackOnMarker { get; set; }
        protected override string GetStartupScript()
        {
            var sb = new StringBuilder();

            sb.AppendLine("var adjustSizeOverride = " + XpandLayoutManager.GetAdjustSizeScript());
            sb.AppendLine("adjustSizeOverride();");
            sb.AppendFormat("var div = document.getElementById('{0}');", div.ClientID);
            sb.AppendLine("window.ElementToResize = div;");
            sb.AppendLine("var initMap = function() { ");
            
            sb.AppendFormat(@"{0}
                var parentSplitter = XpandHelper.GetElementParentControl(div);
                if (parentSplitter && !parentSplitter.xpandInitialized) {{
                    window.setTimeout(initMap, 500);
                    return;
                }}
            

            ", XpandLayoutManager.GetXpandHelperScript(), div.ClientID);

            sb.AppendLine("var mapOptions = {");
            sb.AppendLine("zoom: 4,");
            sb.AppendLine("mapTypeId: google.maps.MapTypeId.ROADMAP }");
            sb.AppendFormat(" var map = new google.maps.Map(document.getElementById('{0}'), mapOptions);\r\n",
                            div.ClientID);

            sb.AppendLine("var geocoder = new google.maps.Geocoder();");
            sb.AppendLine(@"var addMarkerClickEvent = function(marker, objectId) {
                                google.maps.event.addListener(marker, 'click', function() {");
            sb.AppendLine(@"if (marker.infoWindow) marker.infoWindow.open(map, marker);");


            sb.AppendLine(@"var markerCallback = function (s,e) {");
            if (PerformCallbackOnMarker)
            {
                sb.Append(@"
                    var up = XpandHelper.GetFirstChildControl(parentSplitter.GetPane(1).GetElement().childNodes[0]);
                    if (up && up.GetMainElement()) {{ 
                        up.PerformCallback(objectId);}}");
            }
            sb.AppendLine("};");

            sb.AppendLine("var arg = 'd1:' + objectId;");
            sb.AppendLine(RenderUtils.GetCallbackEventReference(Page, this, "arg", "markerCallback", "'" + ClientID + "'",
                GetCallBackErrorHandlerName()) + ";");

            sb.AppendLine(" });};");
            
            sb.AppendLine("var bounds = new google.maps.LatLngBounds ();");
            sb.AppendLine("var createMarker = function(location, objectId, fitBounds, infoWindowContent, infoWindowMaxWidth) {");
            sb.AppendLine(@" 
                           
                              var marker = new google.maps.Marker({
                                  map: map,
                                  position: location
                              
                              });
                              if (infoWindowContent) {
                                marker.infoWindow = new google.maps.InfoWindow({
                                    content: infoWindowContent,
                                    maxWidth: infoWindowMaxWidth
                                });
                              }
        
                              addMarkerClickEvent(marker, objectId);  
                              bounds.extend(location);  
                              if (fitBounds) map.fitBounds(bounds);
                              
                         }");

            sb.AppendLine(@"
                        var geoCodeQueue = new Array();
                        var createMarkersWithGeoCode = function() {
                                if (geoCodeQueue.length == 0) {
                                    console.log('resizing...');
                                    google.maps.event.trigger(map, 'resize');
                                    window.AdjustSize();
                                    map.fitBounds(bounds);
                                    return;
                                }

                                var info = geoCodeQueue.pop();
                                console.log(info.address);
                                geocoder.geocode( { 'address': info.address}, function(results, status) {
                                    if (status == google.maps.GeocoderStatus.OK) {
                                        console.log('geocode: success');
                                        info.onSuccess(results);
                                    } 
                                    else if (status == google.maps.GeocoderStatus.OVER_QUERY_LIMIT) {
                                        geoCodeQueue.push(info);
                                        window.setTimeout(function () { createMarkersWithGeoCode(); }, 1000);
                                        return;
                                    }
                                    else {
                                        console.error('Geocode was not successful for the following reason: ' + status);
                                    }
                                    createMarkersWithGeoCode();   
                                 });
                                
                                
                            }");

            sb.AppendLine("var createMarkerWithGeocode = function(address, objectId, fitBounds, infoWindowContent, infoWindowMaxWidth) {");
            sb.AppendLine(@"geoCodeQueue.push( {'address': address, 'onSuccess': function(results) {
                              console.log('Success ' + address);
                              createMarker(results[0].geometry.location, objectId, fitBounds, infoWindowContent, infoWindowMaxWidth);  
                            }});}");


            bool useGeoCode = false;
            
            if (DataSource != null)
            {
                var list = DataSource as IList;
                if (list != null)
                {
                    sb.AppendLine("var marker, infoWindow;");
                    var mapViewInfoEventArgs = new MapViewInfoEventArgs();
                    OnMapViewInfoNeeded(mapViewInfoEventArgs);
                    int index = 0;
                    if (mapViewInfoEventArgs.MapViewInfos != null)
                    {
                        foreach (var mapViewInfo in mapViewInfoEventArgs.MapViewInfos)
                        {
                            string infoWindowText = "undefined";
                            if (!string.IsNullOrEmpty(mapViewInfo.InfoWindowText))
                            {
                                infoWindowText = GetInfoWindowText(mapViewInfo);
                            }

                            if (mapViewInfo.Longitude != null && mapViewInfo.Latitude != null)
                            {

                                sb.AppendFormat(CultureInfo.InvariantCulture,
                                           "createMarker(new google.maps.LatLng({0},{1}), '{2}', {3}, '{4}', {5});\r\n",
                                    mapViewInfo.Latitude, mapViewInfo.Longitude, index,
                                    (index == list.Count - 1).ToString(CultureInfo.InvariantCulture).ToLower(),
                                    infoWindowText, InfoWindowWidth);
                            }
                            else if (!string.IsNullOrWhiteSpace(mapViewInfo.Address))
                            {
                                useGeoCode = true;
                                sb.AppendFormat(CultureInfo.InvariantCulture,
                                                "createMarkerWithGeocode('{0}', '{1}', {2}, '{3}', {4});\r\n",
                                                mapViewInfo.Address, index,
                                                (index == list.Count - 1).ToString(CultureInfo.InvariantCulture).ToLower(),
                                                infoWindowText, InfoWindowWidth);
                            }
                            index++;
                        }

                    }
                }
            }


            if (useGeoCode)
            {

                sb.AppendLine("createMarkersWithGeoCode();");
            }
            else
            {
                sb.AppendLine("window.AdjustSize();");
                sb.AppendLine("google.maps.event.trigger(map, 'resize');");
            }
            
            sb.AppendLine("};");
            sb.AppendLine("window.setTimeout(initMap, 500);");
            return sb.ToString();
        }

        [DefaultValue(false)]
        public bool AllowHtmlInInfoText { get; set; }

        private string GetInfoWindowText(MapViewInfo mapViewInfo)
        {
            string html = mapViewInfo.InfoWindowText;
            if (string.IsNullOrEmpty(html)) return string.Empty;

            if (!AllowHtmlInInfoText)
                html = System.Web.HttpUtility.HtmlEncode(mapViewInfo.InfoWindowText);
            return html.Replace("'", "''");
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

            div = new HtmlGenericControl("div") { ID = "MapContent" };
            div.Style.Add("display", "block");
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
                var list = DataSource as IList;
                return list != null && FocusedIndex >= 0 && FocusedIndex < list.Count ? list[FocusedIndex] : null;
            }
        }

        [DefaultValue(infoWidthDefaultValue)]
        public int InfoWindowWidth
        {
            get { return infoWindowWidth; }
            set { infoWindowWidth = value; }
        }
    }

    public interface IMapViewInfo
    {
        string Address { get; set; }
        string InfoWindowText { get; set; }
    }

    public class MapViewInfo : IMapViewInfo
    {
        public string Address { get; set; }
        public string InfoWindowText { get; set; }
        public decimal? Longitude { get; set; }
        public decimal? Latitude { get; set; }

    }
    class MapViewInfoEventArgs : EventArgs
    {
        public IEnumerable<MapViewInfo> MapViewInfos { get; set; }
    }
}
