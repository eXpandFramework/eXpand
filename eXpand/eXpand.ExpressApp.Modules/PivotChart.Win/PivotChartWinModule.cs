using System;
using System.Collections.Generic;
using eXpand.ExpressApp.PivotChart.Core;
using eXpand.ExpressApp.PivotChart.Win.Options;
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

    }
}