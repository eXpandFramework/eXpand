using System;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.PivotChart;
using DevExpress.Persistent.Base;
using DevExpress.XtraPrinting;
using DevExpress.XtraTab;
using Fasterflect;
using Xpand.ExpressApp.PivotChart.Win.Editors;
using Xpand.Persistent.Base.General;

namespace Xpand.ExpressApp.PivotChart.Win.PropertyEditors {
    [PropertyEditor(typeof(IAnalysisInfo), true)]
    public class AnalysisEditorWin : DevExpress.ExpressApp.PivotChart.Win.AnalysisEditorWin, ISupportValueReading,IComplexViewItem {
        private IObjectSpace _objectSpace;
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

        void analysisControl_HandleCreated(object sender, EventArgs e){
            var info = CurrentObject as IAnalysisInfo;
            if (info != null && info.DataType != null)
                ReadValue();
            else if (!(CurrentObject is IAnalysisInfo))
                ReadValue();
        }

        protected override IAnalysisControl CreateAnalysisControl() {
            var analysisControl = new AnalysisControlWin();
            analysisControl.HandleCreated += analysisControl_HandleCreated;
            analysisControl.CallMethod("SetObjectSpace", _objectSpace);
            analysisControl.TabControl.SelectedPageChanged += TabControl_SelectedPageChanged1;
            return analysisControl;
        }

        private void TabControl_SelectedPageChanged1(object sender, TabPageChangedEventArgs e){
            Printable = (IPrintable) this.CallMethod("GetPrintable");
        }

        void IComplexViewItem.Setup(IObjectSpace objectSpace, XafApplication application){
            Setup(objectSpace, application);
            _objectSpace = objectSpace;
        }
    }
}