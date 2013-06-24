using System.ComponentModel;
using System.Drawing;
using DevExpress.ExpressApp.HtmlPropertyEditor.Web;
using DevExpress.Utils;

namespace Xpand.ExpressApp.HtmlPropertyEditor.Web {
    [ToolboxItem(true)]
    [ToolboxBitmap(typeof(HtmlPropertyEditorAspNetModule), "Resources.Toolbox_Module_HtmlPropertyEditor_Web.bmp")]
    [ToolboxTabName(XpandAssemblyInfo.TabAspNetModules)]
    public sealed class XpandHtmlPropertyEditorAspNetModule : XpandModuleBase {
        public XpandHtmlPropertyEditorAspNetModule() {
            RequiredModuleTypes.Add(typeof(HtmlPropertyEditorAspNetModule));
        }
    }
}