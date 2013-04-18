using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Security;
using DevExpress.Utils;
using Xpand.ExpressApp.Security.Core;
using Xpand.Persistent.Base.PersistentMetaData;

namespace Xpand.ExpressApp.Security {
    [ToolboxBitmap(typeof(SecurityModule), "Resources.BO_Security.ico")]
    [ToolboxItem(true)]
    [ToolboxTabName(XpandAssemblyInfo.TabModules)]
    public sealed class XpandSecurityModule : XpandModuleBase {
        public XpandSecurityModule() {
            RequiredModuleTypes.Add(typeof(SecurityModule));
        }

        public override void CustomizeTypesInfo(ITypesInfo typesInfo) {
            base.CustomizeTypesInfo(typesInfo);
            if (Application != null) {
                var roleTypeProvider = Application.Security as IRoleTypeProvider;
                if (roleTypeProvider != null) {
                    foreach (var attribute in SecurityOperationsAttributes(typesInfo)) {
                        CreateMember(typesInfo, roleTypeProvider, attribute);
                    }
                }
            }
        }

        void CreateMember(ITypesInfo typesInfo, IRoleTypeProvider roleTypeProvider, SecurityOperationsAttribute attribute) {
            var roleTypeInfo = typesInfo.FindTypeInfo(roleTypeProvider.RoleType);
            if (roleTypeInfo.FindMember(attribute.OperationProviderProperty) == null) {
                roleTypeInfo.CreateMember(attribute.OperationProviderProperty, typeof(SecurityOperationsEnum));
            }
        }

        IEnumerable<SecurityOperationsAttribute> SecurityOperationsAttributes(ITypesInfo typesInfo) {
            var typeInfos = typesInfo.PersistentTypes.Where(info => info.FindAttribute<SecurityOperationsAttribute>() != null);
            return typeInfos.SelectMany(info => info.FindAttributes<SecurityOperationsAttribute>());
        }

        #region Overrides of XpandModuleBase
        protected override Type ApplicationType() {
            return typeof(ISettingsStorage);
        }
        #endregion
    }
}