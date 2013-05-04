using System.ComponentModel;
using System.Drawing;
using DevExpress.ExpressApp.Security;
using DevExpress.Utils;
using Xpand.ExpressApp.Security.Core;
using Xpand.ExpressApp.Security.Permissions;
using Xpand.ExpressApp.Security.Win.Permissions;

namespace Xpand.ExpressApp.Security.Win {
    [ToolboxBitmap(typeof(SecurityModule), "Resources.BO_Security.ico")]
    [ToolboxItem(true)]
    [ToolboxTabName(XpandAssemblyInfo.TabWinModules)]
    public sealed class XpandSecurityWinModule : XpandModuleBase {
        public XpandSecurityWinModule() {
            RequiredModuleTypes.Add(typeof(XpandSecurityModule));
            PermissionProviderStorage.Instance.Add(new OverallCustomizationAllowedPermission());
        }
        public override void Setup(DevExpress.ExpressApp.ApplicationModulesManager moduleManager) {
            base.Setup(moduleManager);
            OverallCustomizationAllowedPermissionRequest.Register(Application);
        }
        public override void CustomizeTypesInfo(DevExpress.ExpressApp.DC.ITypesInfo typesInfo) {
            base.CustomizeTypesInfo(typesInfo);
            var type = Application == null ? typeof(XpandRole) : RoleType;
            var typeInfo = typesInfo.FindTypeInfo(type);
            if (typeInfo != null && typeInfo.FindMember("ModifyLayout") == null)
                typeInfo.CreateMember("ModifyLayout", typeof(bool));
        }
    }
}