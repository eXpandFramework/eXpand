using System.Linq;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Security;
using Xpand.ExpressApp.JobScheduler.Jobs.SendEmail;
using Xpand.Persistent.Base.General;

namespace Xpand.ExpressApp.JobScheduler.Jobs {
    public class JobSchedulerJobsModule:XpandModuleBase {
        public JobSchedulerJobsModule() {
            RequiredModuleTypes.Add(typeof(SystemModule.XpandSystemModule));
            RequiredModuleTypes.Add(typeof(JobSchedulerModule));
        }
        private static ITypeInfo GetRoleTypeInfo(ISecurityComplex security) {
            return XafTypesInfo.Instance.PersistentTypes.Single(info => info.Type == security.RoleType);
        }

        public override void CustomizeTypesInfo(ITypesInfo typesInfo) {
            base.CustomizeTypesInfo(typesInfo);
            if (Application != null && Application.Security != null) {
                var securityComplex = Application.Security as ISecurityComplex;
                if (securityComplex != null) {
                    XafTypesInfo.Instance.CreateBothPartMembers(
                        GetRoleTypeInfo(securityComplex).Type, typeof(SendEmailJobDetailDataMap),
                        Dictiorary, true, "RoleRoles_RoleSendEmailDataMaps", "RoleSendEmailDataMapObjects", "Roles");
                }
                if (Application.Security.UserType != null) {
                    XafTypesInfo.Instance.CreateBothPartMembers(Application.Security.UserType, typeof(SendEmailJobDetailDataMap),
                                                                Dictiorary, true,
                                                                "UserUsers_UserSendEmailDataMapObjectUserSendEmailDataMaps", "UserSendEmailDataMapObjects", "Users");
                }
            } else {
                CreateDesignTimeCollection(typesInfo, typeof(SendEmailJobDetailDataMap), "Users");
                CreateDesignTimeCollection(typesInfo, typeof(SendEmailJobDetailDataMap), "Roles");
            }
        }

    }
}
