namespace MainDemo.Module.Controllers
{
   partial class ClearContactTasksController
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
		  this.ClearTasksAction = new DevExpress.ExpressApp.Actions.SimpleAction(this.components);
		  // 
		  // ClearTasksAction
		  // 
		  this.ClearTasksAction.Caption = "Clear Tasks";
		  this.ClearTasksAction.ConfirmationMessage = "Are you sure you want to clear the Tasks list?";
		  this.ClearTasksAction.Id = "ClearTasksAction";
		  this.ClearTasksAction.ImageName = "Action_Clear";
		  this.ClearTasksAction.Execute += new DevExpress.ExpressApp.Actions.SimpleActionExecuteEventHandler(this.ClearTasksAction_Execute);
		  // 
		  // ClearTasksController
		  // 
          this.TargetViewNesting = DevExpress.ExpressApp.Nesting.Root;
		  this.TargetViewType = DevExpress.ExpressApp.ViewType.DetailView;
		  this.TargetObjectType = typeof(MainDemo.Module.BusinessObjects.Contact);
		  this.Activated += new System.EventHandler(this.ClearTasksController_Activated);
      }

      #endregion

      private DevExpress.ExpressApp.Actions.SimpleAction ClearTasksAction;

   }
}
