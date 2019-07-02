using System;
using System.ComponentModel;
using System.Drawing;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Chart.Win;
using DevExpress.Utils;
using Xpand.ExpressApp.Chart.Win.Model;
using Xpand.Persistent.Base.General;
using Xpand.XAF.Modules.ModelMapper;
using Xpand.XAF.Modules.ModelMapper.Services;

namespace Xpand.ExpressApp.Chart.Win {
    [ToolboxBitmap(typeof(ChartWindowsFormsModule), "Resources.Toolbox_Module_ChartListEditor_Win.ico")]
    [ToolboxItem(true)]
    [ToolboxTabName(XpandAssemblyInfo.TabWinModules)]
    public sealed class XpandChartWinModule : XpandModuleBase {
        public XpandChartWinModule() {
            RequiredModuleTypes.Add(typeof(ChartWindowsFormsModule));
            RequiredModuleTypes.Add(typeof(ModelMapperModule));
        }

        public override void Setup(ApplicationModulesManager moduleManager) {
            base.Setup(moduleManager);
            moduleManager.Extend(PredifinedMap.ChartControl,configuration => configuration.MapName="OptionsChart");
            moduleManager.ExtendMap(PredifinedMap.ChartControl)
                .Subscribe(_ => _.extenders.Add(_.targetInterface,typeof(IModelOptionsChartEx)));
        }
    }
}