using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DevExpress.Web.ASPxClasses;

namespace Xpand.ExpressApp.MapView.Web
{
    public class MapControl : ASPxWebControl
    {
        protected override void RegisterIncludeScripts()
        {
            base.RegisterIncludeScripts();
            RegisterIncludeScript(typeof(MapControl), "MapControl.js");
            RegisterScriptBlock();
        }
        protected override void RegisterPostBackScripts()
        {
            base.RegisterPostBackScripts();
        }
    }
}
