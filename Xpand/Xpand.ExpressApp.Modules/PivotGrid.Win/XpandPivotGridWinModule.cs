using System;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Reactive.Linq;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.PivotGrid;
using DevExpress.ExpressApp.PivotGrid.Win;
using DevExpress.ExpressApp.ViewVariantsModule;
using DevExpress.Utils;
using Xpand.ExpressApp.Dashboard;
using Xpand.ExpressApp.PivotGrid.Win.Model;
using Xpand.ExpressApp.PivotGrid.Win.NetIncome;
using Xpand.Persistent.Base.General;
using Xpand.Persistent.Base.General.Controllers.Dashboard;
using Xpand.XAF.Modules.ModelMapper;
using Xpand.XAF.Modules.ModelMapper.Configuration;
using Xpand.XAF.Modules.ModelMapper.Services;


namespace Xpand.ExpressApp.PivotGrid.Win {
    [ToolboxBitmap(typeof(PivotGridWindowsFormsModule), "Resources.Toolbox_Module_PivotGridEditor_Win.ico")]
    [ToolboxItem(true)]
    [ToolboxTabName(XpandAssemblyInfo.TabWinModules)]
    public sealed class XpandPivotGridWinModule : XpandModuleBase, IDashboardInteractionUser {
        public static string PivotGridControlModelName="OptionsPivotGrid";
        public static string PivotGridFieldModelName="OptionsPivotField";

        public XpandPivotGridWinModule() {
            RequiredModuleTypes.Add(typeof(PivotGridModule));
            RequiredModuleTypes.Add(typeof(PivotGridWindowsFormsModule));
            RequiredModuleTypes.Add(typeof(ViewVariantsModule));
            RequiredModuleTypes.Add(typeof(DashboardModule));
            RequiredModuleTypes.Add(typeof(ModelMapperModule));
        }

        protected override void OnExtendingModelInterfaces(ExtendingModelInterfacesArgs e) {
            base.OnExtendingModelInterfaces(e);
            e.Extenders.Add<IModelPivotSettings,IModelPivotSettingsEx>();
        }

        

        public override void Setup(ApplicationModulesManager moduleManager) {
            base.Setup(moduleManager);
            moduleManager.ExtendMap(PredefinedMap.PivotGridControl)
                .SelectMany(_ => new[]{typeof(IModelPivotNetIncome),typeof(IModelPivotGridExtender),typeof(IModelPivotTopObject)}
                    .Select(extenderType => (_,_.targetInterface,extenderType))
                    .Concat(_.targetInterface.Assembly.GetTypes().Where(type => type.Name==$"IModel{PredefinedMap.RepositoryItemSpinEdit}")
                        .Select(type => (_,targetInterface:typeof(IModelPivotSpinEditRule),extenderType:type))))
                .Subscribe(_ => _._.extenders.Add(_.targetInterface, _.extenderType));
            moduleManager.Extend(PredefinedMap.PivotGridControl,configuration => configuration.MapName=PivotGridControlModelName);
            moduleManager.Extend(PredefinedMap.PivotGridField,configuration => configuration.MapName=PivotGridFieldModelName);
            moduleManager.Extend(PredefinedMap.RepositoryItemSpinEdit,configuration => configuration.TargetInterfaceTypes.Add(typeof(IModelPivotSpinEditRule)));
        }
    }
}