using Xpand.ExpressApp.AdditionalViewControlsProvider.Web;
using Xpand.ExpressApp.ExceptionHandling.Web;
using Xpand.ExpressApp.FilterDataStore.Web;
using Xpand.ExpressApp.ModelDifference.Web;
using Xpand.ExpressApp.NCarousel.Web;
using Xpand.ExpressApp.PivotChart.Web;
using Xpand.ExpressApp.Thumbnail.Web;
using Xpand.ExpressApp.TreeListEditors.Web;
using Xpand.ExpressApp.Web.SystemModule;
using Xpand.ExpressApp.WorldCreator.Web;

namespace $projectsuffix$.Module.Web {
    partial class $projectsuffix$AspNetModule {
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
            // $projectsuffix$AspNetModule
            // 
            this.RequiredModuleTypes.Add(typeof($projectsuffix$.Module.$projectsuffix$Module));
            this.RequiredModuleTypes.Add(typeof(DevExpress.ExpressApp.Web.SystemModule.SystemAspNetModule));
            this.RequiredModuleTypes.Add(typeof(DevExpress.ExpressApp.FileAttachments.Web.FileAttachmentsAspNetModule));

            this.RequiredModuleTypes.Add(typeof(DevExpress.ExpressApp.PivotChart.Web.PivotChartAspNetModule));
            this.RequiredModuleTypes.Add(typeof(DevExpress.ExpressApp.Reports.Web.ReportsAspNetModule));

            this.RequiredModuleTypes.Add(typeof(DevExpress.ExpressApp.TreeListEditors.Web.TreeListEditorsAspNetModule));
            this.RequiredModuleTypes.Add(typeof(XpandSystemAspNetModule));
            this.RequiredModuleTypes.Add(typeof(ModelDifferenceAspNetModule));
            this.RequiredModuleTypes.Add(typeof(WorldCreatorWebModule));
            this.RequiredModuleTypes.Add(typeof(ExceptionHandlingWebModule));
            this.RequiredModuleTypes.Add(typeof(AdditionalViewControlsProviderAspNetModule));
            this.RequiredModuleTypes.Add(typeof(XpandPivotChartAspNetModule));
            this.RequiredModuleTypes.Add(typeof(NCarouselWebModule));
            this.RequiredModuleTypes.Add(typeof(ThumbnailWebModule));
            this.RequiredModuleTypes.Add(typeof(FilterDataStoreAspNetModule));
            this.RequiredModuleTypes.Add(typeof(XpandTreeListEditorsAspNetModule));
            this.RequiredModuleTypes.Add(typeof(Xpand.ExpressApp.IO.Web.IOAspNetModule));
            this.RequiredModuleTypes.Add(typeof(Xpand.ExpressApp.Validation.Web.XpandValidationWebModule));

        }

        #endregion
    }
}
