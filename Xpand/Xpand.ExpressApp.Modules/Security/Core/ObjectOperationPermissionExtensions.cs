using System;
using System.Collections.Generic;
using DevExpress.Data.Filtering;
using DevExpress.ExpressApp.Security;
using DevExpress.ExpressApp.Security.Strategy;
using DevExpress.ExpressApp.Xpo;
using DevExpress.Xpo;
using DevExpress.Xpo.Metadata;
using System.Linq;

namespace Xpand.ExpressApp.Security.Core {
    public interface IObjectReadOperationPermission : IObjectOperationPermission {
    }
    public interface IObjectWriteOperationPermission : IObjectOperationPermission {
    }
    public interface IObjectCreateOperationPermission : IObjectOperationPermission {
    }
    public interface IObjectDeleteOperationPermission : IObjectOperationPermission {
    }
    public interface IObjectNavigateOperationPermission : IObjectOperationPermission {
    }

    public interface IObjectOperationPermission {
    }

    public static class ObjectOperationPermissionExtensions {
        static readonly XPDictionary _dictionary;
        static ObjectOperationPermissionExtensions() {
            _dictionary = XpoTypesInfoHelper.GetXpoTypeInfoSource().XPDictionary;
        }

        public static IEnumerable<ObjectOperationPermission> ObjectOperationPermissions(this XPMemberInfo member, ISecuritySystemRole role) {
            var collection = (XPBaseCollection)member.GetValue(role);
            var securityOperations = GetSecurityOperations(member.CollectionElementType.ClassType);
            return securityOperations.SelectMany(securityOperation => collection.Cast<IOperationPermission>(), (securityOperation, operationPermission) =>
                    CreatePermission(member, (IObjectOperationPermission)operationPermission, securityOperation));
        }

        static ObjectOperationPermission CreatePermission(XPMemberInfo member, IObjectOperationPermission operationPermission, string operation) {
            return new ObjectOperationPermission(member.CollectionElementType.ClassType, Criteria(operationPermission), operation);
        }

        static string Criteria(IObjectOperationPermission operationPermission) {
            var keyProperty = _dictionary.GetClassInfo(operationPermission).KeyProperty;
            var keyValue = keyProperty.GetValue(operationPermission);
            return CriteriaOperator.Parse(keyProperty.Name + "=?", keyValue).ToString();
        }

        static IEnumerable<string> GetSecurityOperations(Type classType) {
            if (typeof(IObjectReadOperationPermission).IsAssignableFrom(classType))
                yield return SecurityOperations.Read;
            if (typeof(IObjectNavigateOperationPermission).IsAssignableFrom(classType))
                yield return SecurityOperations.Navigate;
            if (typeof(IObjectCreateOperationPermission).IsAssignableFrom(classType))
                yield return SecurityOperations.Create;
            if (typeof(IObjectDeleteOperationPermission).IsAssignableFrom(classType))
                yield return SecurityOperations.Delete;
            if (typeof(IObjectWriteOperationPermission).IsAssignableFrom(classType))
                yield return SecurityOperations.Write;
        }
    }
}