using System;
using System.Linq;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Security;

namespace eXpand.ExpressApp.MemberLevelSecurity {
    public class MemberLevelObjectAccessComparer : ObjectAccessComparer {
        public override bool IsMemberReadGranted(Type requestedType, string propertyName,
                                                 SecurityContextList securityContexts) {
            ITypeInfo typeInfo = XafTypesInfo.Instance.FindTypeInfo(requestedType);
            IMemberInfo memberInfo = typeInfo.FindMember(propertyName);
            if (memberInfo.GetPath().Any(currentMemberInfo =>!SecuritySystem.IsGranted(new MemberAccessPermission(
                                                                             currentMemberInfo.Owner.Type,
                                                                             currentMemberInfo.Name,
                                                                             MemberOperation.Read)))) {
                return false;
            }
            return base.IsMemberReadGranted(requestedType, propertyName, securityContexts);
        }

        public override bool IsMemberModificationDenied(object targetObject, IMemberInfo memberInfo) {
            if (memberInfo.GetPath().Any(currentMemberInfo =>!SecuritySystem.IsGranted(new MemberAccessPermission(
                                                                             currentMemberInfo.Owner.Type,
                                                                             currentMemberInfo.Name,
                                                                             MemberOperation.Write)))) {
                return true;
            }
            return base.IsMemberModificationDenied(targetObject, memberInfo);
        }
    }
}