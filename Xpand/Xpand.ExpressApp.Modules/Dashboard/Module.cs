using System;
using System.ComponentModel;
using System.Drawing;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Dashboards;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Security;
using DevExpress.Utils;
using Xpand.ExpressApp.Dashboard.BusinessObjects;
using Xpand.ExpressApp.Dashboard.Services;
using Xpand.Persistent.Base.General;
using Xpand.Persistent.Base.General.Controllers.Dashboard;

namespace Xpand.ExpressApp.Dashboard {

    public interface IModelApplicationDashboardModule : IModelApplication {
        IModelDashboardModule DashboardModule { get; }
    }

    public interface IModelDashboardModule : IModelNode {

    }
    [ToolboxBitmap(typeof(DashboardModule))]
    [ToolboxItem(true)]
    [ToolboxTabName(XpandAssemblyInfo.TabWinWebModules)]
    public sealed class DashboardModule : XpandModuleBase,IDashboardInteractionUser {
        public DashboardModule(){
            RequiredModuleTypes.Add(typeof(DevExpress.ExpressApp.Validation.ValidationModule));
            DashboardsModule.DataProvider=new XpandDashboardDataProvider();
        }

        public override void ExtendModelInterfaces(ModelInterfaceExtenders extenders){
            base.ExtendModelInterfaces(extenders);
            extenders.Add<IModelApplication,IModelApplicationDashboardModule>();
            extenders.Add<IModelDashboardModule, IModelDashboardModuleDataSources>();
        }

        public override void CustomizeTypesInfo(ITypesInfo typesInfo) {
            base.CustomizeTypesInfo(typesInfo);
            if (!RuntimeMode) {
                CreateWeaklyTypedCollection(typesInfo, typeof(DashboardDefinition), "Roles");
            } else if (Application.CanBuildSecurityObjects()) {
                BuildSecuritySystemObjects();
            }
        }

        public override void Setup(XafApplication application) {
            base.Setup(application);
            if (application != null && !DesignMode) {
                application.SettingUp += ApplicationOnSetupComplete;
                application.LoggedOn+=ApplicationOnLoggedOn;
            }
            AddToAdditionalExportedTypes(typeof(DashboardDefinition).Namespace, GetType().Assembly);
        }

        private void ApplicationOnLoggedOn(object sender, LogonEventArgs e){
            DashboardsModule.DataProvider.ContextApplication = (XafApplication) sender;
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