using DevExpress.ExpressApp.Validation;
using eXpand.ExpressApp.Security;
using eXpand.ExpressApp.SystemModule;
using eXpand.ExpressApp.Validation;

namespace eXpand.ExpressApp.ModelArtifactState{
    partial class ModelArtifactStateModule
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
         
            RequiredModuleTypes.Add(typeof(ValidationModule));
            RequiredModuleTypes.Add(typeof(eXpandSecurityModule));
            RequiredModuleTypes.Add(typeof(eXpandValidationModule));
            RequiredModuleTypes.Add(typeof(eXpandSystemModule));
        }

        #endregion
    }
}