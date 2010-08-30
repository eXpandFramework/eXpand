using System;
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.PivotChart;
using DevExpress.ExpressApp.Templates;
using DevExpress.Persistent.Base;
using System.Linq;
using AnalysisViewControllerBase = eXpand.ExpressApp.PivotChart.Core.AnalysisViewControllerBase;

namespace eXpand.ExpressApp.PivotChart {
    public class AnalysisDataBindController : AnalysisViewControllerBase {
        public const string BindMultiAnalysisData = "BindMultiAnalysisData";
        public const string Unbindmultianalysisdata = "UnbindMultiAnalysisData";
        readonly SimpleAction bindDataAction;
        readonly SimpleAction unbindDataAction;

        public AnalysisDataBindController() {
            
            bindDataAction = new SimpleAction(this, BindMultiAnalysisData,PredefinedCategory.RecordEdit);
            bindDataAction.Execute += bindDataAction_Execute;
            bindDataAction.Caption = "Bind Analysis Data";
            bindDataAction.SelectionDependencyType = SelectionDependencyType.RequireSingleObject;
            bindDataAction.ImageName = "MenuBar_BindAnalysisData";
            bindDataAction.PaintStyle = ActionItemPaintStyle.CaptionAndImage;
            
            unbindDataAction = new SimpleAction(this, Unbindmultianalysisdata, PredefinedCategory.RecordEdit);
            unbindDataAction.Execute += unbindDataAction_Execute;
            unbindDataAction.Caption = "Unbind Analysis Data";
            unbindDataAction.SelectionDependencyType = SelectionDependencyType.RequireSingleObject;
            unbindDataAction.ImageName = "MenuBar_UnbindAnalysisData";
            unbindDataAction.PaintStyle = ActionItemPaintStyle.CaptionAndImage;
        }

        public SimpleAction BindDataAction {
            get { return bindDataAction; }
        }

        public SimpleAction UnbindDataAction {
            get { return unbindDataAction; }
        }
        
        void bindDataAction_Execute(object sender, SimpleActionExecuteEventArgs e) {
            BindDataToControl();
        }

        void unbindDataAction_Execute(object sender, SimpleActionExecuteEventArgs e) {
            UnbindDataFromControl();
        }

        void analysisEditor_IsDataSourceReadyChanged(object sender, EventArgs e) {
            UpdateActionState();
        }

        protected virtual void SubscribeToEvents() {
            foreach (AnalysisEditorBase analysisEditor in AnalysisEditors) {
                analysisEditor.IsDataSourceReadyChanged += analysisEditor_IsDataSourceReadyChanged;
            }
        }

        protected virtual void UnsubscribeFromEvents() {
            foreach (AnalysisEditorBase analysisEditor in AnalysisEditors) {
                analysisEditor.IsDataSourceReadyChanged -= analysisEditor_IsDataSourceReadyChanged;
            }
        }

        public virtual void BindDataToControl() {
            foreach (AnalysisEditorBase analysisEditor in AnalysisEditors.Where(@base => !@base.IsDataSourceReady)) {
                analysisEditor.IsDataSourceReady = true;
            }
            UpdateBindUnbindActionsState();
        }

        protected virtual void UnbindDataFromControl() {
            foreach (AnalysisEditorBase analysisEditor in AnalysisEditors) {
                analysisEditor.IsDataSourceReady = false;
            }
            UpdateBindUnbindActionsState();
        }

        protected virtual void UpdateBindUnbindActionsState() {
            bindDataAction.Active["IsDataSourceReady"] = !IsDataSourceReady ;
            unbindDataAction.Active["IsDataSourceReady"] = IsDataSourceReady ;
        }

        protected override void Dispose(bool disposing) {
            if (disposing) {
                bindDataAction.Execute -= bindDataAction_Execute;
                unbindDataAction.Execute -= bindDataAction_Execute;
            }
            base.Dispose(disposing);
        }
        protected override void OnFrameAssigned()
        {
            base.OnFrameAssigned();
            Frame.GetController<DevExpress.ExpressApp.PivotChart.AnalysisDataBindController>().Active[GetType().FullName] = false;
        }
        protected override void OnActivated() {
            base.OnActivated();
            
            SubscribeToEvents();
            foreach (AnalysisEditorBase analysisEditor in AnalysisEditors) {
                analysisEditor.IsDataSourceReady = false;
            }
            UpdateBindUnbindActionsState();
        }

        protected override void OnDeactivating() {
            UnsubscribeFromEvents();
            base.OnDeactivating();
        }
    }
}