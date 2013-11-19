using System;
using System.Collections.Generic;
using System.Linq;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.PivotChart;
using DevExpress.Persistent.Base;

namespace Xpand.ExpressApp.PivotChart.Core {
    public abstract class AnalysisViewControllerBase : ViewController<DetailView> {
        IList<AnalysisEditorBase> analysisEditors = new List<AnalysisEditorBase>();

        public IList<AnalysisEditorBase> AnalysisEditors {
            get { return analysisEditors; }
        }

        protected AnalysisViewControllerBase() {
            TargetObjectType = typeof(IAnalysisInfo);
        }

        protected bool IsDataSourceReady {
            get { return analysisEditors.All(@base => @base.IsDataSourceReady); }
        }

        void analysisEditor_IsDataSourceReadyChanged(object sender, EventArgs e) {
            UpdateActionState();
        }

        void View_ControlsCreated(object sender, EventArgs e) {
            OnAnalysisControlCreated();
        }

        protected override void OnActivated() {
            base.OnActivated();
            InitAnalysisEditor();
            if (View.IsControlCreated) {
                OnAnalysisControlCreated();
            } else {
                View.ControlsCreated += View_ControlsCreated;
            }
        }

        protected override void OnDeactivated() {
            foreach (AnalysisEditorBase analysisEditor in analysisEditors) {
                analysisEditor.IsDataSourceReadyChanged -= analysisEditor_IsDataSourceReadyChanged;
            }
            View.ControlsCreated -= View_ControlsCreated;
            foreach (AnalysisEditorBase analysisEditor in analysisEditors) {
                analysisEditor.ControlCreated += AnalysisEditorOnControlCreated;
            }
            analysisEditors = null;
            base.OnDeactivated();
        }

        protected virtual void InitAnalysisEditor() {
            analysisEditors = View.GetItems<AnalysisEditorBase>();
        }

        protected virtual void OnAnalysisControlCreated() {
            UpdateActionState();
            foreach (AnalysisEditorBase analysisEditor in analysisEditors) {
                analysisEditor.ControlCreated+=AnalysisEditorOnControlCreated;
            }
        }

        void AnalysisEditorOnControlCreated(object sender, EventArgs eventArgs) {
            var analysisEditor = ((AnalysisEditorBase) sender);
            analysisEditor.ControlCreated-=AnalysisEditorOnControlCreated;
            IAnalysisControl analysisControl = analysisEditor.Control;
            if (!(((ISupportPivotGridFieldBuilder)analysisControl).FieldBuilder is PivotGridFieldBuilder)) {
                var pivotGridFieldBuilder = new PivotGridFieldBuilder(analysisControl);
                pivotGridFieldBuilder.SetModel(Application.Model);
                ((ISupportPivotGridFieldBuilder)analysisControl).FieldBuilder = pivotGridFieldBuilder;
            }
            analysisEditor.IsDataSourceReadyChanged += analysisEditor_IsDataSourceReadyChanged;
        }

        protected virtual void UpdateActionState() {
        }
    }
}