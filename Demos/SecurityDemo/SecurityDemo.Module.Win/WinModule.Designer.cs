using Xpand.ExpressApp.Win.SystemModule;

namespace SecurityDemo.Module.Win {
    partial class SecurityDemoWindowsFormsModule {
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
            // SecurityDemoWindowsFormsModule
            // 
            this.RequiredModuleTypes.Add(typeof(DevExpress.ExpressApp.TreeListEditors.TreeListEditorsModuleBase));
            this.RequiredModuleTypes.Add(typeof(DevExpress.ExpressApp.TreeListEditors.Win.TreeListEditorsWindowsFormsModule));
            this.RequiredModuleTypes.Add(typeof(DevExpress.ExpressApp.Validation.ValidationModule));
            this.RequiredModuleTypes.Add(typeof(DevExpress.ExpressApp.Validation.Win.ValidationWindowsFormsModule));
            this.RequiredModuleTypes.Add(typeof(DevExpress.ExpressApp.Win.SystemModule.SystemWindowsFormsModule));
            this.RequiredModuleTypes.Add(typeof(XpandSystemWindowsFormsModule));
            this.RequiredModuleTypes.Add(typeof(Xpand.ExpressApp.AdditionalViewControlsProvider.Win.AdditionalViewControlsProviderWindowsFormsModule));
            this.RequiredModuleTypes.Add(typeof(Xpand.ExpressApp.FilterDataStore.Win.FilterDataStoreWindowsFormsModule));
            this.RequiredModuleTypes.Add(typeof(Xpand.ExpressApp.ImportWizard.Win.ImportWizardWindowsFormsModule));
            this.RequiredModuleTypes.Add(typeof(Xpand.ExpressApp.IO.Win.IOWinModule));
            this.RequiredModuleTypes.Add(typeof(Xpand.ExpressApp.MasterDetail.Win.MasterDetailWindowsModule));
            this.RequiredModuleTypes.Add(typeof(Xpand.ExpressApp.PivotChart.Win.XpandPivotChartWinModule));
            this.RequiredModuleTypes.Add(typeof(Xpand.ExpressApp.TreeListEditors.Win.XpandTreeListEditorsWinModule));
            this.RequiredModuleTypes.Add(typeof(Xpand.ExpressApp.Validation.Win.XpandValidationWinModule));
            this.RequiredModuleTypes.Add(typeof(Xpand.ExpressApp.WizardUI.Win.WizardUIWindowsFormsModule));
            this.RequiredModuleTypes.Add(typeof(Xpand.ExpressApp.WorldCreator.Win.WorldCreatorWinModule));
            this.RequiredModuleTypes.Add(typeof(Xpand.ExpressApp.ModelDifference.Win.ModelDifferenceWindowsFormsModule));
        }

        #endregion
    }
}
