using System;
using System.Collections.Generic;
using DevExpress.ExpressApp;
using System.ComponentModel;
using System.Drawing;

namespace DevExpress.ExpressApp.ConditionalEditorState.Web {
    [Browsable(true)]
    [EditorBrowsable(EditorBrowsableState.Always)]
    [ToolboxItem(true)]
    [DevExpress.Utils.ToolboxTabName(XafAssemblyInfo.DXTabXafModules)]
    [ToolboxBitmap(typeof(ConditionalEditorStateAspNetModule), "Resources.ConditionalEditorStateAspNetModule.ico")]
    [Description("Provides the capability to customize the view's editors against against business rules in ASP.NET XAF applications.")]
    public sealed partial class ConditionalEditorStateAspNetModule : ModuleBase {
        public ConditionalEditorStateAspNetModule() {
            InitializeComponent();
        }
    }
}
