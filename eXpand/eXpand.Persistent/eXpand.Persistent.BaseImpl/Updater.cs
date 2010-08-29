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
        private const string Administrators = "Administrators";
        protected Updater(Session session, Version currentDBVersion) : base(session, currentDBVersion) { }


        protected virtual List<IPermission> GetDenyPermissions(Role role) {
            if (role.Name!=Administrators)
                return new List<IPermission> {
                                             new ObjectAccessPermission(typeof(Role), ObjectAccess.AllAccess, ObjectAccessModifier.Deny)
                                         };
            return new List<IPermission>();
        }

        protected void InitializeSecurity()
        {
            Role admins = EnsureRoleExists(Administrators, GetDenyPermissions);
            EnsureUserExists("admin", "Administrator",admins);

            Role userRole = EnsureRoleExists("userRole", GetDenyPermissions);
            EnsureUserExists("user", "user", userRole);
        }


        protected User EnsureUserExists(string userName, string firstName, Role role)
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

        protected Role EnsureRoleExists(string roleName,Func<Role,List<IPermission>> permissionAddFunc)
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
