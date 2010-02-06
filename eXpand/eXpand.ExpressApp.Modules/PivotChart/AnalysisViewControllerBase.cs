using System;
using System.Collections.Generic;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.PivotChart;
using DevExpress.Persistent.Base;
using System.Linq;

namespace eXpand.ExpressApp.PivotChart {

    public abstract class AnalysisViewControllerBase : ViewController<DetailView> {
        IList<AnalysisEditorBase> analysisEditors = new List<AnalysisEditorBase>();

        public IList<AnalysisEditorBase> AnalysisEditors {
            get { return analysisEditors; }
        }

        protected AnalysisViewControllerBase() {
            TargetObjectType = typeof (IAnalysisInfo);
            TargetViewType = ViewType.DetailView;
        }

        protected bool IsDataSourceReady {
            get { return analysisEditors.Where(@base => !@base.IsDataSourceReady).Count()==0; }
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
            }
            else {
                View.ControlsCreated += View_ControlsCreated;
            }
        }

        protected override void OnDeactivating() {
            foreach (AnalysisEditorBase analysisEditor in analysisEditors) {
                analysisEditor.IsDataSourceReadyChanged -= analysisEditor_IsDataSourceReadyChanged;
            }
            View.ControlsCreated -= View_ControlsCreated;
            analysisEditors = null;
            base.OnDeactivating();
        }

        protected virtual void InitAnalysisEditor() {
            analysisEditors = View.GetItems<AnalysisEditorBase>();
        }

        protected virtual void OnAnalysisControlCreated() {
            UpdateActionState();
            foreach (AnalysisEditorBase analysisEditor in analysisEditors){
                analysisEditor.IsDataSourceReadyChanged +=analysisEditor_IsDataSourceReadyChanged;
            }
        }

        protected virtual void UpdateActionState() {
        }
    }
}