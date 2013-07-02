using System;
using System.Collections.Generic;
using System.Linq;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.DC.Xpo;
using DevExpress.ExpressApp.Security;
using DevExpress.ExpressApp.Xpo;
using DevExpress.Persistent.Base;
using DevExpress.Xpo;
using DevExpress.Xpo.Metadata;
using Xpand.Persistent.Base.General;

namespace Xpand.ExpressApp {
    public class DynamicSecuritySystemObjects {
        readonly XafApplication _application;

        public DynamicSecuritySystemObjects(XafApplication application) {
            _application = application;
        }

        public List<XPMemberInfo> BuildRole(Type otherPartMember) {
            return BuildRole(otherPartMember, "Role" + otherPartMember.Name + otherPartMember.Name + "s", otherPartMember.Name + "s", "Roles", false);
        }

        public List<XPMemberInfo> BuildRole(Type otherPartMember, string association, string propertyName, string otherPartPropertyName, bool visibleInDetailView = true) {
            var xpCustomMemberInfos = new List<XPMemberInfo>();
            var securityComplex = _application.Security as IRoleTypeProvider;
            if (securityComplex != null) {
                var typeInfo = XafTypesInfo.Instance.FindTypeInfo(XpandModuleBase.RoleType);
                var typeToCreateOn = securityComplex.RoleType.IsInterface ? XpandModuleBase.RoleType : typeInfo.Type;
                if (IsValidType(typeToCreateOn)) {
                    xpCustomMemberInfos = XafTypesInfo.Instance.CreateBothPartMembers(typeToCreateOn, otherPartMember, XpandModuleBase.Dictiorary, true, association, propertyName, otherPartPropertyName);
                    XafTypesInfo.Instance.RefreshInfo(typeToCreateOn);
                }
            }
            return xpCustomMemberInfos;
        }

        bool IsValidType(Type typeToCreateOn) {
            var isValidDataLayer = _application.ObjectSpaceProviders.OfType<XPObjectSpaceProvider>().Any(provider => !(provider.DataLayer is ThreadSafeDataLayer));
            return isValidDataLayer && IsXpoType(typeToCreateOn);
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
            if (_application.Security != null) {
                if (XpandModuleBase.UserType != null) {
                    if (IsValidType(XpandModuleBase.UserType)) {
                        xpCustomMemberInfos = XafTypesInfo.Instance.CreateBothPartMembers(XpandModuleBase.UserType, otherPartMember, XpandModuleBase.Dictiorary, true, association, propertyName, otherPartPropertyName);
                        XafTypesInfo.Instance.RefreshInfo(XpandModuleBase.UserType);
                    }
                }
            }
            return xpCustomMemberInfos;
        }

        public void HideInDetailView(List<XPMemberInfo> xpMemberInfos, string name) {
            var xpMemberInfo = xpMemberInfos.SingleOrDefault(info => info.Name == name);
            if (xpMemberInfo != null)
                HideMemberInDetailView(xpMemberInfo);
        }

        void HideMemberInDetailView(XPMemberInfo xpMemberInfo) {
            xpMemberInfo.AddAttribute(new VisibleInDetailViewAttribute(false));
        }

        public void HideRoleInDetailView(List<XPMemberInfo> xpMemberInfos) {
            var xpMemberInfo = xpMemberInfos.SingleOrDefault(info => info.CollectionElementType.ClassType == XpandModuleBase.RoleType);
            if (xpMemberInfo != null) HideMemberInDetailView(xpMemberInfo);
        }
    }
}