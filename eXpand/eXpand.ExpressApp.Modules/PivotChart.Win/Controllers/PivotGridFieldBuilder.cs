using System;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.NodeWrappers;
using DevExpress.Persistent.Base;
using DevExpress.XtraPivotGrid;
using eXpand.ExpressApp.Core;

namespace eXpand.ExpressApp.PivotChart.Win.Controllers {
    public class PivotGridFieldBuilder : DevExpress.ExpressApp.PivotChart.PivotGridFieldBuilder {
        readonly AnalysisEditorWin _analysisEditor;


        public PivotGridFieldBuilder(AnalysisEditorWin analysisEditor) : base(analysisEditor.Control) {
            _analysisEditor = analysisEditor;
        }

        protected override void SetupPivotGridField(PivotGridFieldBase field, Type memberType, string displayFormat) {
            base.SetupPivotGridField(field, memberType, displayFormat);
            if (memberType == typeof (DateTime)) {
                DictionaryNode info = _analysisEditor.View.Info;
                var analysisInfo = (IAnalysisInfo) _analysisEditor.MemberInfo.GetValue(_analysisEditor.CurrentObject);
                field.GroupInterval =new ApplicationNodeWrapper(info.Dictionary.RootNode).BOModel.FindClassByType(
                        analysisInfo.DataType).FindMemberByName(field.FieldName).Node.GetAttributeEnumValue("PivotGroupInterval",
                                                                                     PivotGroupInterval.Date);
            }
        }
    }
}