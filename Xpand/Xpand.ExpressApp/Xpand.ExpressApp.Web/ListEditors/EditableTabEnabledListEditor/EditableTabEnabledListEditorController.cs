using System;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Model;

namespace Xpand.ExpressApp.Web.ListEditors.EditableTabEnabledListEditor{
    public class EditableTabEnabledListEditorController : ViewController<ListView>,IModelExtender{
        private EditableTabEnabledListEditor _editor;

        protected override void OnActivated(){
            base.OnActivated();
            View.ControlsCreated += View_ControlsCreated;
            View.ObjectSpace.Committed += ObjectSpace_Committed;
        }

        private void ObjectSpace_Committed(object sender, EventArgs e){
            if (_editor != null){
                _editor.BindDataSource();
            }
        }


        private void View_ControlsCreated(object sender, EventArgs e){
            _editor = View.Editor as EditableTabEnabledListEditor;
        }

        protected override void OnDeactivated(){
            View.ControlsCreated -= View_ControlsCreated;
            View.ObjectSpace.Committed -= ObjectSpace_Committed;
            base.OnDeactivated();
        }

        public void ExtendModelInterfaces(ModelInterfaceExtenders extenders){
            extenders.Add<IModelListView, IModelListViewEditableTabEnabledEditor>();
            extenders.Add<IModelColumnSummaryItem, IModelColumnSummaryItemEditabledTabEnabled>();
        }
    }
}