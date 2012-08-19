using System;
using System.Linq;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Security;
using DevExpress.Persistent.Base;
using Xpand.ExpressApp.JobScheduler.Jobs.SendEmail;
using Xpand.Persistent.Base.General;

namespace Xpand.ExpressApp.JobScheduler.Jobs {
    public class JobSchedulerJobsModule : XpandModuleBase {
        public JobSchedulerJobsModule() {
            RequiredModuleTypes.Add(typeof(JobSchedulerModule));
        }
        private static ITypeInfo GetRoleTypeInfo(IRoleTypeProvider security) {
            return XafTypesInfo.Instance.PersistentTypes.Single(info => info.Type == security.RoleType);
        }

        public override void CustomizeTypesInfo(ITypesInfo typesInfo) {
            base.CustomizeTypesInfo(typesInfo);
            if (RuntimeMode) {
                if (Application != null && Application.Security != null) {
                    var securityComplex = Application.Security as IRoleTypeProvider;
                    if (securityComplex != null) {
                        var typeToCreateOn = GetRoleTypeInfo(securityComplex).Type;
                        var xpCustomMemberInfos = XafTypesInfo.Instance.CreateBothPartMembers(typeToCreateOn, typeof(SendEmailJobDetailDataMap), Dictiorary, true, "RoleRoles_RoleSendEmailDataMaps", "RoleSendEmailDataMapObjects", "Roles");
                        xpCustomMemberInfos.First(info => info.Name == "RoleSendEmailDataMapObjects").AddAttribute(new VisibleInDetailViewAttribute(false));
                        typesInfo.RefreshInfo(typeToCreateOn);
                    }
                    if (Application.Security.UserType != null) {
                        var xpCustomMemberInfos = XafTypesInfo.Instance.CreateBothPartMembers(Application.Security.UserType, typeof(SendEmailJobDetailDataMap), Dictiorary, true, "UserUsers_UserSendEmailDataMapObjectUserSendEmailDataMaps", "UserSendEmailDataMapObjects", "Users");
                        xpCustomMemberInfos.First(info => info.Name == "UserSendEmailDataMapObjects").AddAttribute(new VisibleInDetailViewAttribute(false));
                        typesInfo.RefreshInfo(Application.Security.UserType);
                    }
                }
            } else {
                CreateDesignTimeCollection(typesInfo, typeof(SendEmailJobDetailDataMap), "Users");
                CreateDesignTimeCollection(typesInfo, typeof(SendEmailJobDetailDataMap), "Roles");
            }
        }

    }
}
