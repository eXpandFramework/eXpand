using System;
using AuditTrailTester.Module.BusinessObjects;
using DevExpress.ExpressApp.Security;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Updating;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.BaseImpl.PermissionPolicy;
using Xpand.ExpressApp.Security.Core;
using Xpand.Persistent.BaseImpl.AuditTrail;
using Xpand.Persistent.BaseImpl.Security;

namespace AuditTrailTester.Module.DatabaseUpdate {
    
    public class Updater : ModuleUpdater {
        public Updater(IObjectSpace objectSpace, Version currentDBVersion) :
            base(objectSpace, currentDBVersion) {
        }
        
        public override void UpdateDatabaseAfterUpdateSchema() {
            base.UpdateDatabaseAfterUpdateSchema();
            var defaultRole = (PermissionPolicyRole)ObjectSpace.GetDefaultRole();

            var adminRole = ObjectSpace.GetAdminRole("Admin");
            adminRole.GetUser("Admin");

            var userRole = (PermissionPolicyRole)ObjectSpace.GetRole("User");
            userRole.CanEditModel = true;
            userRole.AddTypePermissionsRecursively<object>(SecurityOperations.FullAccess,SecurityPermissionState.Allow);
            
            var user = (PermissionPolicyUser)userRole.GetUser("user");
            user.Roles.Add(defaultRole);

            if (ObjectSpace.IsNewObject(userRole)) {
                var permissionData = ObjectSpace.CreateObject<AuditTrailOperationPermissionPolicyPolicyData>();
                permissionData.ObjectTypeData = typeof (Employee);
                permissionData.ID = "audit_emplyee";
                permissionData.AuditTrailMembersContext = "Employee_Members";
                ((XpandPermissionPolicyRole) userRole).Permissions.Add(permissionData);
            }
            ObjectSpace.CommitChanges();
        }

    }
}
