using System;
using System.Linq;
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

            if (memberInfo.GetPath().Any(currentMemberInfo => !SecuritySystemExtensions.IsGranted(new MemberAccessPermission(currentMemberInfo.Owner.Type,
                                                                             currentMemberInfo.Name,
                                                                             MemberOperation.Write), true))){
                return true;
            }
            var securityComplex = ((SecurityBase)SecuritySystem.Instance);
            bool isGrantedForNonExistentPermission = securityComplex.IsGrantedForNonExistentPermission;
            securityComplex.IsGrantedForNonExistentPermission = true;
            bool isMemberModificationDenied = base.IsMemberModificationDenied(targetObject, memberInfo);
            securityComplex.IsGrantedForNonExistentPermission = isGrantedForNonExistentPermission;
            return isMemberModificationDenied;
        }
    }
}