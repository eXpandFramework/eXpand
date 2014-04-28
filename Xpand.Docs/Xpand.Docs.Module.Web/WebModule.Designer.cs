namespace Xpand.Docs.Module.Web {
    partial class DocsAspNetModule {
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
            // DocsAspNetModule
            // 
            this.RequiredModuleTypes.Add(typeof(Xpand.Docs.Module.DocsModule));
            this.RequiredModuleTypes.Add(typeof(DevExpress.ExpressApp.Web.SystemModule.SystemAspNetModule));
            this.RequiredModuleTypes.Add(typeof(Xpand.ExpressApp.Web.SystemModule.XpandSystemAspNetModule));
            this.RequiredModuleTypes.Add(typeof(Xpand.ExpressApp.Security.Web.XpandSecurityWebModule));
            this.RequiredModuleTypes.Add(typeof(Xpand.ExpressApp.HtmlPropertyEditor.Web.XpandHtmlPropertyEditorAspNetModule));
            this.RequiredModuleTypes.Add(typeof(Xpand.ExpressApp.AdditionalViewControlsProvider.Web.AdditionalViewControlsProviderAspNetModule));
            this.RequiredModuleTypes.Add(typeof(Xpand.ExpressApp.FileAttachment.Web.XpandFileAttachmentsWebModule));
            this.RequiredModuleTypes.Add(typeof(Xpand.ExpressApp.FilterDataStore.Web.FilterDataStoreAspNetModule));
            this.RequiredModuleTypes.Add(typeof(Xpand.ExpressApp.IO.Web.IOAspNetModule));
            this.RequiredModuleTypes.Add(typeof(Xpand.ExpressApp.ModelDifference.Web.ModelDifferenceAspNetModule));
            this.RequiredModuleTypes.Add(typeof(Xpand.ExpressApp.NCarousel.Web.NCarouselWebModule));
            this.RequiredModuleTypes.Add(typeof(Xpand.ExpressApp.Scheduler.Web.XpandSchedulerAspNetModule));
            this.RequiredModuleTypes.Add(typeof(Xpand.ExpressApp.TreeListEditors.Web.XpandTreeListEditorsAspNetModule));
            this.RequiredModuleTypes.Add(typeof(Xpand.ExpressApp.Validation.Web.XpandValidationWebModule));
            this.RequiredModuleTypes.Add(typeof(Xpand.ExpressApp.WorldCreator.Web.WorldCreatorWebModule));
            this.RequiredModuleTypes.Add(typeof(Xpand.ExpressApp.XtraDashboard.Web.XtraDashboardWebModule));
            this.RequiredModuleTypes.Add(typeof(Xpand.ExpressApp.MapView.Web.MapViewWebModule));


        }

        #endregion
    }
}
