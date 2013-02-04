﻿﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using System.Security.Permissions;
using DevExpress.Data.Filtering;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Security;
using DevExpress.ExpressApp.Security.Strategy;
using DevExpress.ExpressApp.Security.Strategy.PermissionMatrix;
using DevExpress.ExpressApp.Xpo;
using DevExpress.Persistent.Base.Security;
using DevExpress.Xpo;
using Xpand.Utils.Helpers;

namespace Xpand.ExpressApp.Security.Core {
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class FullPermissionAttribute : Attribute {
    }
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class PermissionBehaviorAttribute : Attribute {
        readonly string _name;

        public PermissionBehaviorAttribute(object @enum) {
            if (!@enum.GetType().IsEnum)
                throw new NotImplementedException();
            _name = Enum.GetName(@enum.GetType(), @enum);
        }

        public PermissionBehaviorAttribute(string name) {
            _name = name;
        }

        public string Name {
            get { return _name; }
        }
    }
    public static class SecuritySystemExtensions {

        public static SecuritySystemRoleBase GetDefaultRole(this IObjectSpace objectSpace, string roleName) {
            var defaultRole = objectSpace.GetRole(roleName);
            if (objectSpace.IsNewObject(defaultRole)) {
                defaultRole.AddObjectAccessPermission(SecuritySystem.UserType, "[Oid] = CurrentUserId()", SecurityOperations.ReadOnlyAccess);
                defaultRole.AddMemberAccessPermission(XpandModuleBase.UserType, "ChangePasswordOnFirstLogon,StoredPassword", SecurityOperations.Write);
                defaultRole.SetTypePermissions(((IRoleTypeProvider)SecuritySystem.Instance).RoleType, SecurityOperations.Read, SecuritySystemModifier.Allow);
            }
            return defaultRole;
        }

        public static SecuritySystemRoleBase GetDefaultRole(this IObjectSpace objectSpace) {
            return objectSpace.GetDefaultRole("Default");
        }

        public static ISecurityUserWithRoles GetUser(this SecuritySystemRoleBase systemRole, string userName, string passWord = "") {
            var objectSpace = XPObjectSpace.FindObjectSpaceByObject(systemRole);
            return GetUser(objectSpace, userName, passWord, systemRole);
        }

        public static ISecurityUserWithRoles GetUser(this IObjectSpace objectSpace, string userName, string passWord = "", params SecuritySystemRoleBase[] roles) {
            return (ISecurityUserWithRoles)objectSpace.FindObject(SecuritySystem.UserType, new BinaryOperator("UserName", userName)) ??
                        CreateUser(objectSpace, userName, passWord, roles);
        }

        public static ISecurityUserWithRoles CreateUser(IObjectSpace objectSpace, string userName, string passWord, IEnumerable<SecuritySystemRoleBase> roles) {
            var user2 = (ISecurityUserWithRoles)objectSpace.CreateObject(SecuritySystem.UserType);
            var typeInfo = objectSpace.TypesInfo.FindTypeInfo(user2.GetType());
            typeInfo.FindMember("UserName").SetValue(user2, userName);
            typeInfo.Type.GetMethod("SetPassword").Invoke(user2, new object[] { passWord });
            var roleCollection = (XPBaseCollection)typeInfo.FindMember("Roles").GetValue(user2);
            foreach (var role in roles) {
                roleCollection.BaseAdd(role);
            }
            return user2;
        }

        public static SecuritySystemRoleBase GetAdminRole(this IObjectSpace objectSpace, string roleName) {
            var administratorRole = (SecuritySystemRoleBase)objectSpace.FindObject(XpandModuleBase.RoleType, new BinaryOperator("Name", roleName));
            if (administratorRole == null) {
                administratorRole = (SecuritySystemRoleBase)objectSpace.CreateObject(XpandModuleBase.RoleType);
                administratorRole.Name = roleName;
                administratorRole.IsAdministrative = true;
            }
            return administratorRole;
        }

