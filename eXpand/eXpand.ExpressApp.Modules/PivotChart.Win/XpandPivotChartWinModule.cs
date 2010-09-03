using System;
using System.Collections.Generic;
using DevExpress.ExpressApp.Model;
using eXpand.ExpressApp.PivotChart.Core;
using eXpand.ExpressApp.PivotChart.Win.Options;
using AnalysisPropertyEditorNodeUpdater = eXpand.ExpressApp.PivotChart.Win.Core.AnalysisPropertyEditorNodeUpdater;
using TypesInfo = eXpand.ExpressApp.PivotChart.Core.TypesInfo;

namespace eXpand.ExpressApp.PivotChart.Win {

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