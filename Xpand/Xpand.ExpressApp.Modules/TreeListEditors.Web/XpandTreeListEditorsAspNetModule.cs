using System;
using System.ComponentModel;
using System.Drawing;
using DevExpress.ExpressApp.TreeListEditors.Web;
using DevExpress.Utils;

namespace Xpand.ExpressApp.TreeListEditors.Web {
    [Description, ToolboxTabName("eXpressApp"), EditorBrowsable(EditorBrowsableState.Always)]
    [ToolboxBitmap(typeof(XpandTreeListEditorsAspNetModule))]
    [ToolboxItem(true)]
    public sealed class XpandTreeListEditorsAspNetModule : XpandModuleBase {
        public XpandTreeListEditorsAspNetModule() {
            RequiredModuleTypes.Add(typeof(TreeListEditorsAspNetModule));
            RequiredModuleTypes.Add(typeof(XpandTreeListEditorsModule));
        }
    }
}