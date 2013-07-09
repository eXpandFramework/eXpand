using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using DevExpress.Data.Filtering;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Security;
using DevExpress.Utils;
using Xpand.ExpressApp.Security.Core;
using Xpand.ExpressApp.Security.Permissions;
using Xpand.Persistent.Base.PersistentMetaData;

namespace Xpand.ExpressApp.Security {
    [ToolboxBitmap(typeof(SecurityModule), "Resources.BO_Security.ico")]
    [ToolboxItem(true)]
    [ToolboxTabName(XpandAssemblyInfo.TabAspNetModules)]
    public sealed class XpandSecurityModule : XpandModuleBase {
        public XpandSecurityModule() {
            RequiredModuleTypes.Add(typeof(SecurityModule));
        }

        public override void Setup(ApplicationModulesManager moduleManager) {
            base.Setup(moduleManager);
            if (RuntimeMode) {
                Application.SetupComplete += ApplicationOnSetupComplete;
            }
        }

        void ApplicationOnSetupComplete(object sender, EventArgs eventArgs) {
            var securityStrategy = ((XafApplication)sender).Security as SecurityStrategy;
            if (securityStrategy != null) (securityStrategy).CustomizeRequestProcessors += OnCustomizeRequestProcessors;
        }

        void OnCustomizeRequestProcessors(object sender, CustomizeRequestProcessorsEventArgs customizeRequestProcessorsEventArgs) {
            var keyValuePairs = new[]{
                new KeyValuePair<Type, IPermissionRequestProcessor>(typeof (MyDetailsOperationRequest), new MyDetailsRequestProcessor(customizeRequestProcessorsEventArgs.Permissions))
            };
            foreach (var keyValuePair in keyValuePairs) {
                customizeRequestProcessorsEventArgs.Processors.Add(keyValuePair);    
            }
        }

        public override void CustomizeTypesInfo(ITypesInfo typesInfo) {
            base.CustomizeTypesInfo(typesInfo);
            if (Application != null) {
                var roleTypeProvider = Application.Security as IRoleTypeProvider;
                if (roleTypeProvider != null) {
                    foreach (var attribute in SecurityOperationsAttributes(typesInfo)) {
                        CreateMember(typesInfo, roleTypeProvider, attribute);
                    }
                    if (CriteriaOperator.GetCustomFunction(IsAllowedToRoleOperator.OperatorName) == null) {
                        CriteriaOperator.RegisterCustomFunction(new IsAllowedToRoleOperator());
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