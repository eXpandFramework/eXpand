using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using DevExpress.ExpressApp.Model;
using Xpand.ExpressApp.PivotChart.Core;
using Xpand.ExpressApp.PivotChart.Win.Options;
using AnalysisPropertyEditorNodeUpdater = Xpand.ExpressApp.PivotChart.Win.Core.AnalysisPropertyEditorNodeUpdater;
using TypesInfo = Xpand.ExpressApp.PivotChart.Core.TypesInfo;

namespace Xpand.ExpressApp.PivotChart.Win {
    [ToolboxBitmap(typeof(XpandPivotChartWinModule))]
    [ToolboxItem(true)]
    public sealed partial class XpandPivotChartWinModule : XpandPivotChartModuleBase {
        public XpandPivotChartWinModule() {
            InitializeComponent();
        }


        protected override Dictionary<Type, Type> GetOptionsMapperDictionary() {
            return PivotGridOptionMapper.Instance.Dictionary;
        }


        protected override IModelNodesGeneratorUpdater GetAnalysisPropertyEditorNodeUpdater() {
            return new AnalysisPropertyEditorNodeUpdater(); 
        }

        public override TypesInfo TypesInfo {
            get { return Core.TypesInfo.Instance; }
        }

    }
}