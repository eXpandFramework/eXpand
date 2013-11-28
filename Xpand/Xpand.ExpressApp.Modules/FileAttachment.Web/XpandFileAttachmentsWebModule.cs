using System.ComponentModel;
using System.Drawing;
using DevExpress.Utils;
using Xpand.Persistent.Base.General;
using Xpand.Persistent.Base.General.Controllers;

namespace Xpand.ExpressApp.FileAttachment.Web {
    [ToolboxTabName(XpandAssemblyInfo.TabAspNetModules), ToolboxBitmap(typeof(XpandFileAttachmentsWebModule)), ToolboxItem(true)]
    public sealed class XpandFileAttachmentsWebModule : XpandModuleBase, IModuleSupportUploadControl {
        public XpandFileAttachmentsWebModule() {
            RequiredModuleTypes.Add(typeof (XpandFileAttachmentsModule));
        }
    }
}