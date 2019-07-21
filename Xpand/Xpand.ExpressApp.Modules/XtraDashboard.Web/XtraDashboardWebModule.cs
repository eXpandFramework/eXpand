using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Model;
using DevExpress.Utils;
using Xpand.ExpressApp.Dashboard;
using Xpand.ExpressApp.XtraDashboard.Web.Controllers;
using Xpand.ExpressApp.XtraDashboard.Web.PropertyEditors;
using Xpand.Persistent.Base.General;
using Xpand.XAF.Modules.ModelMapper;
using Xpand.XAF.Modules.ModelMapper.Configuration;
using Xpand.XAF.Modules.ModelMapper.Services;


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
            RequiredModuleTypes.Add(typeof(XAF.Modules.SuppressConfirmation.SuppressConfirmationModule));
            RequiredModuleTypes.Add(typeof(ModelMapperModule));
        }

        public override void Setup(ApplicationModulesManager moduleManager) {
            base.Setup(moduleManager);
            
            moduleManager.Extend(PredefinedMap.ASPxDashboard);
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
