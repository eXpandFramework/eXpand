using System;
using System.Collections.Generic;
using System.Web.UI;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Updating;
using Xpand.ExpressApp.Web.Layout;

namespace Xpand.ExpressApp.MapView.Web
{
    public sealed partial class XpandMapViewWebModule : ModuleBase
    {
        public XpandMapViewWebModule()
        {
            InitializeComponent();
            XpandLayoutManager.RegisterListControlAdapter(typeof(MapControl), typeof(GoogleMapListControlAdapter));
        }


        public override void Setup(ApplicationModulesManager moduleManager)
        {
            base.Setup(moduleManager);
            Application.CustomizeTemplate += Application_CustomizeTemplate;
        }

        void Application_CustomizeTemplate(object sender, CustomizeTemplateEventArgs e)
        {
            Page page = e.Template as Page;
            if (page != null)
                page.ClientScript.RegisterClientScriptInclude("GoogleMaps", "https://maps.googleapis.com/maps/api/js?v=3.exp&sensor=false");
        }
        protected override void RegisterEditorDescriptors(List<EditorDescriptor> editorDescriptors)
        {
            editorDescriptors.Add(new ListEditorDescriptor(new AliasRegistration("MapListEditor", typeof(IMapAddress), true)));
            editorDescriptors.Add(new ListEditorDescriptor(new EditorTypeRegistration("MapListEditor", typeof(IMapAddress), typeof(MapListEditor), true)));
        }
    }

    public class GoogleMapListControlAdapter : ListControlAdapterBase<MapControl>
    {

        public override string CreateSetBoundsScript(string widthFunc, string heightFunc)
        {
            return string.Empty;
        }
    }
}
