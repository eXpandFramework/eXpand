namespace MainDemo.Module.Controllers
{
	partial class ChooseTemplateController
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
		  this.components = new System.ComponentModel.Container();
		  this.ChooseTemplateAction = new DevExpress.ExpressApp.Actions.SingleChoiceAction(this.components);
		  // 
		  // ClearFieldsAction
		  // 
		  this.ChooseTemplateAction.Caption = "Page Template";
		  this.ChooseTemplateAction.PaintStyle = DevExpress.ExpressApp.Templates.ActionItemPaintStyle.Image;
		  this.ChooseTemplateAction.ItemType = DevExpress.ExpressApp.Actions.SingleChoiceActionItemType.ItemIsMode;
		  this.ChooseTemplateAction.Id = "ChooseTemplateAction";
		  this.ChooseTemplateAction.Category = "Appearance";
		  this.ChooseTemplateAction.Execute += new DevExpress.ExpressApp.Actions.SingleChoiceActionExecuteEventHandler(ChooseTemplateAction_Execute);
		  // 
		  // ClearFieldsController
		  // 
		  this.Activated += new System.EventHandler(ChooseTemplateController_Activated);
		  this.Deactivated += new System.EventHandler(ChooseTemplateController_Deactivated);
	  }


      #endregion

		private DevExpress.ExpressApp.Actions.SingleChoiceAction ChooseTemplateAction;

   }
}
