using System.ComponentModel;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Model;
using DevExpress.XtraPivotGrid;
using AnalysisViewControllerBase = Xpand.ExpressApp.PivotChart.Core.AnalysisViewControllerBase;

namespace Xpand.ExpressApp.PivotChart {
    public interface IModelMemberAnalysisDisplayDateTime
    {
        [Category("eXpand.PivotChart")]
        PivotGroupInterval PivotGroupInterval { get; set; }
    }
    public class AnalysisDisplayDateTimeViewController : AnalysisViewControllerBase,IModelExtender {



        #region IModelExtender Members

        void IModelExtender.ExtendModelInterfaces(ModelInterfaceExtenders extenders)
        {
            extenders.Add<IModelMember,IModelMemberAnalysisDisplayDateTime>();
        }

        #endregion
    }
}