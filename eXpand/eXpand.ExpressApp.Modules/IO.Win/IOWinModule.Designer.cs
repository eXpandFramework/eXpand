using DevExpress.ExpressApp.TreeListEditors.Win;
using eXpand.ExpressApp.TreeListEditors.Win;

namespace eXpand.ExpressApp.IO.Win {
    partial class IOWinModule
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
            this.RequiredModuleTypes.Add(typeof(IOModule));
            this.RequiredModuleTypes.Add(typeof(TreeListEditorsWindowsFormsModule));
            this.RequiredModuleTypes.Add(typeof(eXpandTreeListEditorsWin));
        }

        #endregion
    }
}