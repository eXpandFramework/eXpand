using Xpand.ExpressApp.AdditionalViewControlsProvider;
using Xpand.ExpressApp.SystemModule;

namespace SecurityDemo.Module
{
    partial class SecurityDemoModule
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
            // SecurityDemoModule
            // 
            this.RequiredModuleTypes.Add(typeof(DevExpress.ExpressApp.SystemModule.SystemModule));
            this.RequiredModuleTypes.Add(typeof(XpandSystemModule));
            this.RequiredModuleTypes.Add(typeof(Xpand.ExpressApp.AdditionalViewControlsProvider.AdditionalViewControlsModule));
            this.RequiredModuleTypes.Add(typeof(Xpand.ExpressApp.IO.IOModule));
            this.RequiredModuleTypes.Add(typeof(Xpand.ExpressApp.ModelArtifactState.ModelArtifactStateModule));
            this.RequiredModuleTypes.Add(typeof(Xpand.ExpressApp.ModelDifference.ModelDifferenceModule));
            this.RequiredModuleTypes.Add(typeof(Xpand.ExpressApp.PivotChart.XpandPivotChartModule));
            this.RequiredModuleTypes.Add(typeof(Xpand.ExpressApp.Security.XpandSecurityModule));
            this.RequiredModuleTypes.Add(typeof(Xpand.ExpressApp.StateMachine.XpandStateMachineModule));
            this.RequiredModuleTypes.Add(typeof(Xpand.ExpressApp.TreeListEditors.XpandTreeListEditorsModule));
            this.RequiredModuleTypes.Add(typeof(Xpand.ExpressApp.Validation.XpandValidationModule));
            this.RequiredModuleTypes.Add(typeof(Xpand.ExpressApp.ViewVariants.XpandViewVariantsModule));
            this.RequiredModuleTypes.Add(typeof(Xpand.ExpressApp.Workflow.XpandWorkFlowModule));
            
        }

        #endregion
    }
}
