using DevExpress.ExpressApp.FileAttachments.Web;
using Xpand.ExpressApp.AdditionalViewControlsProvider.Web;
using Xpand.ExpressApp.FilterDataStore.Web;
using Xpand.ExpressApp.TreeListEditors.Web;
using Xpand.ExpressApp.Web.SystemModule;
using Xpand.ExpressApp.WorldCreator.Web;

namespace FeatureCenter.Module.Web {
    partial class FeatureCenterAspNetModule {
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
            // FeatureCenterAspNetModule
            // 
            this.RequiredModuleTypes.Add(typeof(FeatureCenter.Module.FeatureCenterModule));
            this.RequiredModuleTypes.Add(typeof(DevExpress.ExpressApp.HtmlPropertyEditor.Web.HtmlPropertyEditorAspNetModule));
            this.RequiredModuleTypes.Add(typeof(DevExpress.ExpressApp.PivotChart.Web.PivotChartAspNetModule));
            this.RequiredModuleTypes.Add(typeof(DevExpress.ExpressApp.Reports.Web.ReportsAspNetModule));
            this.RequiredModuleTypes.Add(typeof(DevExpress.ExpressApp.TreeListEditors.Web.TreeListEditorsAspNetModule));
            this.RequiredModuleTypes.Add(typeof(DevExpress.ExpressApp.PivotChart.Web.PivotChartAspNetModule));

            this.RequiredModuleTypes.Add(typeof(FeatureCenter.Module.FeatureCenterModule));
            this.RequiredModuleTypes.Add(typeof(DevExpress.ExpressApp.Web.SystemModule.SystemAspNetModule));
            this.RequiredModuleTypes.Add(typeof(FileAttachmentsAspNetModule));
            this.RequiredModuleTypes.Add(typeof(DevExpress.ExpressApp.Scheduler.Web.SchedulerAspNetModule));
            this.RequiredModuleTypes.Add(typeof(AdditionalViewControlsProviderAspNetModule));
            this.RequiredModuleTypes.Add(typeof(Xpand.ExpressApp.PivotChart.Web.XpandPivotChartAspNetModule));
            this.RequiredModuleTypes.Add(typeof(Xpand.ExpressApp.ModelDifference.Web.ModelDifferenceAspNetModule));
            this.RequiredModuleTypes.Add(typeof(Xpand.ExpressApp.Thumbnail.Web.ThumbnailWebModule));
            this.RequiredModuleTypes.Add(typeof(Xpand.ExpressApp.NCarousel.Web.NCarouselWebModule));
            this.RequiredModuleTypes.Add(typeof(XpandSystemAspNetModule));
            this.RequiredModuleTypes.Add(typeof(XpandTreeListEditorsAspNetModule));
            this.RequiredModuleTypes.Add(typeof(WorldCreatorWebModule));
            this.RequiredModuleTypes.Add(typeof(Xpand.ExpressApp.IO.Web.IOAspNetModule));
            this.RequiredModuleTypes.Add(typeof(Xpand.ExpressApp.WorldCreator.Web.WorldCreatorWebModule));
            this.RequiredModuleTypes.Add(typeof(FilterDataStoreAspNetModule));

        }

        #endregion
    }
}
