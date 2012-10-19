using System;
using System.Collections.Generic;
using System.Linq;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Security;
using DevExpress.ExpressApp.Win.SystemModule;
using Xpand.ExpressApp.Security.Core;
using Xpand.ExpressApp.Security.Permissions;

namespace Xpand.ExpressApp.Security.Win.Permissions {
    public class OverallCustomizationAllowedPermission : IOperationPermission, IPermissionInfo {
        public string Operation {
            get { return "OverallCustomizationAllowed"; }
        }
        #region Implementation of IPermissionInfo
        public IEnumerable<IOperationPermission> GetPermissions(XpandRole xpandRole) {
            return xpandRole.GetMemberValue("ModifyLayout").Equals(true)
                       ? new[] { new OverallCustomizationAllowedPermission() }
                       : Enumerable.Empty<IOperationPermission>();
        }
        #endregion
    }
    public class OverallCustomizationAllowedPermissionRequestProcessor : PermissionRequestProcessorBase<OverallCustomizationAllowedPermissionRequest> {
        private readonly IPermissionDictionary permissions;
        public OverallCustomizationAllowedPermissionRequestProcessor(IPermissionDictionary permissions) {
            this.permissions = permissions;
        }
        #region Overrides of PermissionRequestProcessorBase<OverallCustomizationAllowedPermissionRequest>
        public override bool IsGranted(OverallCustomizationAllowedPermissionRequest permissionRequest) {
            return (permissions.FindFirst<OverallCustomizationAllowedPermission>() != null);
        }
        #endregion
    }


    public class OverallCustomizationAllowedPermissionRequest : IPermissionRequest {

        public object GetHashObject() {
            return GetType().FullName;
        }

        public static void Register(XafApplication application) {
            if (application != null) {
                application.LoggedOn += ApplicationOnLoggedOn;
                application.SetupComplete += ApplicationOnSetupComplete;
            }
        }

        static void ApplicationOnSetupComplete(object sender, EventArgs eventArgs) {
            var securityStrategy = SecuritySystem.Instance as SecurityStrategy;
            if (securityStrategy != null) (securityStrategy).CustomizeRequestProcessors += OnCustomizeRequestProcessors;
        }

        static void OnCustomizeRequestProcessors(object sender, CustomizeRequestProcessorsEventArgs customizeRequestProcessorsEventArgs) {
            var requestProcessor = new OverallCustomizationAllowedPermissionRequestProcessor(customizeRequestProcessorsEventArgs.Permissions);
            customizeRequestProcessorsEventArgs.Processors.Add(new KeyValuePair<Type, IPermissionRequestProcessor>(typeof(OverallCustomizationAllowedPermissionRequest), requestProcessor));
        }

        static void ApplicationOnLoggedOn(object sender, LogonEventArgs logonEventArgs) {
            var xafApplication = ((XafApplication)sender);
            var modelWinLayoutManagerOptions = ((IModelWinLayoutManagerOptions)xafApplication.Model.Options.LayoutManagerOptions);
            modelWinLayoutManagerOptions.CustomizationEnabled = SecuritySystem.IsGranted(new OverallCustomizationAllowedPermissionRequest());
        }
    }
}
