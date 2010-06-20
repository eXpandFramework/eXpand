using System;
using DevExpress.ExpressApp.PivotChart;
using DevExpress.XtraPivotGrid;

namespace eXpand.ExpressApp.PivotChart.Core {
    public class PivotGridFieldBuilder : DevExpress.ExpressApp.PivotChart.PivotGridFieldBuilder {
        public event EventHandler<SetupGridFieldArgs> SetupGridField;
        protected virtual void OnSetupGridField(SetupGridFieldArgs e)
        {
            EventHandler<SetupGridFieldArgs> handler = SetupGridField;
            if (handler != null) handler(this, e);
        }
        public PivotGridFieldBuilder(IAnalysisControl analysisControl)
            : base(analysisControl)
        {
        }


        protected override void SetupPivotGridField(PivotGridFieldBase field, Type memberType, string displayFormat) {
            base.SetupPivotGridField(field, memberType, displayFormat);
            OnSetupGridField(new SetupGridFieldArgs(field, memberType, displayFormat));
        }
    }
}