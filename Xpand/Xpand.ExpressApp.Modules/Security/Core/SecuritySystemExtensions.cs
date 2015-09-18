﻿﻿using System;
﻿using System.Collections.Generic;
﻿using System.Linq;
﻿using System.Security;
﻿using System.Security.Permissions;
﻿using DevExpress.Data.Filtering;
﻿using DevExpress.ExpressApp;
﻿using DevExpress.ExpressApp.DC;
﻿using DevExpress.ExpressApp.Security;
﻿using DevExpress.ExpressApp.Security.Strategy;
﻿using DevExpress.ExpressApp.Xpo;
﻿using DevExpress.Persistent.Base.Security;
﻿using DevExpress.Xpo;
﻿using Fasterflect;
﻿using Xpand.ExpressApp.Security.AuthenticationProviders;
﻿using Xpand.ExpressApp.Security.Permissions;
﻿using Xpand.Persistent.Base.General;
﻿using Xpand.Utils.Helpers;
﻿using IOperationPermissionProvider = DevExpress.ExpressApp.Security.IOperationPermissionProvider;

namespace Xpand.ExpressApp.Security.Core {
    [AttributeUsage(AttributeTargets.Class)]
    public class FullPermissionAttribute : Attribute {
    }
    [AttributeUsage(AttributeTargets.Class)]
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

        public static void NewSecurityStrategyComplex<TAuthentation, TLogonParameter>(this XafApplication application, Type userType = null, Type roleType = null)
            where TAuthentation : AuthenticationBase {
                application.NewSecurityStrategyComplex(typeof(TAuthentation), typeof(TLogonParameter), userType ?? typeof(XpandUser),roleType??typeof(XpandRole));
        }

        public static void NewSecurityStrategyComplex(this XafApplication application,Type authethicationType=null, Type logonParametersType=null,Type userType=null,Type roleType=null){
            logonParametersType = logonParametersType ?? typeof(XpandLogonParameters);
            userType = userType??typeof(XpandUser);
            AuthenticationStandard authenticationStandard = new XpandAuthenticationStandard(userType, logonParametersType);
            if(authethicationType!=null){
                authenticationStandard = (AuthenticationStandard)authethicationType.CreateInstance();
                authenticationStandard.UserType = userType;
                authenticationStandard.LogonParametersType = logonParametersType;
            }
            var security = new SecurityStrategyComplex(userType, roleType??typeof(XpandRole), authenticationStandard);
            application.Security=security;
        }

        public static SecuritySystemRoleBase GetDefaultRole(this IObjectSpace objectSpace, string roleName) {
            var defaultRole = objectSpace.GetRole(roleName);
            if (objectSpace.IsNewObject(defaultRole)) {
                defaultRole.AddObjectAccessPermission(SecuritySystem.UserType, "[Oid] = CurrentUserId()", SecurityOperations.ReadOnlyAccess);
                defaultRole.AddMemberAccessPermission(SecuritySystem.UserType, "ChangePasswordOnFirstLogon,StoredPassword", SecurityOperations.Write, "[Oid] = CurrentUserId()");
                defaultRole.GrandObjectAccessRecursively();
            }
            return defaultRole;
        }

        public static void GrandObjectAccessRecursively(this SecuritySystemRoleBase defaultRole) {
            Type roleType=defaultRole.GetType();
            foreach (Type type in SecurityStrategy.GetSecuredTypes().Where(type => roleType == type || type.IsAssignableFrom(roleType))) {
                defaultRole.AddObjectAccessPermission(type, "[Name]='" + defaultRole.Name + "'", SecurityOperations.ReadOnlyAccess);
            }
        }

        public static SecuritySystemRoleBase GetDefaultRole(this IObjectSpace objectSpace) {
            return objectSpace.GetDefaultRole("Default");
        }

        public static ISecurityUserWithRoles GetAnonymousUser(this XpandRole systemRole) {
            var optionsAthentication = ((IModelOptionsAuthentication)ApplicationHelper.Instance.Application.Model.Options).Athentication;
            var anonymousUserName = optionsAthentication.AnonymousAuthentication.AnonymousUser;
            return GetAnonymousUser(systemRole, anonymousUserName);
        }

        public static ISecurityUserWithRoles GetAnonymousUser(this XpandRole systemRole, string userName) {
            return systemRole.GetUser(userName);
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
            user2.CallMethod("SetPassword",new[]{typeof(string)}, new object[]{passWord});
            var roleCollection = (XPBaseCollection)typeInfo.FindMember("Roles").GetValue(user2);
            foreach (var role in roles) {
                roleCollection.BaseAdd(role);
            }
            return user2;
        }

