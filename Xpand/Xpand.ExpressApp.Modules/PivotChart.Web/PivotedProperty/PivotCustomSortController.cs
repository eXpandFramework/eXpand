using System.Linq;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.PivotChart;
using DevExpress.Web.ASPxPivotGrid;
using Xpand.ExpressApp.PivotChart.PivotedProperty;
using Xpand.ExpressApp.PivotChart.Web.Editors;

namespace Xpand.ExpressApp.PivotChart.Web.PivotedProperty {
    public class PivotCustomSortController : PivotChart.PivotedProperty.PivotCustomSortController{
        protected override void CustomSort(IAnalysisControl analysisControl, IMemberInfo memberInfo){
            ASPxPivotGrid pivotGridControl = ((AnalysisControlWeb)analysisControl).PivotGrid;
            pivotGridControl.CustomFieldSort += (sender, args) =>{
                var pivotedSortAttribute = memberInfo.FindAttributes<PivotedSortAttribute>().SingleOrDefault(attribute => attribute.PropertyName == args.Field.FieldName);
                if (pivotedSortAttribute != null){
                    var compareResult = GetCompareResult(pivotedSortAttribute.SortDirection,
                        args.GetListSourceColumnValue(args.ListSourceRowIndex1, pivotedSortAttribute.SortPropertyName),
                        args.GetListSourceColumnValue(args.ListSourceRowIndex2, pivotedSortAttribute.SortPropertyName));
                    args.Result = compareResult;
                    args.Handled = true;
                }
            };
        }

    }
}