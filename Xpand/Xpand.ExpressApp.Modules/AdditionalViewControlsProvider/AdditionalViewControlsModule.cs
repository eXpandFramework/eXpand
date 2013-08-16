using System.ComponentModel;
using System.Drawing;
using System.Linq;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Model;
using Xpand.ExpressApp.AdditionalViewControlsProvider.Logic;
using Xpand.ExpressApp.AdditionalViewControlsProvider.Model;
using Xpand.ExpressApp.Logic;
using Xpand.Persistent.Base.General;

namespace Xpand.ExpressApp.AdditionalViewControlsProvider {
    [ToolboxBitmap(typeof(AdditionalViewControlsModule))]
    [ToolboxItem(false)]
    public sealed class AdditionalViewControlsModule : XpandModuleBase{

        public AdditionalViewControlsModule() {
            LogicInstallerManager.RegisterInstaller(new AdditionalViewControlsLogicInstaller(this));
        }

        #region IModelExtender Members

        public override void CustomizeTypesInfo(DevExpress.ExpressApp.DC.ITypesInfo typesInfo) {
            base.CustomizeTypesInfo(typesInfo);
            var typeInfos = typesInfo.PersistentTypes.Where(info => info.FindAttribute<PessimisticLockingMessageAttribute>() != null);
            foreach (var typeInfo in typeInfos) {
                var memberInfo = typeInfo.FindMember("LockedUserMessage");
                if (memberInfo == null) {
                    var xpClassInfo = Dictiorary.GetClassInfo(typeInfo.Type);
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
