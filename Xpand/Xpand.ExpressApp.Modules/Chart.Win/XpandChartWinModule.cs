using System;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Chart.Win;
using DevExpress.Utils;
using EnumsNET;
using Xpand.ExpressApp.Chart.Win.Model;
using Xpand.Persistent.Base.General;
using Xpand.XAF.Modules.ModelMapper;
using Xpand.XAF.Modules.ModelMapper.Configuration;
using Xpand.XAF.Modules.ModelMapper.Services;

namespace Xpand.ExpressApp.Chart.Win {
    [ToolboxBitmap(typeof(ChartWindowsFormsModule), "Resources.Toolbox_Module_ChartListEditor_Win.ico")]
    [ToolboxItem(true)]
    [ToolboxTabName(XpandAssemblyInfo.TabWinModules)]
    public sealed class XpandChartWinModule : XpandModuleBase {
        public static string ChartControlMapName = "OptionsChart";
        public XpandChartWinModule() {
            RequiredModuleTypes.Add(typeof(ChartWindowsFormsModule));
            RequiredModuleTypes.Add(typeof(ModelMapperModule));
        }

        public override void Setup(ApplicationModulesManager moduleManager) {
            base.Setup(moduleManager);
            moduleManager.Extend(PredefinedMap.ChartControl,configuration => configuration.MapName=ChartControlMapName);
            
            var diagrams = Enums.GetMembers<PredefinedMap>().Where(member =>
                member.Name.StartsWith(PredefinedMap.ChartControl.ToString()) &&
                member.Value != PredefinedMap.ChartControl&&member.Value != PredefinedMap.ChartControlDiagram).Select(member => member.Value).ToArray();
            moduleManager.Extend((map, configuration) => {
                configuration.MapName = $"{map.ToString().Replace(PredefinedMap.ChartControl.ToString(), "Chart")}";
                configuration.DisplayName = $"{map.ToString().Replace(PredefinedMap.ChartControl.ToString(), "")}";
            },diagrams);
            
            moduleManager.ExtendMap(PredefinedMap.ChartControl)
                .Subscribe(_ => _.extenders.Add(_.targetInterface,typeof(IModelOptionsChartEx)));
        }
    }
}