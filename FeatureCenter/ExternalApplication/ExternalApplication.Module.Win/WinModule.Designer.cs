
using Xpand.ExpressApp.ModelDifference;
using Xpand.ExpressApp.ModelDifference.Win;
using Xpand.ExpressApp.SystemModule;
using Xpand.ExpressApp.Win.SystemModule;

namespace ExternalApplication.Module.Win
{
    partial class ExternalApplicationWindowsFormsModule
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
            // ExternalApplicationWindowsFormsModule
            // 
            AdditionalBusinessClassAssemblies.Add(typeof(Xpand.Persistent.BaseImpl.ImportExport.ClassInfoGraphNode).Assembly);
            this.RequiredModuleTypes.Add(typeof(DevExpress.ExpressApp.Win.SystemModule.SystemWindowsFormsModule));
            this.RequiredModuleTypes.Add(typeof(ModelDifferenceModule));
            this.RequiredModuleTypes.Add(typeof(ModelDifferenceWindowsFormsModule));
            this.RequiredModuleTypes.Add(typeof(XpandSystemModule));
            this.RequiredModuleTypes.Add(typeof(XpandSystemWindowsFormsModule));
            this.RequiredModuleTypes.Add(typeof(Xpand.ExpressApp.Validation.XpandValidationModule));
        }

        #endregion
    }
}
