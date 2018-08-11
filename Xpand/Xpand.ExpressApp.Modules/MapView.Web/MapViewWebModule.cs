using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Web.UI;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Editors;
using DevExpress.Utils;
using Xpand.ExpressApp.Web.Layout;
using Xpand.Persistent.Base.General;

namespace Xpand.ExpressApp.MapView.Web {
    [ToolboxItem(true)]
    [ToolboxTabName(XpandAssemblyInfo.TabAspNetModules)]
    public sealed class MapViewWebModule : XpandModuleBase {

        public event EventHandler<CustomGetApiKeyEventArgs> CustomGetGoogleApiKey;
        public MapViewWebModule() {
            RequiredModuleTypes.Add(typeof(MapViewModule));
            XpandLayoutManager.RegisterListControlAdapter(typeof(MapControl), typeof(GoogleMapListControlAdapter));
        }

        public override void Setup(XafApplication application) {
            base.Setup(application);
            Application.CustomizeTemplate += Application_CustomizeTemplate;
        }

        public string GoogleApiKey { get; set; }
        void Application_CustomizeTemplate(object sender, CustomizeTemplateEventArgs e) {
            var page = e.Template as Page;
            if (page != null) {
                CustomGetApiKeyEventArgs customGetApiKeyEventArgs = new CustomGetApiKeyEventArgs();
                CustomGetGoogleApiKey?.Invoke(this, customGetApiKeyEventArgs);
                string apiKey = customGetApiKeyEventArgs.ApiKey;
                if (string.IsNullOrWhiteSpace(apiKey))
                    apiKey = GoogleApiKey;

                UriBuilder uriBuilder = new UriBuilder("https://maps.googleapis.com/maps/api/js?v=3.13&sensor=false");
                if (!string.IsNullOrWhiteSpace(apiKey))
                    uriBuilder.Query += "&key=" + apiKey;

                page.ClientScript.RegisterClientScriptInclude("GoogleMaps", uriBuilder.Uri.ToString());
            }
        }
        protected override void RegisterEditorDescriptors(List<EditorDescriptor> editorDescriptors) {
            editorDescriptors.Add(new ListEditorDescriptor(new AliasRegistration("MapListEditor", typeof(object), false)));
            editorDescriptors.Add(new ListEditorDescriptor(new EditorTypeRegistration("MapListEditor", typeof(object), typeof(MapListEditor), false)));
        }
    }


    public class CustomGetApiKeyEventArgs : EventArgs {
        public string ApiKey { get; set; }
    }

    public class GoogleMapListControlAdapter : ListControlAdapterBase<MapControl> {

        public override string CreateSetBoundsScript(string widthFunc, string heightFunc) {
            return string.Empty;
        }
    }
}
