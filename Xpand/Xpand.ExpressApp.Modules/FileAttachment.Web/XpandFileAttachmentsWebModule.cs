using System.ComponentModel;
using System.Drawing;
using DevExpress.ExpressApp;
using DevExpress.Utils;
using Xpand.Persistent.Base.General;
using Xpand.XAF.Modules.ModelMapper;
using Xpand.XAF.Modules.ModelMapper.Configuration;
using Xpand.XAF.Modules.ModelMapper.Services;

namespace Xpand.ExpressApp.FileAttachment.Web {
    [ToolboxTabName(XpandAssemblyInfo.TabAspNetModules), ToolboxBitmap(typeof(XpandFileAttachmentsWebModule)), ToolboxItem(true)]
    public sealed class XpandFileAttachmentsWebModule : XpandModuleBase {
        public XpandFileAttachmentsWebModule() {
            RequiredModuleTypes.Add(typeof (XpandFileAttachmentsModule));
            RequiredModuleTypes.Add(typeof(ModelMapperModule));
        }

        public override void Setup(ApplicationModulesManager moduleManager) {
            base.Setup(moduleManager);
            moduleManager.Extend(PredefinedMap.ASPxUploadControl);
        }
    }
}