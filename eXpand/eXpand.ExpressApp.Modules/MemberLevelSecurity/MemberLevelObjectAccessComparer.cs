using System;
using System.Linq;
using DevExpress.Data.Filtering;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Security;
using eXpand.ExpressApp.Security.Core;

namespace eXpand.ExpressApp.MemberLevelSecurity {
    public class MemberLevelObjectAccessComparer : ObjectAccessComparer {
        public override bool IsMemberReadGranted(Type requestedType, string propertyName,
                                                 SecurityContextList securityContexts) {
            ITypeInfo typeInfo = XafTypesInfo.Instance.FindTypeInfo(requestedType);
            IMemberInfo memberInfo = typeInfo.FindMember(propertyName);
            if (memberInfo.GetPath().Any(currentMemberInfo =>!SecuritySystemExtensions.IsGranted(new MemberAccessPermission(
                                                                             currentMemberInfo.Owner.Type,
                                                                             currentMemberInfo.Name,
                                                                             MemberOperation.Read),true))) {
                return false;
            }
            var securityComplex = ((SecurityBase)SecuritySystem.Instance);
            bool isGrantedForNonExistentPermission = securityComplex.IsGrantedForNonExistentPermission;
            securityComplex.IsGrantedForNonExistentPermission = true;
            bool isMemberReadGranted = base.IsMemberReadGranted(requestedType, propertyName, securityContexts);
            securityComplex.IsGrantedForNonExistentPermission = isGrantedForNonExistentPermission;
            return isMemberReadGranted;
        }

        public override bool IsMemberModificationDenied(object targetObject, IMemberInfo memberInfo) {
            bool firstOrDefault =memberInfo.GetPath().Select(info =>!SecuritySystemExtensions.IsGranted(
                        new MemberAccessPermission(info.Owner.Type, info.Name, MemberOperation.Write), true)).Where(b => b).FirstOrDefault();
            if (firstOrDefault) {
                return Fit(targetObject);
            }
            var securityComplex = ((SecurityBase)SecuritySystem.Instance);
            bool isGrantedForNonExistentPermission = securityComplex.IsGrantedForNonExistentPermission;
            securityComplex.IsGrantedForNonExistentPermission = true;
            bool isMemberModificationDenied = base.IsMemberModificationDenied(targetObject, memberInfo);
            securityComplex.IsGrantedForNonExistentPermission = isGrantedForNonExistentPermission;
            return isMemberModificationDenied;
        }

        public bool Fit(object currentObject) {
            var memberAccessPermission = ((SecurityBase)SecuritySystem.Instance).PermissionSet.GetPermission(typeof(MemberAccessPermission)) as MemberAccessPermission;
            if (memberAccessPermission != null && memberAccessPermission.IsSubsetOf(memberAccessPermission))
            {
                ObjectSpace objectSpace = ObjectSpace.FindObjectSpace(currentObject);
                var criteriaOperator = CriteriaOperator.Parse(memberAccessPermission.Criteria);
                return objectSpace.GetExpressionEvaluator(currentObject.GetType(), criteriaOperator).Fit(currentObject);
            }
            return true;
        }
    }
}