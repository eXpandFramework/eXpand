using DevExpress.ExpressApp;
using DevExpress.ExpressApp.NodeWrappers;
using DevExpress.ExpressApp.PivotChart;
using DevExpress.Persistent.Base;
using DevExpress.XtraPivotGrid;
using eXpand.ExpressApp.Core.DictionaryHelpers;
using eXpand.ExpressApp.Core;
using AnalysisViewControllerBase = eXpand.ExpressApp.PivotChart.Core.AnalysisViewControllerBase;

namespace eXpand.ExpressApp.PivotChart {
    public class AnalysisDisplayDateTimeViewControllerBase : AnalysisViewControllerBase {
        public const string PivotGroupInterval = "PivotGroupInterval";
        public override Schema GetSchema()
        {
            var schemaHelper = new SchemaHelper();
            DictionaryNode injectAttribute = schemaHelper.InjectAttribute(PivotGroupInterval, typeof (PivotGroupInterval), ModelElement.Member);
            return new Schema(injectAttribute);
        }

        protected PivotGroupInterval GetPivotGroupInterval(AnalysisEditorBase analysisEditor, string fieldName) {
            DictionaryNode info = analysisEditor.View.Info;
            var analysisInfo = (IAnalysisInfo)analysisEditor.MemberInfo.GetValue(analysisEditor.CurrentObject);
            return new ApplicationNodeWrapper(info.Dictionary.RootNode).BOModel.FindClassByType(analysisInfo.DataType)
                .FindMemberByName(fieldName).Node.GetAttributeEnumValue(PivotGroupInterval, DevExpress.XtraPivotGrid.PivotGroupInterval.Date);
        }
    }
}