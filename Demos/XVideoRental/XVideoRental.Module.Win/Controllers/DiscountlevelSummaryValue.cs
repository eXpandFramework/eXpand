using DevExpress.XtraPivotGrid;
using XVideoRental.Module.Win.BusinessObjects;
using Xpand.ExpressApp.PivotGrid.Win.Model;

namespace XVideoRental.Module.Win.Controllers {
    public class DiscountlevelSummaryValue : IPivotCustomSummaryEvent {
        #region Implementation of ICustomSummaryValue
        public void Calculate(PivotGridCustomSummaryEventArgs args) {
            var summary = (decimal)args.SummaryValue.Summary;
            args.CustomValue = new DiscountLevelCalculator().Calculate(summary);
        }
        #endregion
    }

}
