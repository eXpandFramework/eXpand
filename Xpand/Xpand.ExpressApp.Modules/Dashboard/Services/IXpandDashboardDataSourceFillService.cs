namespace Xpand.ExpressApp.Dashboard.Services{
    public interface IXpandDashboardDataSourceFillService{
        XpandDashboardDataSourceFillService FillService{ get; }
        int TopReturnedRecords{ get; set; }
        bool ShowPersistentMembersOnly{ get; set; }
    }
}