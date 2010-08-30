using System;
using System.Diagnostics;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Model.Core;
using DevExpress.ExpressApp.Security;
using DevExpress.ExpressApp.Updating;
using DevExpress.Xpo;
using DevExpress.Data.Filtering;
using DevExpress.Persistent.BaseImpl;
using eXpand.ExpressApp.FilterDataStore.Providers;
using FeatureCenter.Base;
using System.Linq;

namespace FeatureCenter.Module
{
    public class Updater : ModuleUpdater
    {
        public Updater(Session session, Version currentDBVersion) : base(session, currentDBVersion) { }
        public override void UpdateDatabaseAfterUpdateSchema()
        {
            base.UpdateDatabaseAfterUpdateSchema();
            InitializeSecurity();
            new DummyDataBuilder(Session).CreateObjects();
        }


        private void InitializeSecurity()
        {

            User admin = EnsureUserExists("admin", "Administrator");
            UserFilterProvider.UpdaterUserKey = admin.Oid;
            User user = EnsureUserExists("user", "User");

            Role admins = EnsureRoleExists("Administrators");
            Role users = EnsureRoleExists("Users");
            ApplyDenySecurityAccessPermissions(users);
            ApplyModelEditingPermission(admins);
            admin.Roles.Add(admins);
            user.Roles.Add(users);

            admin.Save();
            user.Save();
        }


        public static User EnsureUserExists(Session session,string userName, string firstName) {
            var user = session.FindObject<User>(new BinaryOperator("UserName", userName));
            if (user == null)
            {
                user = new User(session) { UserName = userName, FirstName = firstName };
                user.SetPassword("");
                user.Save();
            }
            return user;
        }
        private User EnsureUserExists(string userName, string firstName)
        {
            return EnsureUserExists(Session, userName, firstName);
        }

        public static Role EnsureRoleExists(Session session,string roleName) {
            var role = session.FindObject<Role>(new BinaryOperator("Name", roleName));
            if (role == null)
            {
                role = new Role(session) { Name = roleName };

                while (role.PersistentPermissions.Count > 0)
                {
                    session.Delete(role.PersistentPermissions[0]);
                }
                ApplyDefaultPermissions(role);
                role.Save();
            }

            return role;
        }
        private Role EnsureRoleExists(string roleName)
        {
            return EnsureRoleExists(Session,roleName);
        }


        private static void ApplyDefaultPermissions(RoleBase role)
        {
            role.AddPermission(new ObjectAccessPermission(typeof(object), ObjectAccess.AllAccess));
            role.Save();
        }

        public static void ApplyModelEditingPermission(RoleBase role)
        {
            if (role.Permissions.OfType<EditModelPermission>().Count()==0) {
                role.AddPermission(new EditModelPermission(ModelAccessModifier.Allow));
                role.Save();
            }
        }
        

        private static void ApplyDenySecurityAccessPermissions(RoleBase role)
        {
            if (role.Permissions.OfType<ObjectAccessPermission>().Count()==0) {
                role.AddPermission(new ObjectAccessPermission(typeof(Role), ObjectAccess.AllAccess,
                                                              ObjectAccessModifier.Deny));
                role.Save();

            }
        }
    }
}
