using DevExpress.Data.Filtering;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Security;
using DevExpress.ExpressApp.Security.Strategy;

namespace Xpand.Persistent.Base {
    public class ModuleUpdaterHelper {
        public static void CreateDummyUsers(IObjectSpace ObjectSpace) {
            var adminRole =
                ObjectSpace.FindObject<SecuritySystemRole>(new BinaryOperator("Name",
                                                                              SecurityStrategy.AdministratorRoleName));
            if (adminRole == null) {
                adminRole = ObjectSpace.CreateObject<SecuritySystemRole>();
                adminRole.Name = SecurityStrategy.AdministratorRoleName;
                adminRole.IsAdministrative = true;
                adminRole.Save();
            }

            var userRole = ObjectSpace.FindObject<SecuritySystemRole>(new BinaryOperator("Name", "User"));
            if (userRole == null) {
                userRole = ObjectSpace.CreateObject<SecuritySystemRole>();
                userRole.Name = "User";
                var userTypePermission = ObjectSpace.CreateObject<SecuritySystemTypePermissionObject>();
                userTypePermission.TargetType = typeof(SecuritySystemUser);
                var currentUserObjectPermission = ObjectSpace.CreateObject<SecuritySystemObjectPermissionsObject>();
                currentUserObjectPermission.Criteria = "[Oid] = CurrentUserId()";
                currentUserObjectPermission.AllowNavigate = true;
                currentUserObjectPermission.AllowRead = true;
                userTypePermission.ObjectPermissions.Add(currentUserObjectPermission);
                userRole.TypePermissions.Add(userTypePermission);
            }

            var user1 = ObjectSpace.FindObject<SecuritySystemUser>(
                new BinaryOperator("UserName", "Sam"));
            if (user1 == null) {
                user1 = ObjectSpace.CreateObject<SecuritySystemUser>();
                user1.UserName = "Sam";
                // Set a password if the standard authentication type is used 
                user1.SetPassword("");
            }
            // If a user named 'John' doesn't exist in the database, create this user 
            var user2 = ObjectSpace.FindObject<SecuritySystemUser>(
                new BinaryOperator("UserName", "John"));
            if (user2 == null) {
                user2 = ObjectSpace.CreateObject<SecuritySystemUser>();
                user2.UserName = "John";
                // Set a password if the standard authentication type is used 
                user2.SetPassword("");
            }
            user1.Roles.Add(adminRole);
            user2.Roles.Add(userRole);
        }
    }
}