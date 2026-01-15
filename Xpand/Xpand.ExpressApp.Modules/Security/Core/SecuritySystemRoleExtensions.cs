using System;
using System.Collections.Generic;
using System.Linq;
using DevExpress.Data.Filtering;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Security;
using DevExpress.Persistent.Base;
using DevExpress.Xpo;
using DevExpress.Xpo.Helpers;
using DevExpress.Xpo.Metadata;
using Xpand.ExpressApp.Security.Permissions;
using Xpand.Persistent.Base.Security;

namespace Xpand.ExpressApp.Security.Core {

    public enum SecurityOperationsEnum {
        Read,
        Write,
        Create,
        Delete,
        Navigate,
        FullObjectAccess,
        FullAccess,
        ReadOnlyAccess,
        CRUDAccess,
        ReadWriteAccess

    }
    public static class SecuritySystemRoleExtensions {
        static IEnumerable<XPMemberInfo> OperationPermissionCollectionMembers(this IXpandRoleCustomPermissions xpandRoleCustomPermissions) {
            var classInfo = ((IXPClassInfoProvider)xpandRoleCustomPermissions).ClassInfo;
            return classInfo.OwnMembers.Where(info => info.IsAssociationList && info.CollectionElementType.HasAttribute(typeof(SecurityOperationsAttribute)));
        }

        public static IEnumerable<NavigationItemPermission> GetHiddenNavigationItemPermissions(this ISupportHiddenNavigationItems supportHiddenNavigationItems) {
            return !string.IsNullOrEmpty(supportHiddenNavigationItems.HiddenNavigationItems)
                ? supportHiddenNavigationItems.HiddenNavigationItems.Split(';', ',')
                    .Select(s => new NavigationItemPermission(s.Trim()))
                : Enumerable.Empty<NavigationItemPermission>();
        }

        public static IEnumerable<IOperationPermission> GetCustomPermissions(this IXpandRoleCustomPermissions xpandRoleCustomPermissions) {
            var operationPermissions = xpandRoleCustomPermissions.Permissions.SelectMany(data => data.GetPermissions());
            var permissions = operationPermissions.Union(PermissionProviderStorage.Instance.SelectMany(info => info.GetPermissions(xpandRoleCustomPermissions)));
            return xpandRoleCustomPermissions.SecurityOperationAttributePermissions().Concat(permissions);
        }

        public static IEnumerable<IOperationPermission> SecurityOperationAttributePermissions(this IXpandRoleCustomPermissions xpandRoleCustomPermissions){
            var operationPermissionCollectionMembers = xpandRoleCustomPermissions.OperationPermissionCollectionMembers();
            var operationPermissions = new List<IOperationPermission>();
            foreach (var operationPermissionCollectionMember in operationPermissionCollectionMembers){
                operationPermissions.AddRange(xpandRoleCustomPermissions.ObjectOperationPermissions(operationPermissionCollectionMember));
            }
            return operationPermissions.Distinct();
        }

        static IEnumerable<IOperationPermission> ObjectOperationPermissions(this ISecurityRole securityRole, XPMemberInfo member) {
            var collection = ((XPBaseCollection)member.GetValue(securityRole)).OfType<object>().ToArray();
            var securityOperation = GetSecurityOperation(securityRole, member);
            return Enumerable.Empty<IOperationPermission>();
            // throw new NotImplementedException("25.2 BC");
            // if (!string.IsNullOrEmpty(securityOperation)) {
            //     
            //     // foreach (var operation in securityOperation.Split(ServerPermissionRequestProcessor.Delimiters, StringSplitOptions.RemoveEmptyEntries)) {
            //     //     foreach (var obj in collection) {
            //     //         yield return ObjectOperationPermissions(member, obj, operation, securityRole.Name);
            //     //     }
            //     // }
            // }
        }

        static IOperationPermission ObjectOperationPermissions(XPMemberInfo member, object obj, string securityOperation, string roleName){
            var permissionPolicyUser = SecuritySystem.CurrentUser as IPermissionPolicyUser;
            if (permissionPolicyUser == null){
                return new ObjectOperationPermission(member.CollectionElementType.ClassType,
                    Criteria(obj, member.CollectionElementType), securityOperation);
            }
            return new ObjectOperationStatePermission(member.CollectionElementType.ClassType,
                Criteria(obj, member.CollectionElementType), securityOperation, roleName, true);
        }

        static string Criteria(object obj, XPClassInfo classInfo) {
            var keyProperty = classInfo.KeyProperty;
            var keyValue = keyProperty.GetValue(obj);
            return CriteriaOperator.Parse(keyProperty.Name + "=?", keyValue).ToString();
        }

        static string GetSecurityOperation(ISecurityRole securityRole, XPMemberInfo memberInfo) {
            var typeInfo = XafTypesInfo.Instance.FindTypeInfo(memberInfo.CollectionElementType.ClassType);
            var roleTypeInfo = XafTypesInfo.Instance.FindTypeInfo(securityRole.GetType());
            var operationsAttribute = typeInfo.FindAttributes<SecurityOperationsAttribute>().FirstOrDefault(attribute => attribute.CollectionName == memberInfo.Name);
            return operationsAttribute != null ? Convert(securityRole, roleTypeInfo, operationsAttribute) : null;
        }

        static string Convert(ISecurityRole securityRole, ITypeInfo roleTypeInfo, SecurityOperationsAttribute operationsAttribute) {
            var memberInfo = MemberInfo(roleTypeInfo, operationsAttribute);
            if (memberInfo != null) {
                var value = memberInfo.GetValue(securityRole);
                if (value == null || ReferenceEquals(value, ""))
                    return null;
                var securityOperations = (SecurityOperationsEnum)value;
                var fieldInfo = typeof (SecurityOperations).GetField(securityOperations.ToString());
                if (fieldInfo != null) 
                    return fieldInfo.GetValue(null).ToString();
                throw new NotImplementedException(value.ToString());
            }
            return null;
        }

        static IMemberInfo MemberInfo(ITypeInfo roleTypeInfo, SecurityOperationsAttribute operationsAttribute) {
            return roleTypeInfo.FindMember(operationsAttribute.OperationProviderProperty);
        }
    }
}