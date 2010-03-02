using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using DevExpress.Web.ASPxPivotGrid;
using eXpand.ExpressApp.PivotChart.Web.Core;
using eXpand.ExpressApp.PivotChart.Web.Editors;

namespace eXpand.ExpressApp.PivotChart.Web.Options
{
    public class PivotOptionsController : PivotChart.PivotOptionsController
    {
        protected override Dictionary<Type, Type> GetActionChoiceItems() {
            return PivotGridOptionMapper.Instance.Dictionary;
        }

        protected override Type GetPersistentType(Type type) {
            return PivotGridOptionMapper.Instance[type];
        }

        protected override IEnumerable<object> GetGridOptionInstance(Type type) {
            var asPxPivotGrids = AnalysisEditors.Where(analysisEditor => analysisEditor.Control != null).Select(analysisEditor => ((AnalysisControlWeb)analysisEditor.Control).PivotGrid);
            foreach (var asPxPivotGrid in asPxPivotGrids) {
                foreach (var propertyInfo in PropertyInfos(type)) {
                    yield return propertyInfo.GetValue(asPxPivotGrid,null);
                }
            }
//            return asPxPivotGrids.Select(pivotGridControl => {
//                var propertyInfos = PropertyInfos(type);
//                var single = propertyInfos.ToList().Select(info1 => info1.GetValue(pivotGridControl, null)).Single();
//                return single;
//            }).ToList();
        }

        IEnumerable<PropertyInfo> PropertyInfos(Type type) {
            return typeof(ASPxPivotGrid).GetProperties().Where(propertyInfo => propertyInfo.PropertyType == type);
        }
    }
}


