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
using Xpand.XAF.Modules.ModelMapper.Services;


namespace Xpand.ExpressApp.PivotGrid.Win {
    [ToolboxBitmap(typeof(PivotGridWindowsFormsModule), "Resources.Toolbox_Module_PivotGridEditor_Win.ico")]
    [ToolboxItem(true)]
    [ToolboxTabName(XpandAssemblyInfo.TabWinModules)]
    public sealed class XpandPivotGridWinModule : XpandModuleBase, IDashboardInteractionUser {
        public XpandPivotGridWinModule() {
            RequiredModuleTypes.Add(typeof(PivotGridModule));
            RequiredModuleTypes.Add(typeof(PivotGridWindowsFormsModule));
            RequiredModuleTypes.Add(typeof(ViewVariantsModule));
            RequiredModuleTypes.Add(typeof(DashboardModule));
            RequiredModuleTypes.Add(typeof(ModelMapperModule));
        }

        public override void Setup(ApplicationModulesManager moduleManager) {
            base.Setup(moduleManager);
            moduleManager.ExtendMap(PredifinedMap.PivotGridControl)
                .SelectMany(_ => new[]{typeof(IModelPivotNetIncome),typeof(IModelPivotGridExtender),typeof(IModelPivotTopObject)}.Select(extenderType => (_,extenderType)))
                .Subscribe(_ => _._.extenders.Add(_._.targetInterface, _.extenderType));
            moduleManager.Extend(PredifinedMap.PivotGridControl,configuration => configuration.MapName="OptionsPivotGrid");
            moduleManager.Extend(PredifinedMap.PivotGridField,configuration => configuration.MapName="OptionsPivotField");
        }
    }
}