using System;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.DC;
using Xpand.ExpressApp.JobScheduler.Jobs.SendEmail;
using Xpand.Persistent.Base.General;

namespace Xpand.ExpressApp.JobScheduler.Jobs {
    public class JobSchedulerJobsModule : XpandModuleBase {
        public JobSchedulerJobsModule() {
            RequiredModuleTypes.Add(typeof(JobSchedulerModule));
        }

        public override void Setup(XafApplication application) {
            base.Setup(application);
            if (application != null && !DesignMode) {
                application.SetupComplete += ApplicationOnSetupComplete;
            }
            if (RuntimeMode) {
                AddToAdditionalExportedTypes(typeof(SendEmailJobDataMap).Namespace, GetType().Assembly);
            }
        }

        void ApplicationOnSetupComplete(object sender, EventArgs eventArgs) {
            BuildSecuritySystemObjects();
        }

        public override void CustomizeTypesInfo(ITypesInfo typesInfo) {
            base.CustomizeTypesInfo(typesInfo);
            if (!RuntimeMode) {
                CreateDesignTimeCollection(typesInfo, typeof(SendEmailJobDetailDataMap), "Users");
                CreateDesignTimeCollection(typesInfo, typeof(SendEmailJobDetailDataMap), "Roles");
            } else if ((Application.Security.UserType != null && !Application.Security.UserType.IsInterface)) {
                BuildSecuritySystemObjects();
            }
        }

        void BuildSecuritySystemObjects() {
            var dynamicSecuritySystemObjects = new DynamicSecuritySystemObjects(Application);
            var xpMemberInfos = dynamicSecuritySystemObjects.BuildUser(typeof(SendEmailJobDetailDataMap), "UserUsers_UserSendEmailDataMapObjectUserSendEmailDataMaps", "UserSendEmailDataMapObjects", "Users");
            dynamicSecuritySystemObjects.HideInDetailView(xpMemberInfos, "UserSendEmailDataMapObjects");
            xpMemberInfos = dynamicSecuritySystemObjects.BuildRole(typeof(SendEmailJobDetailDataMap), "RoleRoles_RoleSendEmailDataMaps", "RoleSendEmailDataMapObjects", "Roles");
            dynamicSecuritySystemObjects.HideInDetailView(xpMemberInfos, "RoleSendEmailDataMapObjects");

        }
    }
}
