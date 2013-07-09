using System;
using System.Collections.Generic;

using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Updating;

namespace Xpand.ExpressApp.MapView.Web
{
    public sealed partial class WebModule : ModuleBase
    {
        public WebModule()
        {
            InitializeComponent();
        }
        
        

        protected override void RegisterEditorDescriptors(List<EditorDescriptor> editorDescriptors)
        {
            editorDescriptors.Add(new ListEditorDescriptor(new AliasRegistration("MapListEditor", typeof(IGeoCoded), true)));
            editorDescriptors.Add(new ListEditorDescriptor(new EditorTypeRegistration("MapListEditor", typeof(IGeoCoded), typeof(MapListEditor), true)));
        }
    }
}
