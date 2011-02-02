using System;
using System.Collections.Generic;
using System.Linq;
using DevExpress.XtraPivotGrid;
using Xpand.ExpressApp.PivotChart.Win.Editors;

namespace Xpand.ExpressApp.PivotChart.Win.Options {
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