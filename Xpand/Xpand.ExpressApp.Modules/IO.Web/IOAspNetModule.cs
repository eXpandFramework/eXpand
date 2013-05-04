using System.ComponentModel;
using System.Drawing;
using DevExpress.ExpressApp.FileAttachments.Web;
using DevExpress.ExpressApp.TreeListEditors.Web;
using DevExpress.Utils;

namespace Xpand.ExpressApp.IO.Web {
    [ToolboxBitmap(typeof(IOAspNetModule))]
    [ToolboxItem(true)]
    [ToolboxTabName(XpandAssemblyInfo.TabAspNetModules)]
    public sealed class IOAspNetModule : XpandModuleBase {
        public IOAspNetModule() {
            RequiredModuleTypes.Add(typeof(IOModule));
            RequiredModuleTypes.Add(typeof(TreeListEditorsAspNetModule));
            RequiredModuleTypes.Add(typeof(FileAttachmentsAspNetModule));
        }
    }
}