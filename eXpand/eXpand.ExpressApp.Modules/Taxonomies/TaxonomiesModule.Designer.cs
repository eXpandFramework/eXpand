using eXpand.ExpressApp.SystemModule;

namespace eXpand.ExpressApp.Taxonomies{
    partial class TaxonomiesModule {
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
            // TaxonomiesModule
            // 
            this.AdditionalBusinessClasses.Add(typeof(eXpand.Persistent.Base.Taxonomies.BaseTaxonomy));
            this.AdditionalBusinessClasses.Add(typeof(eXpand.Persistent.Base.Taxonomies.BaseTerm));
            this.RequiredModuleTypes.Add(typeof(eXpand.ExpressApp.SystemModule.eXpandSystemModule));
            this.RequiredModuleTypes.Add(typeof(DevExpress.ExpressApp.ViewVariantsModule.ViewVariantsModule));

        }
        #endregion
    }
}