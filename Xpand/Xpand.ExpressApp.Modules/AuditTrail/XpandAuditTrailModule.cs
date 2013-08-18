using System.ComponentModel;
using System.Drawing;
using DevExpress.ExpressApp.AuditTrail;
using DevExpress.ExpressApp.Model;
using DevExpress.Utils;
using Xpand.ExpressApp.AuditTrail.Model;
using Xpand.ExpressApp.Logic;
using Xpand.Persistent.Base.General;

namespace Xpand.ExpressApp.AuditTrail {
    [ToolboxBitmap(typeof(AuditTrailModule))]
    [ToolboxItem(true)]
    [ToolboxTabName(XpandAssemblyInfo.TabWinWebModules)]
    public sealed class XpandAuditTrailModule :XpandModuleBase {
        public XpandAuditTrailModule() {
            RequiredModuleTypes.Add(typeof (AuditTrailModule));
            LogicInstallerManager.RegisterInstaller(new AuditTrailLogicInstaller(this));
        }
        public override void ExtendModelInterfaces(ModelInterfaceExtenders extenders) {
            base.ExtendModelInterfaces(extenders);
            extenders.Add<IModelApplication,IModelApplicationAudiTrail>();
        }

    }
}