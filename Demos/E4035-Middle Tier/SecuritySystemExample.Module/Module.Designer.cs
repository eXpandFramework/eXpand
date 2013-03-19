// Developer Express Code Central Example:
// How to: Implement middle tier security with the .NET Remoting service
// 
// The complete description is available in the Middle Tier Security - .NET
// Remoting Service (http://documentation.devexpress.com/#xaf/CustomDocument3438)
// topic.
// 
// You can find sample updates and versions for different programming languages here:
// http://www.devexpress.com/example=E4035

using Xpand.ExpressApp.AdditionalViewControlsProvider.Win;
using Xpand.ExpressApp.Chart.Win;
using Xpand.ExpressApp.ConditionalDetailViews;
using Xpand.ExpressApp.ImportWizard.Win;
using Xpand.ExpressApp.MasterDetail.Win;
using Xpand.ExpressApp.ModelDifference.Win;
using Xpand.ExpressApp.PivotGrid.Win;
using Xpand.ExpressApp.Reports.Win;
using Xpand.ExpressApp.Scheduler.Win;
using Xpand.ExpressApp.Security.Win;
using Xpand.ExpressApp.StateMachine;
using Xpand.ExpressApp.TreeListEditors.Win;
using Xpand.ExpressApp.Validation.Win;
using Xpand.ExpressApp.ViewVariants;
using Xpand.ExpressApp.WizardUI.Win;

namespace SecuritySystemExample.Module {
    partial class SecuritySystemExampleModule {
        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing) {
            if (disposing && (components != null)) {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent() {
            // 
            // SecuritySystemExampleModule
            // 
            this.RequiredModuleTypes.Add(typeof(DevExpress.ExpressApp.SystemModule.SystemModule));
            RequiredModuleTypes.Add(typeof(AdditionalViewControlsProviderWindowsFormsModule));
            RequiredModuleTypes.Add(typeof(XpandChartWinModule));
            RequiredModuleTypes.Add(typeof(ConditionalDetailViewModule));
            RequiredModuleTypes.Add(typeof(ImportWizardWindowsFormsModule));
            RequiredModuleTypes.Add(typeof(MasterDetailWindowsModule));
            RequiredModuleTypes.Add(typeof(XpandPivotGridWinModule));
            RequiredModuleTypes.Add(typeof(XpandReportsWindowsFormsModule));
            RequiredModuleTypes.Add(typeof(XpandSchedulerWindowsFormsModule));
            RequiredModuleTypes.Add(typeof(XpandSecurityWinModule));
            RequiredModuleTypes.Add(typeof(XpandStateMachineModule));
            RequiredModuleTypes.Add(typeof(XpandTreeListEditorsWinModule));
            RequiredModuleTypes.Add(typeof(XpandValidationWinModule));
            RequiredModuleTypes.Add(typeof(XpandViewVariantsModule));
            RequiredModuleTypes.Add(typeof(WizardUIWindowsFormsModule));
            RequiredModuleTypes.Add(typeof(Xpand.ExpressApp.Dashboard.Win.DashboardWindowsFormsModule));
            RequiredModuleTypes.Add(typeof(ModelDifferenceWindowsFormsModule));
            RequiredModuleTypes.Add(typeof(Xpand.ExpressApp.Win.SystemModule.XpandSystemWindowsFormsModule));
        }

        #endregion
    }
}
