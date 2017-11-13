namespace MainDemo.Module.Controllers {
	partial class PopupNotesController {
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary> 
		/// Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing) {
			if(disposing && (components != null)) {
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
			this.components = new System.ComponentModel.Container();
			this.ShowNotesAction = new DevExpress.ExpressApp.Actions.PopupWindowShowAction(this.components);
			// 
			// ShowNotesAction
			// 
			this.ShowNotesAction.Caption = "Show Notes";
            this.ShowNotesAction.Category = DevExpress.Persistent.Base.PredefinedCategory.Edit.ToString();
			this.ShowNotesAction.Id = "ShowNotesAction";
            this.ShowNotesAction.ImageName = "BO_Note";
			this.ShowNotesAction.Execute += new DevExpress.ExpressApp.Actions.PopupWindowShowActionExecuteEventHandler(this.ShowNotesAction_Execute);
			this.ShowNotesAction.CustomizePopupWindowParams += new DevExpress.ExpressApp.Actions.CustomizePopupWindowParamsEventHandler(this.ShowNotesAction_CustomizePopupWindowParams);
			// 
			// PopupNotesController
			// 
			this.TargetViewType = DevExpress.ExpressApp.ViewType.DetailView;
            this.TargetObjectType = typeof(MainDemo.Module.BusinessObjects.DemoTask);

		}
		
		private DevExpress.ExpressApp.Actions.PopupWindowShowAction ShowNotesAction;
		#endregion

   }
}
