namespace Xpand.Docs.Module {
    partial class DocsModule {
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
            // DocsModule
            // 
            this.RequiredModuleTypes.Add(typeof(DevExpress.ExpressApp.SystemModule.SystemModule));
            this.RequiredModuleTypes.Add(typeof(Xpand.ExpressApp.SystemModule.XpandSystemModule));
            this.RequiredModuleTypes.Add(typeof(Xpand.ExpressApp.AuditTrail.XpandAuditTrailModule));
            this.RequiredModuleTypes.Add(typeof(Xpand.ExpressApp.Email.EmailModule));
            this.RequiredModuleTypes.Add(typeof(Xpand.ExpressApp.ModelArtifactState.ModelArtifactStateModule));
            this.RequiredModuleTypes.Add(typeof(Xpand.ExpressApp.Security.XpandSecurityModule));
            this.RequiredModuleTypes.Add(typeof(Xpand.ExpressApp.StateMachine.XpandStateMachineModule));
            this.RequiredModuleTypes.Add(typeof(Xpand.ExpressApp.ViewVariants.XpandViewVariantsModule));
            this.RequiredModuleTypes.Add(typeof(Xpand.ExpressApp.Workflow.XpandWorkFlowModule));
            this.RequiredModuleTypes.Add(typeof(Xpand.ExpressApp.WorldCreator.DBMapper.WorldCreatorDBMapperModule));
            this.RequiredModuleTypes.Add(typeof(Xpand.ExpressApp.AuditTrail.XpandAuditTrailModule));
        }

        #endregion
    }
}
