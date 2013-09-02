using System;
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
            return sb.ToString();
        }

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
