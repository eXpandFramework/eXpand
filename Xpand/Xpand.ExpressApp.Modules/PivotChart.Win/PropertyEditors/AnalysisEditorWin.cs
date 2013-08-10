using System;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.PivotChart;
using DevExpress.Persistent.Base;
using Xpand.ExpressApp.PivotChart.Win.Editors;
using Xpand.Persistent.Base.General;

namespace Xpand.ExpressApp.PivotChart.Win.PropertyEditors {
    [PropertyEditor(typeof(IAnalysisInfo), true)]
    public class AnalysisEditorWin : DevExpress.ExpressApp.PivotChart.Win.AnalysisEditorWin, ISupportValueReading {
        public AnalysisEditorWin(Type objectType, IModelMemberViewItem model) : base(objectType, model) { }
        public event EventHandler ValueReading;

        protected void OnValueReading(EventArgs e) {
            EventHandler handler = ValueReading;
            if (handler != null) handler(this, e);
        }

        public new AnalysisControlWin Control {
            get { return (AnalysisControlWin)base.Control; }
        }
        protected override void ReadValueCore() {
            OnValueReading(new EventArgs());
            base.ReadValueCore();
        }

        void analysisControl_HandleCreated(object sender, EventArgs e) {
            if (CurrentObject is IAnalysisInfo && ((IAnalysisInfo)CurrentObject).DataType != null)
                ReadValue();
            else if (!(CurrentObject is IAnalysisInfo))
                ReadValue();
        }

        protected override IAnalysisControl CreateAnalysisControl() {
            var analysisControl = new AnalysisControlWin();
            analysisControl.HandleCreated += analysisControl_HandleCreated;
            return analysisControl;
        }
    }
}