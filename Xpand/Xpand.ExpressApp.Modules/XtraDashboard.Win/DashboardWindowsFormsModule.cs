using System.ComponentModel;
using System.Drawing;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Model;
using DevExpress.Utils;
using Xpand.ExpressApp.Dashboard;
using Xpand.ExpressApp.Dashboard.PropertyEditors;
using Xpand.ExpressApp.Win.SystemModule.ModelAdapters;
using Xpand.Persistent.Base.General;
using Xpand.XAF.Modules.ModelMapper;
using Xpand.XAF.Modules.ModelMapper.Configuration;
using Xpand.XAF.Modules.ModelMapper.Services;

namespace Xpand.ExpressApp.XtraDashboard.Win {
    [ToolboxBitmap(typeof(DashboardWindowsFormsModule))]
    [ToolboxItem(true)]
    [ToolboxTabName(XpandAssemblyInfo.TabWinModules)]
    public sealed class DashboardWindowsFormsModule : XpandModuleBase {
        public DashboardWindowsFormsModule() {
            RequiredModuleTypes.Add(typeof(DashboardModule));
            RequiredModuleTypes.Add(typeof(Security.Win.XpandSecurityWinModule));
            RequiredModuleTypes.Add(typeof(DevExpress.ExpressApp.Validation.Win.ValidationWindowsFormsModule));
            RequiredModuleTypes.Add(typeof(ModelMapperModule));
        }

        public override void Setup(ApplicationModulesManager moduleManager) {
            base.Setup(moduleManager);
            moduleManager.Extend(PredefinedMap.DashboardDesigner, configuration => {
                configuration.MapName = "DesignerSettings";
                configuration.TargetInterfaceTypes.Clear();
                configuration.TargetInterfaceTypes.Add(typeof(IModelDashboardModule));
            });
            moduleManager.Extend(PredefinedMap.DashboardViewer);
        }

        public override void ExtendModelInterfaces(ModelInterfaceExtenders extenders) {
            base.ExtendModelInterfaces(extenders);
//            extenders.Add<IModelMemberViewItem, IModelMemberViewItemRichEdit>();
        }
    }
}