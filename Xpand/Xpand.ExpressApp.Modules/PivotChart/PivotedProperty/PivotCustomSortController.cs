using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.PivotChart;
using DevExpress.Xpo;
using DevExpress.XtraPivotGrid;
using AnalysisViewControllerBase = Xpand.ExpressApp.PivotChart.Core.AnalysisViewControllerBase;
using PivotGridFieldBuilder = Xpand.ExpressApp.PivotChart.Core.PivotGridFieldBuilder;

namespace Xpand.ExpressApp.PivotChart.PivotedProperty {
    public abstract class PivotCustomSortController : AnalysisViewControllerBase {
        protected void OnSetupGridField(AnalysisEditorBase analysisEditor, PivotGridFieldBase pivotGridField) {
            IMemberInfo memberInfo1 = analysisEditor.MemberInfo.GetPath()[0];
            IEnumerable<string> properties =
                memberInfo1.FindAttributes<PivotedSortAttribute>().Select(attribute => attribute.PropertyName);
            if (properties.Contains(pivotGridField.FieldName))
                pivotGridField.SortMode = PivotSortMode.Custom;
        }

        protected override void OnAnalysisControlCreated() {
            base.OnAnalysisControlCreated();
            foreach (AnalysisEditorBase analysisEditor in AnalysisEditors) {
                analysisEditor.ControlCreated += AnalysisEditorOnControlCreated;
            }
        }

        void AnalysisEditorOnControlCreated(object sender, EventArgs eventArgs) {
            var editor = ((AnalysisEditorBase)sender);
            editor.ControlCreated -= AnalysisEditorOnControlCreated;
            var supportPivotGridFieldBuilder = ((ISupportPivotGridFieldBuilder)editor.Control);
            SetupGridField(supportPivotGridFieldBuilder, editor);
            var analysisControl = (IAnalysisControl)supportPivotGridFieldBuilder;
            IMemberInfo memberInfo = editor.MemberInfo.GetPath()[0];
            CustomSort(analysisControl, memberInfo);
        }

        protected abstract void CustomSort(IAnalysisControl analysisControl, IMemberInfo memberInfo);
        protected virtual void SetupGridField(ISupportPivotGridFieldBuilder supportPivotGridFieldBuilder, AnalysisEditorBase editor) {
            ((PivotGridFieldBuilder)supportPivotGridFieldBuilder.FieldBuilder).SetupGridField += (o, fieldArgs) => OnSetupGridField(editor, fieldArgs.PivotGridField);
        }

        protected int GetCompareResult(SortDirection sortDirection, object val1, object val2) {
            int result = Comparer.Default.Compare(val1, val2);
            if (sortDirection == SortDirection.Descending)
                result = result * -1;
            return result;
        }

    }
}