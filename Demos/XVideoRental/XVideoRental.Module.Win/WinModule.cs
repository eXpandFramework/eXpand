using System;
using System.Collections.Generic;
using System.ComponentModel;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Chart.Win;
using DevExpress.ExpressApp.ConditionalAppearance;
using DevExpress.ExpressApp.PivotChart.Win;
using DevExpress.ExpressApp.PivotGrid.Win;
using DevExpress.ExpressApp.ReportsV2;
using DevExpress.ExpressApp.Scheduler.Win;
using DevExpress.ExpressApp.ScriptRecorder.Win;
using DevExpress.ExpressApp.TreeListEditors;
using DevExpress.ExpressApp.TreeListEditors.Win;
using DevExpress.ExpressApp.Updating;
using DevExpress.ExpressApp.ViewVariantsModule;
using DevExpress.ExpressApp.Win.SystemModule;
using Xpand.ExpressApp.Chart.Win;
using Xpand.ExpressApp.MasterDetail.Win;
using Xpand.ExpressApp.PivotGrid.Win;
using Xpand.ExpressApp.ReportsV2.Win;
using Xpand.ExpressApp.Scheduler.Win;
using Xpand.ExpressApp.Security.Win;
using Xpand.ExpressApp.SystemModule;
using Xpand.ExpressApp.Win.SystemModule;
using Xpand.ExpressApp.XtraDashboard.Win;
using Xpand.XAF.Modules.CloneModelView;
using Xpand.XAF.Modules.ModelMapper;
using XVideoRental.Module.Win.BusinessObjects;
using XVideoRental.Module.Win.BusinessObjects.Movie;
using XVideoRental.Module.Win.Reports;
using Updater = XVideoRental.Module.Win.DatabaseUpdate.Updater;

namespace XVideoRental.Module.Win {
    [ToolboxItemFilter("Xaf.Platform.Win")]
    public sealed class XVideoRentalWindowsFormsModule : ModuleBase {
        public XVideoRentalWindowsFormsModule() {

            RequiredModuleTypes.Add(typeof(SystemWindowsFormsModule));
            RequiredModuleTypes.Add(typeof(ScriptRecorderWindowsFormsModule));
            RequiredModuleTypes.Add(typeof(PivotChartWindowsFormsModule));
            RequiredModuleTypes.Add(typeof(PivotGridWindowsFormsModule));
            RequiredModuleTypes.Add(typeof(ChartWindowsFormsModule));
            RequiredModuleTypes.Add(typeof(SchedulerWindowsFormsModule));
            RequiredModuleTypes.Add(typeof(ReportsModuleV2));
            RequiredModuleTypes.Add(typeof(ConditionalAppearanceModule));
            RequiredModuleTypes.Add(typeof(ViewVariantsModule));
            RequiredModuleTypes.Add(typeof(XpandSystemModule));
            RequiredModuleTypes.Add(typeof(XpandSystemWindowsFormsModule));
            RequiredModuleTypes.Add(typeof(XpandPivotGridWinModule));
            RequiredModuleTypes.Add(typeof(XpandChartWinModule));
            RequiredModuleTypes.Add(typeof(MasterDetailWindowsModule));
            RequiredModuleTypes.Add(typeof(XpandSchedulerWindowsFormsModule));
            RequiredModuleTypes.Add(typeof(XpandSecurityWinModule));
            RequiredModuleTypes.Add(typeof(ReportsV2WinModule));
            RequiredModuleTypes.Add(typeof(DashboardWindowsFormsModule));
            RequiredModuleTypes.Add(typeof(CloneModelViewModule));
            RequiredModuleTypes.Add(typeof(ModelMapperModule));
            RequiredModuleTypes.Add(typeof(TreeListEditorsModuleBase));
            RequiredModuleTypes.Add(typeof(TreeListEditorsWindowsFormsModule));
            
        }

        public override IEnumerable<ModuleUpdater> GetModuleUpdaters(IObjectSpace objectSpace, Version versionFromDB) {
            ModuleUpdater updater = new Updater(objectSpace, versionFromDB);
            var predefinedReportsUpdater =
                new PredefinedReportsUpdater(Application, objectSpace, versionFromDB);
            predefinedReportsUpdater.AddPredefinedReport<ActiveCustomers>("Active Customers", typeof(Customer));
            predefinedReportsUpdater.AddPredefinedReport<CustomerCards>("Customer Cards", typeof(Customer));
            predefinedReportsUpdater.AddPredefinedReport<MostProfitableGenres>("Most Profitable Genres", typeof(Movie));
            predefinedReportsUpdater.AddPredefinedReport<MovieInvetory>("Movie Invetory", typeof(MovieItem));
            predefinedReportsUpdater.AddPredefinedReport<MovieRentalsByCustomer>("Movie Rentals By Customer",
                typeof(Customer));
            predefinedReportsUpdater.AddPredefinedReport<TopMovieRentals>("Top Movie Rentals", typeof(Movie));
            return new[] {updater, predefinedReportsUpdater};
        }
    }
}