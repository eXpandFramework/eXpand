using System.ComponentModel;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.PivotChart;
using DevExpress.XtraPivotGrid;
using AnalysisViewControllerBase = eXpand.ExpressApp.PivotChart.Core.AnalysisViewControllerBase;

namespace eXpand.ExpressApp.PivotChart {
    public interface IModelMemberAnalysisDisplayDateTime:IModelMember
    {
        [Category("eXpand")]
        PivotGroupInterval PivotGroupInterval { get; set; }
    }
    public interface IModelPropertyEditorAnalysisDisplayDateTime:IModelPropertyEditor
    {
        [Category("eXpand")]
        [ModelValueCalculator("((IModelMemberAnalysisDisplayDateTime)ModelMember)", "PivotGroupInterval")]
        PivotGroupInterval PivotGroupInterval { get; set; }
    }
    public class AnalysisDisplayDateTimeViewControllerBase : AnalysisViewControllerBase,IModelExtender {
        

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