using System;
using System.Collections.Generic;
using System.Linq;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Security;
using DevExpress.ExpressApp.Win.SystemModule;
using DevExpress.Xpo.Helpers;
using Xpand.ExpressApp.Security.Core;
using Xpand.ExpressApp.Security.Permissions;
using Xpand.Persistent.Base.Security;

namespace Xpand.ExpressApp.Security.Win.Permissions {
    public class OverallCustomizationAllowedPermission : IOperationPermission, IPermissionInfo {
        public string Operation => "OverallCustomizationAllowed";

        #region Implementation of IPermissionInfo
        public IEnumerable<IOperationPermission> GetPermissions(ISecurityRole securityRole) {
            return true.Equals(((IXPClassInfoProvider) securityRole).ClassInfo.GetMember("ModifyLayout").GetValue(securityRole))
                       ? new[] { new OverallCustomizationAllowedPermission() }
                       : Enumerable.Empty<IOperationPermission>();
        }
        #endregion
    }
    public class OverallCustomizationAllowedPermissionRequestProcessor : PermissionRequestProcessorBase<OverallCustomizationAllowedPermissionRequest> {
        private readonly IPermissionDictionary _permissions;
        public OverallCustomizationAllowedPermissionRequestProcessor(IPermissionDictionary permissions) {
            _permissions = permissions;
        }
        #region Overrides of PermissionRequestProcessorBase<OverallCustomizationAllowedPermissionRequest>
        public override bool IsGranted(OverallCustomizationAllowedPermissionRequest permissionRequest) {
            return (_permissions.FindFirst<OverallCustomizationAllowedPermission>() != null);
        }
        #endregion
    }


    public class OverallCustomizationAllowedPermissionRequest : IPermissionRequest {

        public object GetHashObject() {
            return GetType().FullName;
        }

        public static void Register(XafApplication application) {
            if (application != null) {
                application.SetupComplete += ApplicationOnSetupComplete;
            }
        }

        static void ApplicationOnSetupComplete(object sender, EventArgs eventArgs) {
            if (((XafApplication)sender).Security is SecurityStrategy securityStrategy) {
                ((XafApplication)sender).LoggedOn += ApplicationOnLoggedOn;
                (securityStrategy).CustomizeRequestProcessors += OnCustomizeRequestProcessors;
            }
        }

        static void OnCustomizeRequestProcessors(object sender, CustomizeRequestProcessorsEventArgs customizeRequestProcessorsEventArgs) {
            var requestProcessor = new OverallCustomizationAllowedPermissionRequestProcessor(customizeRequestProcessorsEventArgs.Permissions.WithCustomPermissions());
            customizeRequestProcessorsEventArgs.Processors.Add(new KeyValuePair<Type, IPermissionRequestProcessor>(typeof(OverallCustomizationAllowedPermissionRequest), requestProcessor));
        }

        static void ApplicationOnLoggedOn(object sender, LogonEventArgs logonEventArgs) {
            var xafApplication = ((XafApplication)sender);
            if (((XafApplication) sender).Security.IsRemoteClient())
                return;
            // var modelWinLayoutManagerOptions = ((IModelWinLayoutManagerOptions)xafApplication.Model.Options.LayoutManagerOptions);
            // modelWinLayoutManagerOptions.CustomizationEnabled = SecuritySystem.IsGranted(new OverallCustomizationAllowedPermissionRequest());
        }
    }
}
