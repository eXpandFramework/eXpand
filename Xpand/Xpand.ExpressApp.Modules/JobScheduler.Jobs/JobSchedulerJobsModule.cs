using System;
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
            return XafTypesInfo.Instance.PersistentTypes.Where(info => info.Type == security.RoleType).Single();
        }

        public override void CustomizeTypesInfo(ITypesInfo typesInfo) {
            base.CustomizeTypesInfo(typesInfo);
            if (Application != null && Application.Security != null) {
                if (Application.Security is ISecurityComplex) {
                    XafTypesInfo.Instance.CreateBothPartMembers(
                        GetRoleTypeInfo((ISecurityComplex)Application.Security).Type, typeof(SendEmailJobDetailDataMap),
                        XafTypesInfo.XpoTypeInfoSource.XPDictionary, true, "RoleRoles_RoleSendEmailDataMaps", "RoleSendEmailDataMapObjects", "Roles");
                }
                if (Application.Security.UserType != null) {
                    XafTypesInfo.Instance.CreateBothPartMembers(Application.Security.UserType, typeof(SendEmailJobDetailDataMap),
                                                                XafTypesInfo.XpoTypeInfoSource.XPDictionary, true,
                                                                "UserUsers_UserSendEmailDataMapObjectUserSendEmailDataMaps", "UserSendEmailDataMapObjects", "Users");
                }
            } else {
                CreateDesignTimeCollection(typesInfo, typeof(SendEmailJobDetailDataMap), "Users");
                CreateDesignTimeCollection(typesInfo, typeof(SendEmailJobDetailDataMap), "Roles");
            }
        }

    }
}
