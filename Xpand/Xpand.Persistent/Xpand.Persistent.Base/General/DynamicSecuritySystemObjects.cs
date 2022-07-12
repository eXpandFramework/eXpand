using System;
using System.Collections.Generic;
using System.Linq;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.DC.Xpo;
using DevExpress.ExpressApp.Security;
using DevExpress.ExpressApp.Xpo;
using DevExpress.Persistent.Base;
using DevExpress.Xpo;
using DevExpress.Xpo.Metadata;
using Xpand.Extensions.TypeExtensions;

namespace Xpand.Persistent.Base.General {
    public class DynamicSecuritySystemObjects {
        readonly XafApplication _application;

        public DynamicSecuritySystemObjects(XafApplication application) {
            _application = application;
        }

        public List<XPMemberInfo> BuildRole(Type otherPartMember) {
            return BuildRole(otherPartMember, otherPartMember.Name + otherPartMember.Name + "s", otherPartMember.Name + "s", "Roles", false);
        }

        public List<XPMemberInfo> BuildRole(Type otherPartMember, string association, string propertyName, string otherPartPropertyName, bool visibleInDetailView = true) {
            var xpCustomMemberInfos = new List<XPMemberInfo>();
            if (_application.Security is IRoleTypeProvider { RoleType:{ } } securityComplex) {
                var typeInfo = XafTypesInfo.Instance.FindTypeInfo(XpandModuleBase.RoleType);
                var typeToCreateOn = securityComplex.RoleType.IsInterface ? XpandModuleBase.RoleType : typeInfo.Type;
                if (IsValidType(typeToCreateOn)) {
                    xpCustomMemberInfos = XafTypesInfo.Instance.CreateBothPartMembers(typeToCreateOn, otherPartMember,  true, GetRoleAssociation(association, typeInfo), propertyName, otherPartPropertyName);
                    XafTypesInfo.Instance.RefreshInfo(typeToCreateOn);
                }
            }
            return xpCustomMemberInfos;
        }

        private static string GetRoleAssociation(string association, ITypeInfo typeInfo){
            return typeof(IPermissionPolicyRole).IsAssignableFrom(typeInfo.Type)
                ? typeInfo.FullName.Replace(".", "_") + association
                : association;
        }

        bool IsValidType(Type typeToCreateOn){
            var isValidDataLayer = IsValidDataLayer();
            return isValidDataLayer && IsXpoType(typeToCreateOn);
        }

        private bool IsValidDataLayer(){
            if (_application.ObjectSpaceProviders.Any(provider => provider.GetType().InheritsFrom("DevExpress.ExpressApp.Security.ClientServer.DataServerObjectSpaceProvider")))
                return true;
            var isValidDataLayer =_application.ObjectSpaceProviders.OfType<XPObjectSpaceProvider>()
                    .Any(provider => !(provider.DataLayer is ThreadSafeDataLayer));
            return isValidDataLayer;
        }

        bool IsXpoType(Type typeToCreateOn) {
            XpoTypeInfoSource xpoTypeInfoSource = XpoTypesInfoHelper.GetXpoTypeInfoSource();
            return xpoTypeInfoSource.GetOriginalType(typeToCreateOn) != null &&
                   xpoTypeInfoSource.RegisteredEntities.Contains(typeToCreateOn);
        }

        public List<XPMemberInfo> BuildUser(Type otherPartMember) {
            return BuildUser(otherPartMember, "User" + otherPartMember.Name + otherPartMember.Name + "s", otherPartMember.Name + "s", "Users");
        }

        public List<XPMemberInfo> BuildUser(Type otherPartMember, string association, string propertyName, string otherPartPropertyName) {
            var xpCustomMemberInfos = new List<XPMemberInfo>();
            if (_application.Security != null){
                var userType = XpandModuleBase.UserType;
                if (userType != null) {
                    if (IsValidType(userType)) {
                        xpCustomMemberInfos = XafTypesInfo.Instance.CreateBothPartMembers(userType, otherPartMember,  true, GetUserAssociation(association, userType), propertyName, otherPartPropertyName);
                        XafTypesInfo.Instance.RefreshInfo(userType);
                    }
                }
            }
            return xpCustomMemberInfos;
        }

        private static string GetUserAssociation(string association, Type userType){
            return typeof(IPermissionPolicyUser).IsAssignableFrom(userType)
                ? userType.FullName?.Replace(".", "_") + association
                : association;
        }

        public void HideInDetailView(List<XPMemberInfo> xpMemberInfos, string name) {
            var xpMemberInfo = xpMemberInfos.SingleOrDefault(info => info.Name == name);
            if (xpMemberInfo != null)
                HideMemberInDetailView(xpMemberInfo);
        }

        void HideMemberInDetailView(XPMemberInfo xpMemberInfo) {
            if (!xpMemberInfo.HasAttribute(typeof(VisibleInDetailViewAttribute)))
                xpMemberInfo.AddAttribute(new VisibleInDetailViewAttribute(false));
        }

        public void HideRoleInDetailView(List<XPMemberInfo> xpMemberInfos) {
            var xpMemberInfo = xpMemberInfos.SingleOrDefault(info => info.CollectionElementType.ClassType == XpandModuleBase.RoleType);
            if (xpMemberInfo != null) HideMemberInDetailView(xpMemberInfo);
        }
    }
}