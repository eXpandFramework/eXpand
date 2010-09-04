using DevExpress.ExpressApp.ConditionalEditorState.Win;
using DevExpress.ExpressApp.FileAttachments.Win;

namespace Xpand.ExpressApp.WorldCreator.Win {
    partial class WorldCreatorWinModule
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
            RequiredModuleTypes.Add(typeof(WorldCreatorModule));
            RequiredModuleTypes.Add(typeof(ConditionalEditorStateWindowsFormsModule));
            RequiredModuleTypes.Add(typeof(FileAttachmentsWindowsFormsModule));
        }

        #endregion
    }
}