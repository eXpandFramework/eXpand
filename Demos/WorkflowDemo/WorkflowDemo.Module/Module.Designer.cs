using Xpand.ExpressApp.ConditionalObjectView;
using Xpand.ExpressApp.WorldCreator.DBMapper;

namespace WorkflowDemo.Module {
	partial class WorkflowDemoModule {
		/// <summary> 
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary> 
		/// Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing) {
			if(disposing && (components != null)) {
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
            this.RequiredModuleTypes.Add(typeof(DevExpress.ExpressApp.Security.SecurityModule));
            this.RequiredModuleTypes.Add(typeof(Xpand.ExpressApp.SystemModule.XpandSystemModule));
            this.RequiredModuleTypes.Add(typeof(Xpand.ExpressApp.ModelArtifactState.ModelArtifactStateModule));
            this.RequiredModuleTypes.Add(typeof(Xpand.ExpressApp.ModelDifference.ModelDifferenceModule));
            this.RequiredModuleTypes.Add(typeof(Xpand.ExpressApp.Security.XpandSecurityModule));
            this.RequiredModuleTypes.Add(typeof(Xpand.ExpressApp.Validation.XpandValidationModule));
            this.RequiredModuleTypes.Add(typeof(Xpand.ExpressApp.ViewVariants.XpandViewVariantsModule));
            this.RequiredModuleTypes.Add(typeof(Xpand.ExpressApp.WorldCreator.WorldCreatorModule));
            this.RequiredModuleTypes.Add(typeof(Xpand.ExpressApp.IO.IOModule));
            this.RequiredModuleTypes.Add(typeof(Xpand.ExpressApp.Validation.XpandValidationModule));
            this.RequiredModuleTypes.Add(typeof(Xpand.ExpressApp.PivotChart.XpandPivotChartModule));
            this.RequiredModuleTypes.Add(typeof(Xpand.ExpressApp.TreeListEditors.XpandTreeListEditorsModule));
            this.RequiredModuleTypes.Add(typeof(Xpand.ExpressApp.FilterDataStore.FilterDataStoreModule));
            this.RequiredModuleTypes.Add(typeof(Xpand.ExpressApp.AdditionalViewControlsProvider.AdditionalViewControlsModule));
            this.RequiredModuleTypes.Add(typeof(WorldCreatorDBMapperModule));
            this.RequiredModuleTypes.Add(typeof(ConditionalObjectViewModule));
            this.RequiredModuleTypes.Add(typeof(Xpand.ExpressApp.JobScheduler.JobSchedulerModule));
            this.RequiredModuleTypes.Add(typeof(Xpand.ExpressApp.JobScheduler.Jobs.JobSchedulerJobsModule));
            this.RequiredModuleTypes.Add(typeof(Xpand.ExpressApp.ImportWizard.ImportWizardModule));
            this.RequiredModuleTypes.Add(typeof(Xpand.ExpressApp.Workflow.XpandWorkFlowModule));
            this.RequiredModuleTypes.Add(typeof(Xpand.ExpressApp.StateMachine.XpandStateMachineModule));
            this.RequiredModuleTypes.Add(typeof(Xpand.ExpressApp.Reports.XpandReportsModule));
            RequiredModuleTypes.Add(typeof(Xpand.ExpressApp.Validation.XpandValidationModule));
            RequiredModuleTypes.Add(typeof(Xpand.ExpressApp.ModelAdaptor.ModelAdaptorModule));
        }

		#endregion
	}
}

