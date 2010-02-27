using System;
using System.Collections.Generic;
using eXpand.ExpressApp.PivotChart.Win.Core;
using TypesInfo = eXpand.ExpressApp.PivotChart.Core.TypesInfo;

namespace eXpand.ExpressApp.PivotChart.Win {
    public sealed partial class PivotChartWinModule : PivotChartXpandModuleBase {
        public PivotChartWinModule() {
            InitializeComponent();
        }


        protected override Dictionary<Type, Type> GetOptionsMapperDictionary() {
            return PivotGridOptionMapper.Instance.Dictionary;
        }


        public override TypesInfo TypesInfo {
            get { return Core.TypesInfo.Instance; }
        }

        protected override Type GetPropertyEditorType() {
            return typeof (AnalysisEditorWin);
        }
    }
}