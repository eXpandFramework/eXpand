// Developer Express Code Central Example:
// How to: Implement middle tier security with the .NET Remoting service
// 
// The complete description is available in the Middle Tier Security - .NET
// Remoting Service (http://documentation.devexpress.com/#xaf/CustomDocument3438)
// topic.
// 
// You can find sample updates and versions for different programming languages here:
// http://www.devexpress.com/example=E4035

using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Security;
using DevExpress.ExpressApp.Web.SystemModule;
using DevExpress.ExpressApp.Win.SystemModule;
using DevExpress.ExpressApp.Xpo;
using SecuritySystemExample.Module;
using Xpand.ExpressApp.AdditionalViewControlsProvider.Win;
using Xpand.ExpressApp.MiddleTier;
using Xpand.ExpressApp.ModelDifference.Win;

namespace ConsoleApplicationServer {
    public class ConsoleApplicationServerServerApplication : XpandServerApplication {
        public ConsoleApplicationServerServerApplication(SecurityStrategyComplex securityStrategyComplex)
            : base(securityStrategyComplex) {
            // Change the ServerApplication.ApplicationName property value. It should be the same as your client application name. 
            ApplicationName = "SecuritySystemExample";

            // Add your client application's modules to the ServerApplication.Modules collection here. 
            Modules.Add(new AdditionalViewControlsProviderWindowsFormsModule());
            Modules.Add(new Xpand.ExpressApp.Chart.Win.XpandChartWinModule());
            Modules.Add(new Xpand.ExpressApp.ConditionalObjectView.ConditionalObjectViewModule());
            Modules.Add(new Xpand.ExpressApp.ImportWizard.Win.ImportWizardWindowsFormsModule());
            Modules.Add(new Xpand.ExpressApp.MasterDetail.Win.MasterDetailWindowsModule());
            Modules.Add(new Xpand.ExpressApp.PivotGrid.Win.XpandPivotGridWinModule());
            Modules.Add(new Xpand.ExpressApp.Reports.Win.XpandReportsWindowsFormsModule());
            Modules.Add(new Xpand.ExpressApp.Scheduler.Win.XpandSchedulerWindowsFormsModule());
            Modules.Add(new Xpand.ExpressApp.Security.Win.XpandSecurityWinModule());
            Modules.Add(new Xpand.ExpressApp.StateMachine.XpandStateMachineModule());
            Modules.Add(new Xpand.ExpressApp.TreeListEditors.Win.XpandTreeListEditorsWinModule());
            Modules.Add(new Xpand.ExpressApp.Validation.Win.XpandValidationWinModule());
            Modules.Add(new Xpand.ExpressApp.ViewVariants.XpandViewVariantsModule());
            Modules.Add(new Xpand.ExpressApp.WizardUI.Win.WizardUIWindowsFormsModule());
            Modules.Add(new ModelDifferenceWindowsFormsModule());
            //            Modules.Add(new Xpand.ExpressApp.Win.SystemModule.XpandSystemWindowsFormsModule());
            Modules.Add(new SecuritySystemExampleModule());
        }

        protected override void OnDatabaseVersionMismatch(DatabaseVersionMismatchEventArgs args) {
            args.Updater.Update();
            args.Handled = true;
        }

        protected override void CreateDefaultObjectSpaceProvider(CreateCustomObjectSpaceProviderEventArgs args) {
            args.ObjectSpaceProvider = new XPObjectSpaceProvider(args.ConnectionString, args.Connection);
        }
    }
}