        public static SecuritySystemRoleBase GetRole(this IObjectSpace objectSpace, string roleName) {
            var securityDemoRole = (SecuritySystemRoleBase)objectSpace.FindObject(XpandModuleBase.RoleType, new BinaryOperator("Name", roleName));
            if (securityDemoRole == null) {
                securityDemoRole = (SecuritySystemRoleBase)objectSpace.CreateObject(XpandModuleBase.RoleType);
                securityDemoRole.Name = roleName;
            }
            return securityDemoRole;
        }

        public static SecuritySystemTypePermissionObject CreateTypePermission<TObject>(this SecuritySystemRoleBase systemRole, Action<SecuritySystemTypePermissionObject> action, bool defaultAllowValues = true) {
            var targetType = typeof(TObject);
            var permission = systemRole.CreateTypePermission<TObject>();
            permission.TargetType = targetType;
            permission.AllowDelete = defaultAllowValues;
            permission.AllowNavigate = defaultAllowValues;
            permission.AllowRead = defaultAllowValues;
            permission.AllowWrite = defaultAllowValues;
            permission.AllowCreate = defaultAllowValues;
            action.Invoke(permission);
            return permission;
        }

        public static SecuritySystemTypePermissionObject CreateTypePermission<TObject>(this SecuritySystemRoleBase systemRole) {
            return CreateTypePermission(systemRole, typeof(TObject));
        }

        public static SecuritySystemTypePermissionObject CreateTypePermission(this SecuritySystemRoleBase systemRole, Type targetType) {
            var objectSpace = XPObjectSpace.FindObjectSpaceByObject(systemRole);
            var permissionObject = objectSpace.CreateObject<SecuritySystemTypePermissionObject>();
            permissionObject.TargetType = targetType;
            systemRole.TypePermissions.Add(permissionObject);
            return permissionObject;
        }

        public static SecuritySystemTypePermissionObject CreateTypePermission(this SecuritySystemRoleBase systemRole, Type targetType, Action<SecuritySystemTypePermissionObject> action,
                                                                                                     bool defaultAllowValues = true) {
            var permission = systemRole.CreateTypePermission(targetType);
            permission.TargetType = targetType;
            permission.AllowDelete = defaultAllowValues;
            permission.AllowNavigate = defaultAllowValues;
            permission.AllowRead = defaultAllowValues;
            permission.AllowWrite = defaultAllowValues;
            permission.AllowCreate = defaultAllowValues;
            if (action != null) action.Invoke(permission);
            return permission;
        }

        public static SecuritySystemMemberPermissionsObject CreateMemberPermission(this SecuritySystemTypePermissionObject securitySystemTypePermissionObject, Action<SecuritySystemMemberPermissionsObject> action, bool defaultAllowValues = true) {
            IObjectSpace objectSpace = XPObjectSpace.FindObjectSpaceByObject(securitySystemTypePermissionObject);
            var permission = objectSpace.CreateObject<SecuritySystemMemberPermissionsObject>();
            permission.AllowRead = defaultAllowValues;
            permission.AllowWrite = defaultAllowValues;
            permission.EffectiveRead = defaultAllowValues;
            permission.EffectiveWrite = defaultAllowValues;
            securitySystemTypePermissionObject.MemberPermissions.Add(permission);
            action.Invoke(permission);
            //            var typeInfo = XafTypesInfo.Instance.FindTypeInfo(securitySystemTypePermissionObject.TargetType);
            //            var members = permission.Members.Split(',');
            //            permission.Members = memb
            return permission;
        }

        public static SecuritySystemObjectPermissionsObject CreateObjectPermission(this SecuritySystemTypePermissionObject securitySystemTypePermissionObject, bool defaultAllowValues = true) {
            return CreateObjectPermission(securitySystemTypePermissionObject, null, defaultAllowValues);
        }

