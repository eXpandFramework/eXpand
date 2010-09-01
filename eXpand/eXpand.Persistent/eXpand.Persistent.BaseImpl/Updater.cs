using System;
using System.Collections.Generic;
using System.Security;
using DevExpress.ExpressApp.Security;
using DevExpress.ExpressApp.Updating;
using DevExpress.Xpo;
using DevExpress.Data.Filtering;
using DevExpress.Persistent.BaseImpl;

namespace eXpand.Persistent.BaseImpl
{
    public abstract class Updater : ModuleUpdater
    {
        public const string Administrators = "Administrators";
        public const string UserRole = "UserRole";
        protected Updater(Session session, Version currentDBVersion) : base(session, currentDBVersion) { }


        protected virtual List<IPermission> GetPermissions(Role role) {
            if (role.Name!=Administrators)
                return new List<IPermission> {
                                             new ObjectAccessPermission(typeof(Role), ObjectAccess.AllAccess, ObjectAccessModifier.Deny)
                                         };
            return new List<IPermission>();
        }

        protected virtual void InitializeSecurity()
        {
            Role admins = EnsureRoleExists(Administrators, GetPermissions);
            EnsureUserExists("admin", "Administrator",admins);

            Role userRole = EnsureRoleExists(UserRole, GetPermissions);
            EnsureUserExists("user", "user", userRole);
        }


        protected virtual User EnsureUserExists(string userName, string firstName, Role role)
        {
            var user = Session.FindObject<User>(new BinaryOperator("UserName", userName));
            if (user == null){
                user = new User(Session) { UserName = userName, FirstName = firstName };
                user.SetPassword("");
                user.Roles.Add(role);
                user.Save();
            }
            return user;
        }

        protected virtual Role EnsureRoleExists(string roleName,Func<Role,List<IPermission>> permissionAddFunc)
        {
            var role = Session.FindObject<Role>(new BinaryOperator("Name", roleName));
            if (role == null){
                role = new Role(Session) { Name = roleName };
                while (role.PersistentPermissions.Count > 0){
                    Session.Delete(role.PersistentPermissions[0]);
                }
                role.AddPermission(new ObjectAccessPermission(typeof(object), ObjectAccess.AllAccess));
                foreach (var permission in permissionAddFunc.Invoke(role)) {
                    role.AddPermission(permission);
                }
                role.Save();
            }

            return role;

        }






    }
}