        public static SecuritySystemRoleBase GetAdminRole(this IObjectSpace objectSpace, string roleName) {
            var roleType = ((IRoleTypeProvider)SecuritySystem.Instance).RoleType;
            var administratorRole = (SecuritySystemRoleBase)objectSpace.FindObject(roleType, new BinaryOperator("Name", roleName));
            if (administratorRole == null) {
                administratorRole = (SecuritySystemRoleBase)objectSpace.CreateObject(roleType);
                administratorRole.Name = roleName;
                administratorRole.IsAdministrative = true;
            }
            return administratorRole;
        }

        public static XpandRole GetAnonymousRole(this IObjectSpace objectSpace, string roleName, bool selfReadOnlyPermissions = true) {
            var anonymousRole = (XpandRole) objectSpace.GetRole(roleName);
            anonymousRole.Permissions.AddRange(new[]{
                objectSpace.CreateModifierPermission<MyDetailsOperationPermissionData>(Modifier.Allow),
                objectSpace.CreateModifierPermission<AnonymousLoginOperationPermissionData>(Modifier.Allow)
            });
            return  anonymousRole;
        }


        public static XpandPermissionData CreateModifierPermission<T>(this IObjectSpace objectSpace, Modifier modifier) where T : ModifierPermissionData {
            var operationPermissionData = objectSpace.CreateObject<T>();
            operationPermissionData.Modifier = modifier;
            return operationPermissionData;
        }

        public static SecuritySystemRoleBase GetRole(this IObjectSpace objectSpace, string roleName,bool selfReadOnlyPermissions=true) {
            var roleType = ((IRoleTypeProvider)SecuritySystem.Instance).RoleType;
            var securityDemoRole = (SecuritySystemRoleBase)objectSpace.FindObject(roleType, new BinaryOperator("Name", roleName));
            if (securityDemoRole == null) {
                securityDemoRole = (SecuritySystemRoleBase)objectSpace.CreateObject(roleType);
                securityDemoRole.Name = roleName;
                if (selfReadOnlyPermissions) {
                    securityDemoRole.GrandObjectAccessRecursively();
                }
            }
            return securityDemoRole;
        }

        [Obsolete("Use AddNewTypePermission<TObject>() instead (does same thing, only renamed)")]
        public static SecuritySystemTypePermissionObject CreateTypePermission<TObject>(this SecuritySystemRoleBase role, Action<SecuritySystemTypePermissionObject> action, bool defaultAllowValues = true) {
            return AddNewTypePermission<TObject>(role, action, defaultAllowValues);
        }

        [Obsolete("Use AddNewTypePermission<TObject>() instead (does same thing, only renamed)")]
        public static SecuritySystemTypePermissionObject CreateTypePermission<TObject>(this SecuritySystemRoleBase role){
            return AddNewTypePermission(role, typeof(TObject));
        }

        [Obsolete("Use AddNewTypePermission() instead (does same thing, only renamed)")]
        public static SecuritySystemTypePermissionObject CreateTypePermission(this SecuritySystemRoleBase role, Type targetType){
            return AddNewTypePermission(role, targetType);
        }

        [Obsolete("Use AddNewTypePermission() instead (does same thing, only renamed)")]
        public static SecuritySystemTypePermissionObject CreateTypePermission(this SecuritySystemRoleBase systemRole, Type targetType, Action<SecuritySystemTypePermissionObject> action,
                                                                                             bool defaultAllowValues = true){
            return AddNewTypePermission(systemRole, targetType, action, defaultAllowValues);
        }

        public static SecuritySystemTypePermissionObject AddNewTypePermission<TObject>(this SecuritySystemRoleBase role, Action<SecuritySystemTypePermissionObject> action, bool defaultAllowValues = true){
            return AddNewTypePermission(role, typeof(TObject), action, defaultAllowValues);
        }

        public static SecuritySystemTypePermissionObject AddNewTypePermission<TObject>(this SecuritySystemRoleBase role){
            return AddNewTypePermission(role, typeof(TObject));
        }

        public static SecuritySystemTypePermissionObject AddNewTypePermission(this SecuritySystemRoleBase role, Type targetType){
            var objectSpace = XPObjectSpace.FindObjectSpaceByObject(role);
            var permissionObject = objectSpace.CreateObject<SecuritySystemTypePermissionObject>();
            permissionObject.TargetType = targetType;
            role.TypePermissions.Add(permissionObject);
            return permissionObject;
        }

