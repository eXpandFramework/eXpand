namespace eXpand.ExpressApp.SystemModule {
    partial class FilterByPropertyPathViewController
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
            this._filterSingleChoiceAction = new DevExpress.ExpressApp.Actions.SingleChoiceAction(this.components);
            // 
            // _filterSingleChoiceAction
            // 
            this._filterSingleChoiceAction.Caption = "Search By";
            this._filterSingleChoiceAction.Category = "";
            this._filterSingleChoiceAction.Id = "_filterSingleChoiceAction";
            this._filterSingleChoiceAction.ItemType = DevExpress.ExpressApp.Actions.SingleChoiceActionItemType.ItemIsOperation;
            this._filterSingleChoiceAction.Execute += new DevExpress.ExpressApp.Actions.SingleChoiceActionExecuteEventHandler(this.createFilterSingleChoiceAction_Execute);

        }

        #endregion

        private DevExpress.ExpressApp.Actions.SingleChoiceAction _filterSingleChoiceAction;
    }
}