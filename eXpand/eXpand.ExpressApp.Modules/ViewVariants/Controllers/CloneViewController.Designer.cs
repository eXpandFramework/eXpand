namespace eXpand.ExpressApp.ViewVariants.Controllers
{
    partial class CloneViewController {
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
            this.components = new System.ComponentModel.Container();
            this.cloneViewPopupWindowShowAction = new DevExpress.ExpressApp.Actions.PopupWindowShowAction(this.components);
            // 
            // cloneViewPopupWindowShowAction
            // 
            this.cloneViewPopupWindowShowAction.AcceptButtonCaption = null;
            this.cloneViewPopupWindowShowAction.CancelButtonCaption = null;
            this.cloneViewPopupWindowShowAction.Caption = "Clone View";
            this.cloneViewPopupWindowShowAction.Category = "View";
            this.cloneViewPopupWindowShowAction.Id = "cloneViewPopupWindowShowAction";
            this.cloneViewPopupWindowShowAction.Execute += new DevExpress.ExpressApp.Actions.PopupWindowShowActionExecuteEventHandler(this.cloneViewPopupWindowShowAction_Execute);
            this.cloneViewPopupWindowShowAction.CustomizePopupWindowParams += new DevExpress.ExpressApp.Actions.CustomizePopupWindowParamsEventHandler(this.cloneViewPopupWindowShowAction_CustomizePopupWindowParams);

        }

        #endregion

        public DevExpress.ExpressApp.Actions.PopupWindowShowAction cloneViewPopupWindowShowAction;

    }
}