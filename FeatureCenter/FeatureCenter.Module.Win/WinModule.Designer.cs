using DevExpress.ExpressApp.ScriptRecorder.Win;
using eXpand.ExpressApp.MemberLevelSecurity.Win;

namespace FeatureCenter.Module.Win
{
    partial class FeatureCenterWindowsFormsModule
    {
        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            // 
            // FeatureCenterWindowsFormsModule
            // 
            this.RequiredModuleTypes.Add(typeof(FeatureCenter.Module.FeatureCenterModule));
            this.RequiredModuleTypes.Add(typeof(DevExpress.ExpressApp.Win.SystemModule.SystemWindowsFormsModule));
            this.RequiredModuleTypes.Add(typeof(DevExpress.ExpressApp.ConditionalEditorState.Win.ConditionalEditorStateWindowsFormsModule));
            this.RequiredModuleTypes.Add(typeof(DevExpress.ExpressApp.FileAttachments.Win.FileAttachmentsWindowsFormsModule));
            this.RequiredModuleTypes.Add(typeof(DevExpress.ExpressApp.HtmlPropertyEditor.Win.HtmlPropertyEditorWindowsFormsModule));
            this.RequiredModuleTypes.Add(typeof(DevExpress.ExpressApp.PivotChart.Win.PivotChartWindowsFormsModule));
            this.RequiredModuleTypes.Add(typeof(DevExpress.ExpressApp.Printing.Win.PrintingWindowsFormsModule));
            this.RequiredModuleTypes.Add(typeof(DevExpress.ExpressApp.Reports.Win.ReportsWindowsFormsModule));
            this.RequiredModuleTypes.Add(typeof(DevExpress.ExpressApp.Scheduler.Win.SchedulerWindowsFormsModule));
            this.RequiredModuleTypes.Add(typeof(DevExpress.ExpressApp.TreeListEditors.Win.TreeListEditorsWindowsFormsModule));
            this.RequiredModuleTypes.Add(typeof(DevExpress.ExpressApp.Validation.Win.ValidationWindowsFormsModule));
            this.RequiredModuleTypes.Add(typeof(eXpand.ExpressApp.SystemModule.eXpandSystemModule));
            this.RequiredModuleTypes.Add(typeof(eXpand.ExpressApp.ModelArtifactState.Win.ModelArtifactStateWindowsFormsModule));
            this.RequiredModuleTypes.Add(typeof(eXpand.ExpressApp.ModelDifference.Win.ModelDifferenceWindowsFormsModule));
            this.RequiredModuleTypes.Add(typeof(eXpand.ExpressApp.TreeListEditors.Win.eXpandTreeListEditorsWinModule));
            this.RequiredModuleTypes.Add(typeof(eXpand.ExpressApp.ViewVariants.Win.eXpandViewVariantsWin));
            this.RequiredModuleTypes.Add(typeof(eXpand.ExpressApp.Win.SystemModule.eXpandSystemWindowsFormsModule));
            this.RequiredModuleTypes.Add(typeof(eXpand.ExpressApp.WizardUI.Win.WizardUIWindowsFormsModule));
            this.RequiredModuleTypes.Add(typeof(eXpand.ExpressApp.WorldCreator.Win.WorldCreatorWinModule));
            this.RequiredModuleTypes.Add(typeof(eXpand.ExpressApp.IO.Win.IOWinModule));
            this.RequiredModuleTypes.Add(typeof(eXpand.ExpressApp.ExceptionHandling.Win.ExceptionHandlingWinModule));
            this.RequiredModuleTypes.Add(typeof(eXpand.ExpressApp.FilterDataStore.Win.FilterDataStoreWindowsFormsModule));
            this.RequiredModuleTypes.Add(typeof(eXpand.ExpressApp.AdditionalViewControlsProvider.Win.AdditionalViewControlsProviderWindowsFormsModule));
            this.RequiredModuleTypes.Add(typeof(eXpand.ExpressApp.PivotChart.Win.PivotChartWinModule));
            this.RequiredModuleTypes.Add(typeof(eXpand.ExpressApp.MasterDetail.Win.MasterDetailWindowsModule));
            this.RequiredModuleTypes.Add(typeof(ScriptRecorderWindowsFormsModule));
            this.RequiredModuleTypes.Add(typeof(MemberLevelSecurityModuleWin));
        }

        #endregion
    }
}
