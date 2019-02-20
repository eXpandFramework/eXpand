using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using DevExpress.Utils;
using Xpand.ExpressApp.Dashboard;
using Xpand.ExpressApp.XtraDashboard.Web.Controllers;
using Xpand.Persistent.Base.General;
using SupressConfirmationController = Xpand.ExpressApp.Web.SystemModule.SupressConfirmationController;

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
        }

        protected override IEnumerable<Type> GetDeclaredControllerTypesCore(IEnumerable<Type> declaredControllerTypes){
            return new[]{typeof(SupressConfirmationController),typeof(DashboarDesignerController),typeof(DashboardViewerController),typeof(WebEditDashboardController),typeof(DisplayViewInNewTabController)};
        }
    }
}
