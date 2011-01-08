using System;
using System.Collections.Generic;
using System.Security;
using DevExpress.Data.Filtering;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Security;
using DevExpress.ExpressApp.Updating;
using DevExpress.Persistent.Base.Security;

namespace Xpand.Persistent.BaseImpl {
    public abstract class Updater : ModuleUpdater {
        public const string Administrators = "Administrators";
        public const string UserRole = "UserRole";
        public const string Admin = "Admin";


        protected Updater(ObjectSpace objectSpace, Version currentDBVersion) : base(objectSpace, currentDBVersion) {
        }

        protected virtual List<IPermission> GetPermissions(ICustomizableRole role) {
            var permissions = new List<IPermission>();
            if (role.Name != Administrators) {
                permissions.Add(new ObjectAccessPermission(((ISecurityComplex)SecuritySystem.Instance).RoleType, ObjectAccess.AllAccess, ObjectAccessModifier.Deny));
            }
            else {
                permissions.Add(new EditModelPermission(ModelAccessModifier.Allow));
            }
            return permissions;
        }

        protected virtual void InitializeSecurity() {
            ICustomizableRole admins = EnsureRoleExists(Administrators, GetPermissions);
            EnsureUserExists(Admin, "Administrator", admins);

            ICustomizableRole userRole = EnsureRoleExists(UserRole, GetPermissions);
            EnsureUserExists("user", "user", userRole);
            ObjectSpace.CommitChanges();
        }


        protected virtual IUserWithRoles EnsureUserExists(string userName, string firstName, ICustomizableRole role,Type userType) {
            var type = userType;
            var user = (IUserWithRoles)ObjectSpace.Session.FindObject(type, new BinaryOperator("UserName", userName));
            if (user == null) {
                user = (IUserWithRoles)ObjectSpace.CreateObject(type);
                var typeInfo = XafTypesInfo.Instance.FindTypeInfo(type);
                typeInfo.FindMember("UserName").SetValue(user, userName);
                var memberInfo = typeInfo.FindMember("FirstName");
                if (memberInfo != null) memberInfo.SetValue(user, userName);
                user.Roles.Add(role);
            }
            return user;
        }

        protected virtual IUserWithRoles EnsureUserExists(string userName, string firstName, ICustomizableRole role) {
            return EnsureUserExists(userName, firstName, role, SecuritySystem.UserType);
        }

        protected virtual ICustomizableRole EnsureRoleExists(string roleName, Func<ICustomizableRole, List<IPermission>> permissionAddFunc) {
            var role = (ICustomizableRole)ObjectSpace.Session.FindObject(((ISecurityComplex)SecuritySystem.Instance).RoleType, new BinaryOperator("Name", roleName));
            if (role == null) {
                role = (ICustomizableRole)ObjectSpace.CreateObject(((ISecurityComplex)SecuritySystem.Instance).RoleType);
                role.Name = roleName;
                role.AddPermission(new ObjectAccessPermission(typeof(object), ObjectAccess.AllAccess));
                foreach (var permission in permissionAddFunc.Invoke(role)) {
                    role.AddPermission(permission);
                }
            }

            return role;

        }






    }
}
