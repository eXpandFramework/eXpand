using DevExpress.ExpressApp.Security;
using Xpand.ExpressApp.Logic.Conditional.Security.Improved;

namespace Xpand.ExpressApp.AdditionalViewControlsProvider.Security.Improved {
    public class AdditionalViewControlsRuleRequestProcessor : ConditionalLogicRuleRequestProcessor<AdditionalViewControlsPermissionRequest> {
        protected override bool IsRequestFit(AdditionalViewControlsPermissionRequest permissionRequest, OperationPermissionBase permission, IRequestSecurityStrategy securityInstance) {
            var isRequestFit = base.IsRequestFit(permissionRequest, permission, securityInstance);
            if (isRequestFit && permission is AdditionalViewControlsPermission) {
                var requestFit = permissionRequest.ControlType == ((AdditionalViewControlsPermission)permission).ControlType && 
                    permissionRequest.Message == ((AdditionalViewControlsPermission)permission).Message&&
                    permissionRequest.MessageProperty == ((AdditionalViewControlsPermission)permission).MessageProperty&&
                    permissionRequest.Position == ((AdditionalViewControlsPermission)permission).Position&&
                    permissionRequest.BackColor == ((AdditionalViewControlsPermission)permission).BackColor&&
                    permissionRequest.ForeColor == ((AdditionalViewControlsPermission)permission).ForeColor&&
                    permissionRequest.FontStyle == ((AdditionalViewControlsPermission)permission).FontStyle&&
                    permissionRequest.Height == ((AdditionalViewControlsPermission)permission).Height&&
                    permissionRequest.FontSize == ((AdditionalViewControlsPermission)permission).FontSize&&
                     permissionRequest.DecoratorType == ((AdditionalViewControlsPermission)permission).DecoratorType;
                return requestFit;
            }
            return false;
        }

    }
}