        public static SecuritySystemTypePermissionObject AddNewTypePermission(this SecuritySystemRoleBase role, Type targetType, Action<SecuritySystemTypePermissionObject> action,
                                                                                                     bool defaultAllowValues = true) {
            var permission = AddNewTypePermission(role, targetType);
            permission.AllowDelete = defaultAllowValues;
            permission.AllowNavigate = defaultAllowValues;
            permission.AllowRead = defaultAllowValues;
            permission.AllowWrite = defaultAllowValues;
            permission.AllowCreate = defaultAllowValues;
            if (action != null) action.Invoke(permission);
            return permission;
        }

        [Obsolete("Use AddNewMemberPermission() instead (does same thing, only renamed)")]
        public static SecuritySystemMemberPermissionsObject CreateMemberPermission(this SecuritySystemTypePermissionObject securitySystemTypePermissionObject, Action<SecuritySystemMemberPermissionsObject> action, bool defaultAllowValues = true) {
            return AddNewMemberPermission(securitySystemTypePermissionObject, action, defaultAllowValues);
        }

        public static SecuritySystemMemberPermissionsObject AddNewMemberPermission(this SecuritySystemTypePermissionObject securitySystemTypePermissionObject, Action<SecuritySystemMemberPermissionsObject> action, bool defaultAllowValues = true)
        {
            IObjectSpace objectSpace = XPObjectSpace.FindObjectSpaceByObject(securitySystemTypePermissionObject);
            var permission = objectSpace.CreateObject<SecuritySystemMemberPermissionsObject>();
            permission.AllowRead = defaultAllowValues;
            permission.AllowWrite = defaultAllowValues;
            permission.EffectiveRead = defaultAllowValues;
            permission.EffectiveWrite = defaultAllowValues;
            securitySystemTypePermissionObject.MemberPermissions.Add(permission);
            action.Invoke(permission);
            return permission;
        }

        [Obsolete("Use AddNewObjectPermission() instead (does same thing, only renamed)")]
        public static SecuritySystemObjectPermissionsObject CreateObjectPermission(this SecuritySystemTypePermissionObject securitySystemTypePermissionObject, bool defaultAllowValues = true) {
            return AddNewObjectPermission(securitySystemTypePermissionObject, defaultAllowValues);   
        }

        public static SecuritySystemObjectPermissionsObject AddNewObjectPermission(this SecuritySystemTypePermissionObject securitySystemTypePermissionObject, bool defaultAllowValues = true) {
            return AddNewObjectPermission(securitySystemTypePermissionObject, null, defaultAllowValues);
        }

        [Obsolete("Use AddNewObjectPermission() instead (does same thing, only renamed)")]
        public static SecuritySystemObjectPermissionsObject CreateObjectPermission(this SecuritySystemTypePermissionObject securitySystemTypePermissionObject, Action<SecuritySystemObjectPermissionsObject> action, bool defaultAllowValues = true) {
            return AddNewObjectPermission(securitySystemTypePermissionObject, action, defaultAllowValues);
        }

        public static SecuritySystemObjectPermissionsObject AddNewObjectPermission(this SecuritySystemTypePermissionObject securitySystemTypePermissionObject, Action<SecuritySystemObjectPermissionsObject> action, bool defaultAllowValues = true) {
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

        [Obsolete("Use AddNewFullPermissionAttributes() instead (does same thing, only renamed)")]
        public static void CreateFullPermissionAttributes(this SecuritySystemRoleBase systemRole, Action<SecuritySystemTypePermissionObject> action = null, bool defaultAllowValues = true) {
            AddNewFullPermissionAttributes(systemRole, action, defaultAllowValues);
        }

        public static void AddNewFullPermissionAttributes(this SecuritySystemRoleBase systemRole, Action<SecuritySystemTypePermissionObject> action = null, bool defaultAllowValues = true) {
            var persistentTypes = XafTypesInfo.Instance.PersistentTypes.Where(info => info.FindAttribute<FullPermissionAttribute>() != null);
            foreach (var typeInfo in persistentTypes) {
                systemRole.AddNewTypePermission(typeInfo.Type, action, defaultAllowValues);
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
            return security is SecurityStrategyComplex;
        }

        public static bool IsGranted(this IRole role, IPermission permission) {
            var permissionSet = new PermissionSet(PermissionState.None);
            role.Permissions.Each(perm => permissionSet.AddPermission(perm));
            var getPermission = permissionSet.GetPermission(typeof(ObjectAccessPermission));
            return getPermission != null && permission.IsSubsetOf(getPermission);
        }
    }
}
