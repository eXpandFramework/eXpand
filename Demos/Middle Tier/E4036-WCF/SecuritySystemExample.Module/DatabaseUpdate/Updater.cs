// Developer Express Code Central Example:
// How to: Implement middle tier security with the .NET Remoting service
// 
// The complete description is available in the Middle Tier Security - .NET
// Remoting Service (http://documentation.devexpress.com/#xaf/CustomDocument3438)
// topic.
// 
// You can find sample updates and versions for different programming languages here:
// http://www.devexpress.com/example=E4035

using System;

using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Updating;
using DevExpress.Xpo;
using DevExpress.Data.Filtering;
using DevExpress.Persistent.BaseImpl;
using DevExpress.ExpressApp.Security;
using SecuritySystemExample.Module.BusinessObjects;
using DevExpress.ExpressApp.Security.Strategy;

namespace SecuritySystemExample.Module.DatabaseUpdate {
    public class Updater : ModuleUpdater {
        public Updater(IObjectSpace objectSpace, Version currentDBVersion) : base(objectSpace, currentDBVersion) { }
        public override void UpdateDatabaseAfterUpdateSchema() {
            base.UpdateDatabaseAfterUpdateSchema();
            // Administrative role
            SecuritySystemRole adminRole = ObjectSpace.FindObject<SecuritySystemRole>(
               new BinaryOperator("Name", SecurityStrategy.AdministratorRoleName));
            if (adminRole == null) {
                adminRole = ObjectSpace.CreateObject<SecuritySystemRole>();
                adminRole.Name = SecurityStrategy.AdministratorRoleName;
                adminRole.IsAdministrative = true;
            }
            // Administrator user
            SecuritySystemUser adminUser = ObjectSpace.FindObject<SecuritySystemUser>(
                new BinaryOperator("UserName", "Administrator"));
            if (adminUser == null) {
                adminUser = ObjectSpace.CreateObject<SecuritySystemUser>();
                adminUser.UserName = "Administrator";
                adminUser.SetPassword("");
                adminUser.Roles.Add(adminRole);
            }
            // A role whith type-level permissions
            SecuritySystemRole contactsManagerRole =
                ObjectSpace.FindObject<SecuritySystemRole>(
                new BinaryOperator("Name", "Contacts Manager"));
            if (contactsManagerRole == null) {
                contactsManagerRole = ObjectSpace.CreateObject<SecuritySystemRole>();
                contactsManagerRole.Name = "Contacts Manager";
                SecuritySystemTypePermissionObject contactTypePermission =
                    ObjectSpace.CreateObject<SecuritySystemTypePermissionObject>();
                contactTypePermission.TargetType = typeof(Contact);
                contactTypePermission.AllowCreate = true;
                contactTypePermission.AllowDelete = true;
                contactTypePermission.AllowNavigate = true;
                contactTypePermission.AllowRead = true;
                contactTypePermission.AllowWrite = true;
                contactsManagerRole.TypePermissions.Add(contactTypePermission);
            }
            SecuritySystemUser userSam = 
                ObjectSpace.FindObject<SecuritySystemUser>(
                new BinaryOperator("UserName", "Sam"));
            if (userSam == null) {
                userSam = ObjectSpace.CreateObject<SecuritySystemUser>();
                userSam.UserName = "Sam";
                userSam.SetPassword("");
                userSam.Roles.Add(contactsManagerRole);
            }
            // A role with object-level permissions
            SecuritySystemRole basicUserRole =
                ObjectSpace.FindObject<SecuritySystemRole>(
                new BinaryOperator("Name", "Basic User"));
            if (basicUserRole == null) {
                basicUserRole = ObjectSpace.CreateObject<SecuritySystemRole>();
                basicUserRole.Name = "Basic User";
                SecuritySystemTypePermissionObject userTypePermission = 
                    ObjectSpace.CreateObject<SecuritySystemTypePermissionObject>();
                userTypePermission.TargetType = typeof(SecuritySystemUser);
                SecuritySystemObjectPermissionsObject currentUserObjectPermission = 
                    ObjectSpace.CreateObject<SecuritySystemObjectPermissionsObject>();
                currentUserObjectPermission.Criteria = "[Oid] = CurrentUserId()";
                currentUserObjectPermission.AllowNavigate = true;
                currentUserObjectPermission.AllowRead = true;
                userTypePermission.ObjectPermissions.Add(currentUserObjectPermission);
                basicUserRole.TypePermissions.Add(userTypePermission);
            }
            SecuritySystemUser userJohn =
                ObjectSpace.FindObject<SecuritySystemUser>(
                new BinaryOperator("UserName", "John"));
            if (userJohn == null) {
                userJohn = ObjectSpace.CreateObject<SecuritySystemUser>();
                userJohn.UserName = "John";
                userJohn.SetPassword("");
                userJohn.Roles.Add(basicUserRole);
            }
            // A role with member-level permissions
            SecuritySystemRole contactViewerRole =
                ObjectSpace.FindObject<SecuritySystemRole>(
                new BinaryOperator("Name", "Contact Viewer"));
            if (contactViewerRole == null) {
                contactViewerRole = ObjectSpace.CreateObject<SecuritySystemRole>();
                contactViewerRole.Name = "Contact Viewer";
                SecuritySystemTypePermissionObject contactLimitedTypePermission =
                    ObjectSpace.CreateObject<SecuritySystemTypePermissionObject>();
                contactLimitedTypePermission.TargetType = typeof(Contact);
                contactLimitedTypePermission.AllowNavigate = true;
                SecuritySystemMemberPermissionsObject contactMemberPermission =
                    ObjectSpace.CreateObject<SecuritySystemMemberPermissionsObject>();
                contactMemberPermission.Members = "Name";
                contactMemberPermission.AllowRead = true;
                contactLimitedTypePermission.MemberPermissions.Add(contactMemberPermission);
                contactViewerRole.TypePermissions.Add(contactLimitedTypePermission);
            }
            SecuritySystemUser userBill =
                ObjectSpace.FindObject<SecuritySystemUser>(
                new BinaryOperator("UserName", "Bill"));
            if (userBill == null) {
                userBill = ObjectSpace.CreateObject<SecuritySystemUser>();
                userBill.UserName = "Bill";
                userBill.SetPassword("");
                userBill.Roles.Add(contactViewerRole);
            }
            // Contact objects are created for demo purposes
            Contact contactMary = ObjectSpace.FindObject<Contact>(
                new BinaryOperator("Name", "Mary Tellitson"));
            if (contactMary == null) {
                contactMary = ObjectSpace.CreateObject<Contact>();
                contactMary.Name = "Mary Tellitson";
                contactMary.Email = "mary_tellitson@example.com";
            }
            Contact contactJohn = ObjectSpace.FindObject<Contact>(
                new BinaryOperator("Name","John Nilsen"));
            if (contactJohn == null) {
                contactJohn = ObjectSpace.CreateObject<Contact>();
                contactJohn.Name = "John Nilsen";
                contactJohn.Email = "john_nilsen@example.com";
            }
        }
    }
}
