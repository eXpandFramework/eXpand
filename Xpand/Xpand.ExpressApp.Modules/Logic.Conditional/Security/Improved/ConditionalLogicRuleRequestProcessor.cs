using DevExpress.ExpressApp.Security;
using Xpand.ExpressApp.Logic.Security.Improved;

namespace Xpand.ExpressApp.Logic.Conditional.Security.Improved {
    public abstract class ConditionalLogicRuleRequestProcessor<T> : LogicRuleRequestProcessor<T> where T:ConditionalLogicRulePermissionRequest {
        protected override bool IsRequestFit(T permissionRequest, OperationPermissionBase permission, IRequestSecurityStrategy securityInstance) {
            var isRequestFit = base.IsRequestFit(permissionRequest, permission, securityInstance);
            if (isRequestFit&& permission is ConditionalLogicRulePermission) {
                var requestFit = permissionRequest.NormalCriteria == ((ConditionalLogicRulePermission)permission).NormalCriteria && permissionRequest.EmptyCriteria == ((ConditionalLogicRulePermission)permission).EmptyCriteria;
                return  requestFit;
            }
            return false;
        }
    }
}