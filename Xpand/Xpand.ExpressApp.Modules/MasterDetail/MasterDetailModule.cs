using System.ComponentModel;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Model;
using DevExpress.Utils;
using Xpand.ExpressApp.Logic;
using Xpand.ExpressApp.MasterDetail.Model;
using Xpand.Persistent.Base.General;
using Xpand.Persistent.Base.Security;
using Xpand.XAF.Modules.ModelViewInheritance;

namespace Xpand.ExpressApp.MasterDetail {
    [ToolboxItem(true)]
    [ToolboxTabName(XpandAssemblyInfo.TabWinWebModules)]
    public class MasterDetailModule : XpandModuleBase,ISecurityModuleUser {

        public MasterDetailModule() {
            LogicInstallerManager.RegisterInstaller(new MasterDetailLogicInstaller(this));
            RequiredModuleTypes.Add(typeof(ModelViewInheritanceModule));
        }

        public override void Setup(ApplicationModulesManager moduleManager){
            base.Setup(moduleManager);
            this.AddSecurityObjectsToAdditionalExportedTypes("Xpand.Persistent.BaseImpl.MasterDetail");
        }

        public override void ExtendModelInterfaces(ModelInterfaceExtenders extenders) {
            base.ExtendModelInterfaces(extenders);
            extenders.Add<IModelApplication,IModelApplicationMasterDetail>();
        }
    }
}