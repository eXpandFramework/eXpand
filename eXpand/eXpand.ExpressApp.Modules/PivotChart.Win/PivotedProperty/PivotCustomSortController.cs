using System.Linq;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.PivotChart;
using DevExpress.XtraPivotGrid;
using eXpand.ExpressApp.PivotChart.PivotedProperty;
using eXpand.ExpressApp.PivotChart.Win.Editors;

namespace eXpand.ExpressApp.PivotChart.Win.PivotedProperty {
    public class PivotCustomSortController : PivotChart.PivotedProperty.PivotCustomSortController{
        protected override void CustomSort(IAnalysisControl analysisControl, IMemberInfo memberInfo){
            PivotGridControl pivotGridControl = ((AnalysisControlWin)analysisControl).PivotGrid;
            pivotGridControl.CustomFieldSort += (sender, args) =>{
                PivotedSortAttribute pivotedSortAttribute = memberInfo.FindAttributes<PivotedSortAttribute>().Where(attribute 
                    => attribute.PropertyName == args.Field.FieldName).SingleOrDefault();
                if (pivotedSortAttribute != null){
                    int compareResult = GetCompareResult(pivotedSortAttribute.SortDirection,
                                                         args.GetListSourceColumnValue(args.ListSourceRowIndex1, pivotedSortAttribute.SortPropertyName),
                                                         args.GetListSourceColumnValue(args.ListSourceRowIndex2, pivotedSortAttribute.SortPropertyName));
                    args.Result = compareResult;
                    args.Handled = true;
                }
            };

        }

    }
}