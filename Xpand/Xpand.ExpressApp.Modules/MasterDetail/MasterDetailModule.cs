using System.ComponentModel;
using DevExpress.ExpressApp.Model;
using DevExpress.Utils;
using Xpand.ExpressApp.Logic;
using Xpand.ExpressApp.MasterDetail.Model;
using Xpand.Persistent.Base.General;

namespace Xpand.ExpressApp.MasterDetail {
    [ToolboxItem(true)]
    [ToolboxTabName(XpandAssemblyInfo.TabWinWebModules)]
    public class MasterDetailModule : XpandModuleBase {

        public MasterDetailModule() {
            LogicInstallerManager.RegisterInstaller(new MasterDetailLogicInstaller(this));
        }

        public override void ExtendModelInterfaces(ModelInterfaceExtenders extenders) {
            base.ExtendModelInterfaces(extenders);
            extenders.Add<IModelApplication,IModelApplicationMasterDetail>();
        }
    }
}