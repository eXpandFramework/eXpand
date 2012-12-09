using System;
using Common.Win.General.DashBoard;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Model;

namespace XVideoRental.Module.Win.Controllers.Common {
    [ModelAbstractClass]
    public interface IModelDashboardReportViewItemEx : IModelDashboardReportViewItem {

    }
    [ViewItem(typeof(IModelDashboardReportViewItem))]
    public class DashboardReportViewItem : global::Common.Win.General.DashBoard.DashboardReportViewItem {
        public DashboardReportViewItem(IModelDashboardReportViewItem model, Type objectType)
            : base(model, objectType) {
        }
    }
}
