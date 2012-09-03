using System;
using System.Linq;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Security;
using DevExpress.Persistent.Base;
using Xpand.Persistent.Base.General;

namespace Xpand.ExpressApp {
    public class DynamicSecuritySystemObjects {
        readonly XafApplication _application;

        public DynamicSecuritySystemObjects(XafApplication application) {
            _application = application;
        }

        public void BuildRole(Type otherPartMember, string association, string propertyName, string otherPartPropertyName) {
            var securityComplex = _application.Security as IRoleTypeProvider;
            if (securityComplex != null) {
                var typeInfo = XafTypesInfo.Instance.FindTypeInfo(XpandModuleBase.RoleType);
                var typeToCreateOn = typeInfo.Type;
                var xpCustomMemberInfos = XafTypesInfo.Instance.CreateBothPartMembers(typeToCreateOn, otherPartMember, XpandModuleBase.Dictiorary, true, association, propertyName, otherPartPropertyName);
                xpCustomMemberInfos.First(info => info.Name == propertyName).AddAttribute(new VisibleInDetailViewAttribute(false));
                XafTypesInfo.Instance.RefreshInfo(typeToCreateOn);
            }
        }

        public void BuildUser(Type otherPartMember, string association, string propertyName, string otherPartPropertyName) {
            if (_application.Security != null) {
                if (XpandModuleBase.UserType != null) {
                    var xpCustomMemberInfos = XafTypesInfo.Instance.CreateBothPartMembers(XpandModuleBase.UserType, otherPartMember, XpandModuleBase.Dictiorary, true, association, propertyName, otherPartPropertyName);
                    xpCustomMemberInfos.First(info => info.Name == propertyName).AddAttribute(new VisibleInDetailViewAttribute(false));
                    XafTypesInfo.Instance.RefreshInfo(XpandModuleBase.UserType);
                }
            }
        }
    }
}