using System;
using System.Collections.Generic;
using System.Security;
using DevExpress.Data.Filtering;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Security;
using DevExpress.ExpressApp.Updating;
using DevExpress.Persistent.Base.Security;
using DevExpress.Xpo;
using Xpand.ExpressApp.Security.Core;
using System.Linq;

namespace Xpand.Persistent.BaseImpl {
    public abstract class Updater : ModuleUpdater {
        public const string UserRole = "UserRole";
        public const string Admin = "Admin";

        protected Updater(IObjectSpace objectSpace, Version currentDBVersion)
            : base(objectSpace, currentDBVersion) {
        }


        public virtual List<object> GetPermissions(object role) {
            return !IsNewSecuritySystem ? GetPermissions((ICustomizableRole)role, new List<object>()) : GetPermissions(((SecurityRole)role), new List<object>());
        }

        private List<object> GetPermissions(ICustomizableRole role, List<object> permissions) {
            if (role.Name != SecurityStrategy.AdministratorRoleName) {
                permissions.Add(new ObjectAccessPermission(RoleType, ObjectAccess.AllAccess, ObjectAccessModifier.Deny));
            } else {
                permissions.Add(new EditModelPermission(ModelAccessModifier.Allow));
            }
            return permissions;
        }

        public virtual Type RoleType {
            get { return ((ISecurityComplex)SecuritySystem.Instance).RoleType; }
        }


        protected virtual bool InitializeSecurity() {
            EnsureAnonymousUser();
            var admins = EnsureRoleExists(SecurityStrategy.AdministratorRoleName, GetPermissions);
            EnsureUserExists(Admin, "Administrator", admins);
            if (!ObjectSpace.IsNewObject(admins))
                return false;
            var userRole = EnsureRoleExists(UserRole, GetPermissions);
            EnsureUserExists("user", "user", userRole);
            ObjectSpace.CommitChanges();
            return true;

        }

        public void EnsureAnonymousUser() {
            if (IsNewSecuritySystem) {
                var anonymousUser = (ISecurityUser)ObjectSpace.FindObject(UserType, new BinaryOperator("UserName", SecurityStrategy.AnonymousUserName));
                if (anonymousUser == null) {
                    anonymousUser = (ISecurityUser)ObjectSpace.CreateObject(UserType);
                    ((XPBaseObject)anonymousUser).SetMemberValue("UserName", SecurityStrategy.AnonymousUserName);
                    anonymousUser.IsActive = true;
                    ((IAuthenticationStandardUser)anonymousUser).SetPassword("");
                }
            }
        }


        public virtual object EnsureUserExists(string userName, string firstName, object role, Type userType) {
            return IsNewSecuritySystem ? EnsureUserExists(userName, role) : EnsureUserExists(userName, role, userType);
        }

        private object EnsureUserExists(string userName, object role, Type userType) {
            var user =
                (IUserWithRoles)
                ((ObjectSpace)ObjectSpace).Session.FindObject(userType, new BinaryOperator("UserName", userName));
            if (user == null) {
                user = (IUserWithRoles)ObjectSpace.CreateObject(userType);
                var typeInfo = XafTypesInfo.Instance.FindTypeInfo(userType);
                typeInfo.FindMember("UserName").SetValue(user, userName);
                var memberInfo = typeInfo.FindMember("FirstName");
                if (memberInfo != null) memberInfo.SetValue(user, userName);
                user.Roles.Add((IRole)role);
            }
            return user;
        }

        private object EnsureUserExists(string userName, object role) {
            var securityUser = ObjectSpace.FindObject(UserType, new BinaryOperator("UserName", userName)) as ISecurityUser;
            if (securityUser == null) {
                var strategyRole = role;
                securityUser = (ISecurityUser)ObjectSpace.CreateObject(UserType);
                ((XPBaseObject)securityUser).SetMemberValue("UserName", userName);
                securityUser.IsActive = true;
                ((IAuthenticationStandardUser)securityUser).SetPassword("");
                ((ISecurityUserWithRoles)securityUser).Roles.Add((ISecurityRole)strategyRole);
            }
            return securityUser;
        }

        public virtual object EnsureUserExists(string userName, string firstName, object role) {
            return EnsureUserExists(userName, firstName, role, UserType);
        }

        public virtual Type UserType {
            get { return SecuritySystem.UserType; }
        }

        private List<object> GetPermissions(SecurityRole securityRole, List<object> permissions) {
            if (securityRole.Name == SecurityStrategy.AdministratorRoleName) {
                var modelPermission = ObjectSpace.CreateObject<ModelOperationPermissionData>();
                modelPermission.Save();
                securityRole.BeginUpdate();
                securityRole.Permissions.GrantRecursive(typeof(object), SecurityOperations.Read);
                securityRole.Permissions.GrantRecursive(typeof(object), SecurityOperations.Write);
                securityRole.Permissions.GrantRecursive(typeof(object), SecurityOperations.Create);
                securityRole.Permissions.GrantRecursive(typeof(object), SecurityOperations.Delete);
                securityRole.Permissions.GrantRecursive(typeof(object), SecurityOperations.Navigate);
                securityRole.EndUpdate();
                permissions.Add(modelPermission);
            }
            return permissions;
        }

        public virtual object EnsureRoleExists(string roleName, Func<object, List<object>> permissionAddFunc) {
            if (IsNewSecuritySystem) {
                var securityRole = ObjectSpace.FindObject<SecurityRole>(new BinaryOperator("Name", SecurityStrategy.AdministratorRoleName));
                return EnsureRoleExists(roleName, permissionAddFunc, securityRole);
            }
            var role = (ICustomizableRole)((ObjectSpace)ObjectSpace).Session.FindObject(RoleType, new BinaryOperator("Name", roleName));
            return EnsureRoleExists(roleName, permissionAddFunc, role);
        }

        private object EnsureRoleExists(string roleName, Func<object, List<object>> permissionAddFunc, ICustomizableRole role) {
            if (role == null) {
                role = (ICustomizableRole)ObjectSpace.CreateObject(RoleType);
                role.Name = roleName;
                role.AddPermission(new ObjectAccessPermission(typeof(object), ObjectAccess.AllAccess));
                foreach (var permission in permissionAddFunc.Invoke(role).OfType<IPermission>()) {
                    role.AddPermission(permission);
                }
            }
            return role;
        }

        private object EnsureRoleExists(string roleName, Func<object, List<object>> permissionAddFunc, SecurityRole securityRole) {
            if (securityRole == null) {
                securityRole = ObjectSpace.CreateObject<SecurityRole>();
                securityRole.Name = roleName;
                if (permissionAddFunc != null)
                    foreach (var permission in permissionAddFunc.Invoke(securityRole).OfType<PermissionData>()) {
                        securityRole.PersistentPermissions.Add(permission);
                    }
                securityRole.Save();
            }
            return securityRole;
        }

        public virtual bool IsNewSecuritySystem {
            get { return ((ISecurityComplex)SecuritySystem.Instance).IsNewSecuritySystem(); }
        }
    }
}
