using DevExpress.ExpressApp.Validation;
using Xpand.ExpressApp.Security;
using Xpand.ExpressApp.SystemModule;
using Xpand.ExpressApp.Validation;

namespace Xpand.ExpressApp.ModelArtifactState.Win{
    partial class ModelArtifactStateWindowsFormsModule
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
            RequiredModuleTypes.Add(typeof(XpandSecurityModule));
            RequiredModuleTypes.Add(typeof(XpandValidationModule));
            RequiredModuleTypes.Add(typeof(XpandSystemModule));
            RequiredModuleTypes.Add(typeof(Logic.Win.LogicWindowsModule));
            
        }

        #endregion
    }
}