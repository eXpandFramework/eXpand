using System;
using System.ComponentModel;
using System.Drawing;
using DevExpress.ExpressApp.FileAttachments.Web;
using DevExpress.ExpressApp.TreeListEditors.Web;

namespace Xpand.ExpressApp.IO.Web {
    [ToolboxBitmap(typeof(IOAspNetModule))]
    [ToolboxItem(true)]
    public sealed class IOAspNetModule : XpandModuleBase {
        public IOAspNetModule() {
            RequiredModuleTypes.Add(typeof(ExpressApp.Web.SystemModule.XpandSystemAspNetModule));
            RequiredModuleTypes.Add(typeof(IOModule));
            RequiredModuleTypes.Add(typeof(TreeListEditorsAspNetModule));
            RequiredModuleTypes.Add(typeof(FileAttachmentsAspNetModule));
        }
    }
}