using System;
using System.ComponentModel;
using System.Drawing;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Security;
using Xpand.ExpressApp.Dashboard.BusinessObjects;

namespace Xpand.ExpressApp.Dashboard {
    [ToolboxBitmap(typeof(DashboardModule))]
    [ToolboxItem(true)]
    public sealed class DashboardModule : XpandModuleBase {
        public override void CustomizeTypesInfo(ITypesInfo typesInfo) {
            base.CustomizeTypesInfo(typesInfo);
            if (!RuntimeMode) {
                CreateDesignTimeCollection(typesInfo, typeof(DashboardDefinition), "Roles");
            } else if ((Application.Security.UserType != null && !Application.Security.UserType.IsInterface)) {
                BuildSecuritySystemObjects();
            }
        }

        public override void Setup(XafApplication application) {
            base.Setup(application);
            if (application != null && !DesignMode) {
                application.SettingUp += ApplicationOnSetupComplete;
            }
        }
        void ApplicationOnSetupComplete(object sender, EventArgs eventArgs) {
            if (((XafApplication)sender).Security is ISecurityComplex)
                BuildSecuritySystemObjects();
        }

        void BuildSecuritySystemObjects() {
            var dynamicSecuritySystemObjects = new DynamicSecuritySystemObjects(Application);
            var xpMemberInfos = dynamicSecuritySystemObjects.BuildRole(typeof(DashboardDefinition));
            dynamicSecuritySystemObjects.HideRoleInDetailView(xpMemberInfos);
        }
    }
}