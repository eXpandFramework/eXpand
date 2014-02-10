using System;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Web.Editors.ASPx;

namespace Xpand.ExpressApp.Web.SystemModule.MasterDetail {
    public class MasterDetailKeyFieldController:ViewController<ListView>{
        protected override void OnActivated(){
            base.OnActivated();
            var editFrame = View.EditFrame;
            if (editFrame != null)
                foreach (var item in ((DetailView) editFrame.View).GetItems<ListPropertyEditor>()){
                    item.ControlCreated+=OnControlsCreated;
                }            
        }

        private void OnControlsCreated(object sender, EventArgs eventArgs){
            var asPxGridListEditor = ((ListPropertyEditor) sender).ListView.Editor as ASPxGridListEditor;
            if (asPxGridListEditor != null) asPxGridListEditor.ControlsCreated+=ASPxGridListEditorOnControlsCreated;
        }

        private void ASPxGridListEditorOnControlsCreated(object sender, EventArgs eventArgs){
            var asPxGridListEditor = ((ASPxGridListEditor) sender);
            asPxGridListEditor.Grid.KeyFieldName = asPxGridListEditor.Model.ModelClass.KeyProperty;
        }
    }
}
