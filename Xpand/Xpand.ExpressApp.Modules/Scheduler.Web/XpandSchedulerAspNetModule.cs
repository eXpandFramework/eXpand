using System.ComponentModel;
using System.Drawing;
using DevExpress.Utils;
using Xpand.Persistent.Base.General;
using Xpand.XAF.Modules.ModelMapper;

namespace Xpand.ExpressApp.Scheduler.Web {
    [ToolboxBitmap(typeof(XpandSchedulerAspNetModule))]
    [ToolboxItem(true)]
    [ToolboxTabName(XpandAssemblyInfo.TabAspNetModules)]
    public sealed class XpandSchedulerAspNetModule : XpandModuleBase {
        public XpandSchedulerAspNetModule() {
            RequiredModuleTypes.Add(typeof (XpandSchedulerModule));
            RequiredModuleTypes.Add(typeof(DevExpress.ExpressApp.Scheduler.Web.SchedulerAspNetModule));
            RequiredModuleTypes.Add(typeof(ModelMapperModule));
        }
    }
}