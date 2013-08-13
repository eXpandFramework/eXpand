using System.ComponentModel;
using DevExpress.ExpressApp.Model;
using Xpand.ExpressApp.Logic;
using Xpand.ExpressApp.MasterDetail.Model;
using Xpand.Persistent.Base.General;

namespace Xpand.ExpressApp.MasterDetail {
    [ToolboxItem(false)]
    public class MasterDetailModule : XpandModuleBase {

        public MasterDetailModule() {
            LogicInstallerManager.Instance.RegisterInstaller(new MasterDetailLogicInstaller(this));
        }

        public override void ExtendModelInterfaces(ModelInterfaceExtenders extenders) {
            base.ExtendModelInterfaces(extenders);
            extenders.Add<IModelApplication,IModelApplicationMasterDetail>();
        }
    }
}