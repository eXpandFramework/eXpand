using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using DevExpress.ExpressApp.Model;
using DevExpress.Utils;
using Xpand.ExpressApp.Dashboard;
using Xpand.ExpressApp.XtraDashboard.Web.Controllers;
using Xpand.ExpressApp.XtraDashboard.Web.PropertyEditors;
using Xpand.Persistent.Base.General;


namespace Xpand.ExpressApp.XtraDashboard.Web {
    [ToolboxBitmap(typeof(XtraDashboardWebModule))]
    [ToolboxItem(true)]
    [ToolboxTabName(XpandAssemblyInfo.TabAspNetModules)]
    public sealed class XtraDashboardWebModule : XpandModuleBase { 
        public XtraDashboardWebModule() {
            RequiredModuleTypes.Add(typeof(DashboardModule));
            RequiredModuleTypes.Add(typeof(DevExpress.ExpressApp.Web.SystemModule.SystemAspNetModule));
            RequiredModuleTypes.Add(typeof(Security.Web.XpandSecurityWebModule));
            RequiredModuleTypes.Add(typeof(DevExpress.ExpressApp.Validation.Web.ValidationAspNetModule));
            RequiredModuleTypes.Add(typeof(XAF.Modules.SupressConfirmation.SupressConfirmationModule));
        }

        public override void ExtendModelInterfaces(ModelInterfaceExtenders extenders) {
            base.ExtendModelInterfaces(extenders);
            extenders.Add<IModelDashboardModule,IModelDashboardModuleASPxDashboard>();
        }

        protected override IEnumerable<Type> GetDeclaredControllerTypesCore(IEnumerable<Type> declaredControllerTypes){
            return new[]{typeof(DashboarDesignerController),typeof(DashboardViewerController),typeof(WebEditDashboardController),typeof(DisplayViewInNewTabController)};
        }
    }
}
