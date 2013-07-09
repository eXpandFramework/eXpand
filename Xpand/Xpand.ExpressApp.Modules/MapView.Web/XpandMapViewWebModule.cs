using System;
using System.Collections.Generic;
using Artem.Google.UI;
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
            XpandLayoutManager.RegisterListControlAdapter(typeof(GoogleMap), typeof(GoogleMapListControlAdapter));
        }
        

        protected override void RegisterEditorDescriptors(List<EditorDescriptor> editorDescriptors)
        {
            editorDescriptors.Add(new ListEditorDescriptor(new AliasRegistration("MapListEditor", typeof(IGeoCoded), true)));
            editorDescriptors.Add(new ListEditorDescriptor(new EditorTypeRegistration("MapListEditor", typeof(IGeoCoded), typeof(MapListEditor), true)));
        }
    }

    public class GoogleMapListControlAdapter : ListControlAdapterBase<GoogleMap>
    {

        public override string CreateSetBoundsScript(string widthFunc, string heightFunc)
        {
            return string.Empty;
        }
    }
}