        public static SecuritySystemObjectPermissionsObject CreateObjectPermission(this SecuritySystemTypePermissionObject securitySystemTypePermissionObject, Action<SecuritySystemObjectPermissionsObject> action, bool defaultAllowValues = true) {
            var objectSpace = XPObjectSpace.FindObjectSpaceByObject(securitySystemTypePermissionObject);
            var permission = objectSpace.CreateObject<SecuritySystemObjectPermissionsObject>();
            permission.AllowDelete = defaultAllowValues;
            permission.AllowNavigate = defaultAllowValues;
            permission.AllowRead = defaultAllowValues;
            permission.AllowWrite = defaultAllowValues;
            permission.EffectiveDelete = defaultAllowValues;
            permission.EffectiveNavigate = defaultAllowValues;
            permission.EffectiveRead = defaultAllowValues;
            permission.EffectiveWrite = defaultAllowValues;
            securitySystemTypePermissionObject.ObjectPermissions.Add(permission);
            if (action != null) action.Invoke(permission);
            return permission;
        }

        public static void CreatePermissionBehaviour(this SecuritySystemRoleBase systemRole, Enum behaviourEnum, Action<SecuritySystemRoleBase, ITypeInfo> action) {
            var typeInfos = XafTypesInfo.Instance.PersistentTypes.Where(info => {
                var permissionBehaviorAttribute = info.FindAttribute<PermissionBehaviorAttribute>();
                return permissionBehaviorAttribute != null && permissionBehaviorAttribute.Name.Equals(Enum.GetName(behaviourEnum.GetType(), behaviourEnum));
            });
            foreach (var typeInfo in typeInfos) {
                action.Invoke(systemRole, typeInfo);
            }
        }

        public static void CreateFullPermissionAttributes(this SecuritySystemRoleBase systemRole, Action<SecuritySystemTypePermissionObject> action = null, bool defaultAllowValues = true) {
            var persistentTypes = XafTypesInfo.Instance.PersistentTypes.Where(info => info.FindAttribute<FullPermissionAttribute>() != null);
            foreach (var typeInfo in persistentTypes) {
                systemRole.CreateTypePermission(typeInfo.Type, action, defaultAllowValues);
            }
        }
        public static bool IsGranted(IPermission permission, bool isGrantedForNonExistent) {
            var securityComplex = (SecuritySystem.Instance as SecurityBase);
            if (securityComplex != null) {
                bool isGrantedForNonExistentPermission = securityComplex.IsGrantedForNonExistentPermission;
                securityComplex.IsGrantedForNonExistentPermission = isGrantedForNonExistent;
                bool granted = SecuritySystem.IsGranted(permission);
                securityComplex.IsGrantedForNonExistentPermission = isGrantedForNonExistentPermission;
                return granted;
            }
            return SecuritySystem.IsGranted(permission);
        }

        public static List<IOperationPermission> GetPermissions(this ISecurityUserWithRoles securityUserWithRoles) {
            var securityComplex = ((IRoleTypeProvider)SecuritySystem.Instance);
            var permissions = new List<IOperationPermission>();
            foreach (ISecurityRole securityRole in securityUserWithRoles.Roles) {
                if (securityComplex.IsNewSecuritySystem()) {
                    var operationPermissions = ((IOperationPermissionProvider)securityRole).GetPermissions();
                    permissions.AddRange(operationPermissions);
                } else {
                    var operationPermissions = ((IOperationPermissionProvider)securityRole).GetPermissions();
                    permissions.AddRange(operationPermissions);
                }
            }
            return permissions;
        }

        public static bool IsNewSecuritySystem(this IRoleTypeProvider security) {
            return typeof(IPermissionMatrixTypePermissionsOwner).IsAssignableFrom(security.RoleType);
        }

        public static bool IsGranted(this IRole role, IPermission permission) {
            var permissionSet = new PermissionSet(PermissionState.None);
            role.Permissions.Each(perm => permissionSet.AddPermission(perm));
            var getPermission = permissionSet.GetPermission(typeof(ObjectAccessPermission));
            return getPermission != null && permission.IsSubsetOf(getPermission);
        }
    }
}
