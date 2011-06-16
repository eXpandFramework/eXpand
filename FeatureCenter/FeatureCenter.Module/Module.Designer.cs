using DevExpress.ExpressApp.ScriptRecorder;
using Xpand.ExpressApp.ConditionalDetailViews;
using Xpand.ExpressApp.ImportWizard;
using Xpand.ExpressApp.JobScheduler;
using Xpand.ExpressApp.MemberLevelSecurity;
using Xpand.ExpressApp.WorldCreator.SqlDBMapper;

namespace FeatureCenter.Module {
    partial class FeatureCenterModule {
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
            // FeatureCenterModule
            // 
            // 
            // Solution1Module
            // 
            AdditionalBusinessClassAssemblies.Add(System.Reflection.Assembly.GetAssembly(typeof(global::DevExpress.Persistent.BaseImpl.Analysis)));
            AdditionalBusinessClassAssemblies.Add(System.Reflection.Assembly.GetAssembly(typeof(global::Xpand.Persistent.BaseImpl.Updater)));
            AdditionalBusinessClassAssemblies.Add(System.Reflection.Assembly.GetAssembly(typeof(global::Xpand.ExpressApp.JobScheduler.Jobs.ThresholdCalculation.ThresholdSeverity)));
            this.RequiredModuleTypes.Add(typeof(DevExpress.ExpressApp.SystemModule.SystemModule));
            this.RequiredModuleTypes.Add(typeof(DevExpress.ExpressApp.Objects.BusinessClassLibraryCustomizationModule));
            this.RequiredModuleTypes.Add(typeof(DevExpress.ExpressApp.CloneObject.CloneObjectModule));
            this.RequiredModuleTypes.Add(typeof(DevExpress.ExpressApp.Validation.ValidationModule));
            this.RequiredModuleTypes.Add(typeof(DevExpress.ExpressApp.ViewVariantsModule.ViewVariantsModule));
            this.RequiredModuleTypes.Add(typeof(DevExpress.ExpressApp.Kpi.KpiModule));
            this.RequiredModuleTypes.Add(typeof(Xpand.ExpressApp.SystemModule.XpandSystemModule));
            this.RequiredModuleTypes.Add(typeof(Xpand.ExpressApp.ModelArtifactState.ModelArtifactStateModule));
            this.RequiredModuleTypes.Add(typeof(Xpand.ExpressApp.ModelDifference.ModelDifferenceModule));
            this.RequiredModuleTypes.Add(typeof(Xpand.ExpressApp.Security.XpandSecurityModule));
            this.RequiredModuleTypes.Add(typeof(Xpand.ExpressApp.Validation.XpandValidationModule));
            this.RequiredModuleTypes.Add(typeof(Xpand.ExpressApp.ViewVariants.XpandViewVariantsModule));
            this.RequiredModuleTypes.Add(typeof(Xpand.ExpressApp.WorldCreator.WorldCreatorModule));
            this.RequiredModuleTypes.Add(typeof(Xpand.ExpressApp.IO.IOModule));
            this.RequiredModuleTypes.Add(typeof(Xpand.ExpressApp.Validation.XpandValidationModule));
            this.RequiredModuleTypes.Add(typeof(Xpand.ExpressApp.MasterDetail.MasterDetailModule));
            this.RequiredModuleTypes.Add(typeof(Xpand.ExpressApp.PivotChart.XpandPivotChartModule));
            this.RequiredModuleTypes.Add(typeof(Xpand.ExpressApp.FilterDataStore.FilterDataStoreModule));
            this.RequiredModuleTypes.Add(typeof(Xpand.ExpressApp.AdditionalViewControlsProvider.AdditionalViewControlsModule));
            this.RequiredModuleTypes.Add(typeof(ScriptRecorderModuleBase));
            this.RequiredModuleTypes.Add(typeof(WorldCreatorSqlDBMapperModule));
            this.RequiredModuleTypes.Add(typeof(ConditionalDetailViewModule));
            this.RequiredModuleTypes.Add(typeof(MemberLevelSecurityModule));
            this.RequiredModuleTypes.Add(typeof(Xpand.ExpressApp.JobScheduler.JobSchedulerModule));
            this.RequiredModuleTypes.Add(typeof(Xpand.ExpressApp.JobScheduler.Jobs.JobSchedulerJobsModule));
            this.RequiredModuleTypes.Add(typeof(ImportWizardModule));
        }

        #endregion
    }
}
