using System;
using System.ComponentModel;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.PivotChart;
using DevExpress.XtraPivotGrid;
using eXpand.ExpressApp.PivotChart.Core;
using AnalysisViewControllerBase = eXpand.ExpressApp.PivotChart.Core.AnalysisViewControllerBase;
using PivotGridFieldBuilder = eXpand.ExpressApp.PivotChart.Core.PivotGridFieldBuilder;

namespace eXpand.ExpressApp.PivotChart {
    public interface IModelMemberAnalysisDisplayDateTime:IModelMember
    {
        [Category("eXpand.PivotChart")]
        PivotGroupInterval PivotGroupInterval { get; set; }
    }
    public interface IModelPropertyEditorAnalysisDisplayDateTime:IModelPropertyEditor
    {
        [Category("eXpand.PivotChart")]
        [ModelValueCalculator("((IModelMemberAnalysisDisplayDateTime)ModelMember)", "PivotGroupInterval")]
        PivotGroupInterval PivotGroupInterval { get; set; }
    }
    public class AnalysisDisplayDateTimeViewController : AnalysisViewControllerBase,IModelExtender {
        protected override void OnAnalysisControlCreated() {
            base.OnAnalysisControlCreated();
            foreach (AnalysisEditorBase analysisEditor in AnalysisEditors) {
                AnalysisEditorBase editor = analysisEditor;
                ((PivotGridFieldBuilder) ((ISupportPivotGridFieldBuilder) analysisEditor.Control).FieldBuilder).
                    SetupGridField += (sender, args) => OnSetupGridField(args, editor);
            }
        }

        void OnSetupGridField(SetupGridFieldArgs setupGridFieldArgs, AnalysisEditorBase editor) {
            PivotGridFieldBase pivotGridFieldBase = setupGridFieldArgs.PivotGridField;
            if (setupGridFieldArgs.MemberType == typeof (DateTime))
                pivotGridFieldBase.GroupInterval = GetPivotGroupInterval(editor,setupGridFieldArgs.PivotGridField.FieldName);
        }

        protected PivotGroupInterval GetPivotGroupInterval(AnalysisEditorBase analysisEditor, string fieldName) {
            return ((IModelPropertyEditorAnalysisDisplayDateTime)analysisEditor.Model).PivotGroupInterval;
        }

        #region IModelExtender Members

        void IModelExtender.ExtendModelInterfaces(ModelInterfaceExtenders extenders)
        {
            extenders.Add<IModelMember,IModelMemberAnalysisDisplayDateTime>();
            extenders.Add<IModelPropertyEditor,IModelPropertyEditorAnalysisDisplayDateTime>();
        }

        #endregion
    }
}