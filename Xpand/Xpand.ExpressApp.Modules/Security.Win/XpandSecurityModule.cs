using System.ComponentModel;
using System.Drawing;
using Xpand.ExpressApp.Security.Permissions;
using Xpand.ExpressApp.Security.Win.Permissions;

namespace Xpand.ExpressApp.Security.Win {
    [ToolboxBitmap(typeof(XpandSecurityWinModule))]
    [ToolboxItem(true)]
    public sealed class XpandSecurityWinModule : XpandModuleBase {
        public XpandSecurityWinModule() {
            RequiredModuleTypes.Add(typeof(XpandSecurityModule));
            PermissionProviderStorage.Instance.Add(new OverallCustomizationAllowedPermission());
        }
        public override void CustomizeTypesInfo(DevExpress.ExpressApp.DC.ITypesInfo typesInfo) {
            base.CustomizeTypesInfo(typesInfo);
            var typeInfo = typesInfo.FindTypeInfo(RoleType);
            typeInfo.CreateMember("ModifyLayout", typeof(bool));
        }
    }
}