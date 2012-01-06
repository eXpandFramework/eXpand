using DevExpress.ExpressApp.Security;

namespace Xpand.ExpressApp.Logic.Security.Improved {
    public class LogicRuleRequestProcessor<T> : PermissionRequestProcessorBase<T> where T : LogicRulePermissionRequest {

        protected override bool IsRequestFit(T permissionRequest, OperationPermissionBase permission, IRequestSecurityStrategy securityInstance) {
            if (permission is LogicRulePermission) {
                return permissionRequest.ExecutionContextGroup ==((LogicRulePermission) permission).ExecutionContextGroup &&
                       permissionRequest.FrameTemplateContextGroup == ((LogicRulePermission)permission).FrameTemplateContextGroup &&
                       permissionRequest.ID == ((LogicRulePermission)permission).ID &&
                       permissionRequest.Index == ((LogicRulePermission)permission).Index &&
                       permissionRequest.IsRootView == ((LogicRulePermission)permission).IsRootView &&
                       permissionRequest.Nesting == ((LogicRulePermission)permission).Nesting &&
                       permissionRequest.ObjectType == ((LogicRulePermission)permission).ObjectType &&
                       permissionRequest.ViewContextGroup == ((LogicRulePermission)permission).ViewContextGroup &&
                       permissionRequest.ViewEditMode == ((LogicRulePermission)permission).ViewEditMode &&
                       permissionRequest.ViewId == ((LogicRulePermission)permission).ViewId &&
                       permissionRequest.ViewType == ((LogicRulePermission)permission).ViewType &&
                       permissionRequest.FrameTemplateContext == ((LogicRulePermission) permission).FrameTemplateContext;
            }
            return false;

        }
    }
}