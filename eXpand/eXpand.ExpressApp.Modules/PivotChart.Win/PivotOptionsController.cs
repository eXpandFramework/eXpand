using System;
using System.Collections.Generic;
using System.Linq;
using DevExpress.XtraPivotGrid;
using eXpand.ExpressApp.PivotChart.Win.Core;

namespace eXpand.ExpressApp.PivotChart.Win {
    public class PivotOptionsController : PivotChart.PivotOptionsController {
        protected override Dictionary<Type, Type> GetActionChoiceItems() {
            return PivotGridOptionMapper.Instance.Dictionary;
        }

        protected override Type GetPersistentType(Type type) {
            return PivotGridOptionMapper.Instance[type];
        }

        protected override IEnumerable<object> GetGridOptionInstance(Type type) {
            return AnalysisEditors.Select(analysisEditor => ((AnalysisControlWin) analysisEditor.Control).PivotGrid).Select
                    (pivotGridControl =>
                     typeof (PivotGridControl).GetProperties().Where(propertyInfo => propertyInfo.PropertyType == type).
                         Select(info1 => info1.GetValue(pivotGridControl, null)).Single()).ToList();
        }
    }
}