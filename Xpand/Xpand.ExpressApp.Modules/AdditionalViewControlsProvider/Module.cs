using System.ComponentModel;
using System.Drawing;
using System.Linq;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Model;
using DevExpress.Utils;
using Xpand.ExpressApp.AdditionalViewControlsProvider.Logic;
using Xpand.ExpressApp.AdditionalViewControlsProvider.Model;
using Xpand.ExpressApp.Logic;
using Xpand.Extensions.XAF.Xpo;
using Xpand.Persistent.Base.General;
using Xpand.Persistent.Base.Security;
using Xpand.XAF.Modules.ModelViewInheritance;

namespace Xpand.ExpressApp.AdditionalViewControlsProvider {
    [ToolboxBitmap(typeof(AdditionalViewControlsModule))]
    [ToolboxItem(true)]
    [ToolboxTabName(XpandAssemblyInfo.TabWinWebModules)]
    public sealed class AdditionalViewControlsModule : XpandModuleBase,ISecurityModuleUser{

        public AdditionalViewControlsModule() {
            LogicInstallerManager.RegisterInstaller(new AdditionalViewControlsLogicInstaller(this));
            RequiredModuleTypes.Add(typeof(ModelViewInheritanceModule));
        }

        public override void Setup(ApplicationModulesManager moduleManager){
            base.Setup(moduleManager);
            this.AddSecurityObjectsToAdditionalExportedTypes("Xpand.Persistent.BaseImpl.AdditionalViewControls");
        }

        #region IModelExtender Members

        public override void CustomizeTypesInfo(DevExpress.ExpressApp.DC.ITypesInfo typesInfo) {
            base.CustomizeTypesInfo(typesInfo);
            var typeInfos = typesInfo.PersistentTypes.Where(info => info.FindAttribute<PessimisticLockingMessageAttribute>() != null);
            foreach (var typeInfo in typeInfos) {
                var memberInfo = typeInfo.FindMember("LockedUserMessage");
                if (memberInfo == null) {
                    var xpClassInfo = typeInfo.QueryXPClassInfo();
                    var lockedUserMessageXpMemberInfo = new LockedUserMessageXpMemberInfo(xpClassInfo);
                    lockedUserMessageXpMemberInfo.AddAttribute(new BrowsableAttribute(false));
                    XafTypesInfo.Instance.RefreshInfo(typeInfo);
                }
            }
        }
        public override void ExtendModelInterfaces(ModelInterfaceExtenders extenders) {
            base.ExtendModelInterfaces(extenders);
            extenders.Add<IModelApplication,IModelApplicationAdditionalViewControls>();
        }
        #endregion
    }
}
