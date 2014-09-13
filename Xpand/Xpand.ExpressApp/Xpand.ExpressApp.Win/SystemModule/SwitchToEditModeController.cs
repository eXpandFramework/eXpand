using System;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.Editors;
using DevExpress.Persistent.Base;
using Xpand.ExpressApp.SystemModule;

namespace Xpand.ExpressApp.Win.SystemModule {
    public class SwitchToEditModeController : ViewController<DetailView> {
        readonly SimpleAction _editAction;

        public SwitchToEditModeController() {
            _editAction = new SimpleAction(this, "SwitchToEditMode", PredefinedCategory.RecordEdit);
            _editAction.Execute+=EditActionOnExecute;
            _editAction.Caption = "Edit";
            _editAction.SelectionDependencyType = SelectionDependencyType.RequireSingleObject;
            _editAction.ImageName = "MenuBar_Edit";
            _editAction.Active["ViewEditMode"] = false;
        }

        public SimpleAction EditAction {
            get { return _editAction; }
        }

        protected override void OnActivated(){
            base.OnActivated();
            _editAction.Active["ViewEditMode"] = !View.AllowEdit;
            ObjectSpace.Committed -= ObjectSpaceOnCommitted;
            ObjectSpace.Committed += ObjectSpaceOnCommitted;
        }

        protected override void OnDeactivated() {
            base.OnDeactivated();
            ObjectSpace.Committed-=ObjectSpaceOnCommitted;
        }

        void ObjectSpaceOnCommitted(object sender, EventArgs eventArgs) {
            var viewEditMode = ((IModelDetailViewViewEditMode)View.Model).ViewEditMode;
            if (viewEditMode.HasValue) {
                View.AllowEdit["ViewEditMode"] = View.ViewEditMode == ViewEditMode.Edit;
                _editAction.Active["ViewEditMode"] = !View.AllowEdit.ResultValue;
            }
        }

        void EditActionOnExecute(object sender, SimpleActionExecuteEventArgs simpleActionExecuteEventArgs) {
            View.AllowEdit["ViewEditMode"] = true;
            _editAction.Active["ViewEditMode"] = !View.AllowEdit.ResultValue;
            ObjectSpace.Refresh();
        }
    }
}