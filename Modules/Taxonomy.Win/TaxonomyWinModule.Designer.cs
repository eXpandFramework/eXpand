
using eXpand.ExpressApp.Win.SystemModule;

namespace eXpand.ExpressApp.Taxonomy.Win {
    partial class TaxonomyWinModule {
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
            // TaxonomyWinModule
            // 
            this.RequiredModuleTypes.Add(typeof(eXpand.ExpressApp.Win.SystemModule.eXpandSystemWindowsFormsModule));
            this.RequiredModuleTypes.Add(typeof(eXpand.ExpressApp.Taxonomy.TaxonomyModule));
            this.RequiredModuleTypes.Add(typeof(DevExpress.ExpressApp.TreeListEditors.Win.TreeListEditorsWindowsFormsModule));

        }
        #endregion
    }
}
