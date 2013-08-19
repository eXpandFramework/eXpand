using System;
using AuditTrailTester.Module.BusinessObjects;
using DevExpress.ExpressApp.Security;
using DevExpress.ExpressApp.Security.Strategy;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Updating;
using Xpand.ExpressApp.AuditTrail.Security;
using Xpand.ExpressApp.Security.Core;

namespace AuditTrailTester.Module.DatabaseUpdate {
    
    public class Updater : ModuleUpdater {
        public Updater(IObjectSpace objectSpace, Version currentDBVersion) :
            base(objectSpace, currentDBVersion) {
        }
        
        public override void UpdateDatabaseAfterUpdateSchema() {
            base.UpdateDatabaseAfterUpdateSchema();
            var defaultRole = (SecuritySystemRole)ObjectSpace.GetDefaultRole();

            var adminRole = ObjectSpace.GetAdminRole("Admin");
            adminRole.GetUser("Admin");

            var userRole = ObjectSpace.GetRole("User");
            userRole.CanEditModel = true;
            userRole.SetTypePermissionsRecursively<object>(SecurityOperations.FullAccess,SecuritySystemModifier.Allow);
            
            var user = (SecuritySystemUser)userRole.GetUser("user");
            user.Roles.Add(defaultRole);

            if (ObjectSpace.IsNewObject(userRole)) {
                var permissionData = ObjectSpace.CreateObject<AuditTrailOperationPermissionData>();
                permissionData.ObjectTypeData = typeof (Employee);
                permissionData.ID = "audit_emplyee";
                permissionData.AuditTrailMembersContext = "Employee_Members";
                ((XpandRole) userRole).Permissions.Add(permissionData);
            }
            ObjectSpace.CommitChanges();
        }

    }
}
