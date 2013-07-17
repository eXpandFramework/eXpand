using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Web.UI;
using Artem.Google.UI;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Web;
using Xpand.ExpressApp.Web;

namespace Xpand.ExpressApp.MapView.Web
{
    [ListEditor(typeof(IGeoCoded), true)]
    public class MapListEditor : ListEditor
    {
        public MapListEditor(IModelListView model)
            : base(model)
        {

        }

        protected override void AssignDataSourceToControl(object dataSource)
        {
        }

        public override DevExpress.ExpressApp.Templates.IContextMenuTemplate ContextMenuTemplate
        {
            get { return null; }
        }

        protected override object CreateControlsCore()
        {
            var control = new Artem.Google.UI.GoogleMap();
            control.ID = "googleMap";
            control.MapType = MapType.Hybrid;
            control.Zoom = 8;
            control.CssClass = "map";
            control.Address = "Dortmund";

            WebWindow.CurrentRequestWindow.PagePreRender += (s, e) =>
                {
                    var scriptManager = XpandWebWindow.FindControlByType<ScriptManager>(WebWindow.CurrentRequestPage);
                    if (scriptManager != null)
                    {
                        var scriptManagerType = scriptManager.GetType();

                        var scriptControlManagerProperty = scriptManagerType.GetProperty("ScriptControlManager",
                                                                  BindingFlags.Instance | BindingFlags.NonPublic);

                        if (scriptControlManagerProperty != null)
                        {

                            var scriptControlManager = scriptControlManagerProperty.GetValue(scriptManager, null);
                            if (scriptControlManager != null)
                            {
                                var scriptControlManagerType = scriptControlManager.GetType();

                                var pagePrerenderRaisedField = scriptControlManagerType.GetField("_pagePreRenderRaised",
                                                                             BindingFlags.Instance |
                                                                             BindingFlags.NonPublic);
                                if (pagePrerenderRaisedField != null)
                                    pagePrerenderRaisedField.SetValue(scriptControlManager, true);

                                scriptManager.RegisterScriptControl(control);
                            }
                        }
                    }
                };

            return control;
        }



        public override System.Collections.IList GetSelectedObjects()
        {
            return new object[0];
        }

        public override void Refresh()
        {
           
        }

        public override DevExpress.ExpressApp.SelectionType SelectionType
        {
            get { return SelectionType.FocusedObject; }
        }
    }
}
