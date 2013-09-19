using System.Collections.Generic;
using System.Web.UI;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Editors;
using Xpand.ExpressApp.Web.Layout;

namespace Xpand.ExpressApp.MapView.Web {
    public sealed partial class MapViewWebModule : ModuleBase {
        public MapViewWebModule() {
            InitializeComponent();
            XpandLayoutManager.RegisterListControlAdapter(typeof(MapControl), typeof(GoogleMapListControlAdapter));
        }

        public override void Setup(XafApplication application) {
            base.Setup(application);
            Application.CustomizeTemplate += Application_CustomizeTemplate;
        }

        void Application_CustomizeTemplate(object sender, CustomizeTemplateEventArgs e) {
            var page = e.Template as Page;
            if (page != null)
                page.ClientScript.RegisterClientScriptInclude("GoogleMaps", "https://maps.googleapis.com/maps/api/js?v=3.exp&sensor=false");
            //page.Header.Controls.Add(new LiteralControl("<script "));
        }
        protected override void RegisterEditorDescriptors(List<EditorDescriptor> editorDescriptors) {
            editorDescriptors.Add(new ListEditorDescriptor(new AliasRegistration("MapListEditor", typeof(object), false)));
            editorDescriptors.Add(new ListEditorDescriptor(new EditorTypeRegistration("MapListEditor", typeof(object), typeof(MapListEditor), false)));
        }
    }

    public class GoogleMapListControlAdapter : ListControlAdapterBase<MapControl> {

        public override string CreateSetBoundsScript(string widthFunc, string heightFunc) {
            return string.Empty;
        }
    }
}
