using System.ComponentModel;
using System.Drawing;
using DevExpress.ExpressApp.TreeListEditors.Web;
using DevExpress.Utils;
using Xpand.Persistent.Base.General;

namespace Xpand.ExpressApp.TreeListEditors.Web {
    [Description, EditorBrowsable(EditorBrowsableState.Always)]
    [ToolboxBitmap(typeof(TreeListEditorsAspNetModule), "Resources.Toolbox_Module_TreeListEditors_Web.ico")]
    [ToolboxItem(true)]
    [ToolboxTabName(XpandAssemblyInfo.TabAspNetModules)]
    public sealed class XpandTreeListEditorsAspNetModule : XpandModuleBase {
        public XpandTreeListEditorsAspNetModule() {
            RequiredModuleTypes.Add(typeof(TreeListEditorsAspNetModule));
            RequiredModuleTypes.Add(typeof(XpandTreeListEditorsModule));
        }
    }
}