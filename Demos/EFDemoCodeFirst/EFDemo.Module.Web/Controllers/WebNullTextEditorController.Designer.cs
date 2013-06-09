namespace EFDemo.Module.Web.Controllers {
   partial class WebNullTextEditorController {
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
          // WebNullTextEditorController
         // 
         this.TargetObjectType = typeof(EFDemo.Module.Data.Contact);
         this.TargetViewType = DevExpress.ExpressApp.ViewType.DetailView;
         this.Activated += new System.EventHandler(this.WebNullTextEditorController_Activated);

      }

      #endregion
   }
}
