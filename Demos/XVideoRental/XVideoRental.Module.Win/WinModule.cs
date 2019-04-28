using System;
using System.Collections.Generic;
using System.ComponentModel;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.ReportsV2;
using DevExpress.ExpressApp.Updating;
using XVideoRental.Module.Win.BusinessObjects;
using XVideoRental.Module.Win.BusinessObjects.Movie;
using XVideoRental.Module.Win.Reports;

namespace XVideoRental.Module.Win {
    [ToolboxItemFilter("Xaf.Platform.Win")]
    public sealed partial class XVideoRentalWindowsFormsModule : ModuleBase {
        public XVideoRentalWindowsFormsModule() {
            InitializeComponent();
            RequiredModuleTypes.Add(typeof(Xpand.XAF.Modules.CloneModelView.CloneModelViewModule));
        }

        public override IEnumerable<ModuleUpdater> GetModuleUpdaters(IObjectSpace objectSpace, Version versionFromDB) {
            ModuleUpdater updater = new DatabaseUpdate.Updater(objectSpace, versionFromDB);
            PredefinedReportsUpdater predefinedReportsUpdater =
                new PredefinedReportsUpdater(Application, objectSpace, versionFromDB);
            predefinedReportsUpdater.AddPredefinedReport<ActiveCustomers>("Active Customers", typeof(Customer));
            predefinedReportsUpdater.AddPredefinedReport<CustomerCards>("Customer Cards", typeof(Customer));
            predefinedReportsUpdater.AddPredefinedReport<MostProfitableGenres>("Most Profitable Genres", typeof(Movie));
            predefinedReportsUpdater.AddPredefinedReport<MovieInvetory>("Movie Invetory", typeof(MovieItem));
            predefinedReportsUpdater.AddPredefinedReport<MovieRentalsByCustomer>("Movie Rentals By Customer", typeof(Customer));
            predefinedReportsUpdater.AddPredefinedReport<TopMovieRentals>("Top Movie Rentals", typeof(Movie));
            return new[] { updater, predefinedReportsUpdater };
        }

    }

}
