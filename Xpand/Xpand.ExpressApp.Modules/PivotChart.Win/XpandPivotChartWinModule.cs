using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.PivotChart;
using DevExpress.ExpressApp.PivotChart.Win;
using Xpand.ExpressApp.PivotChart.Core;
using Xpand.ExpressApp.PivotChart.Win.Options;
using AnalysisPropertyEditorNodeUpdater = Xpand.ExpressApp.PivotChart.Win.Core.AnalysisPropertyEditorNodeUpdater;

namespace Xpand.ExpressApp.PivotChart.Win {
    [ToolboxBitmap(typeof(XpandPivotChartWinModule))]
    [ToolboxItem(true)]
    public sealed class XpandPivotChartWinModule : XpandPivotChartModuleBase {
        public XpandPivotChartWinModule() {
            RequiredModuleTypes.Add(typeof(XpandPivotChartModule));
            RequiredModuleTypes.Add(typeof(PivotChartModuleBase));
            RequiredModuleTypes.Add(typeof(PivotChartWindowsFormsModule));
        }

        public override TypesInfo TypesInfo {
            get { return Core.TypesInfo.Instance; }
        }


        protected override Dictionary<Type, Type> GetOptionsMapperDictionary() {
            return PivotGridOptionMapper.Instance.Dictionary;
        }


        protected override IModelNodesGeneratorUpdater GetAnalysisPropertyEditorNodeUpdater() {
            return new AnalysisPropertyEditorNodeUpdater();
        }
    }
}