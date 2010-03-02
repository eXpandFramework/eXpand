using System;
using DevExpress.ExpressApp.PivotChart;
using DevExpress.XtraPivotGrid;

namespace eXpand.ExpressApp.PivotChart.InPlaceEdit {
    public class PivotGridFieldBuilder : DevExpress.ExpressApp.PivotChart.PivotGridFieldBuilder {
        readonly Func<string, PivotGroupInterval> _func;


        public PivotGridFieldBuilder(IAnalysisControl analysisControl,Func<string,PivotGroupInterval> func) 
            : base(analysisControl) {
            _func = func;
        }

        protected override void SetupPivotGridField(PivotGridFieldBase field, Type memberType, string displayFormat) {
            base.SetupPivotGridField(field, memberType, displayFormat);
            if (memberType==typeof(DateTime))
                field.GroupInterval = _func.Invoke(field.FieldName);
        }
    }
}