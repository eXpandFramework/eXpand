using System;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Web.Editors.ASPx;
using Xpand.ExpressApp.Web.ListEditors;

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
            var asPxGridListEditor = ((ListPropertyEditor) sender).ListView.Editor as XpandASPxGridListEditor;
            if (asPxGridListEditor != null){
                asPxGridListEditor.ControlsCreated+=ASPxGridListEditorOnControlsCreated;
                asPxGridListEditor.CustomCreateWebDataSource+=ASPxGridListEditorOnCustomCreateWebDataSource;
            }
        }

        private void ASPxGridListEditorOnCustomCreateWebDataSource(object sender, CustomCreateWebDataSourceEventArgs e){
            if (e.Collection == null){
                e.Handled = true;
                var objectSpace = Application.CreateObjectSpace();
                var modelListView = ((ListEditor) sender).Model;
                e.Collection = new ProxyCollection(objectSpace, modelListView.ModelClass.TypeInfo, new object[0]);
            }
        }

        private void ASPxGridListEditorOnControlsCreated(object sender, EventArgs eventArgs){
            var asPxGridListEditor = ((ASPxGridListEditor) sender);
            asPxGridListEditor.Grid.KeyFieldName = asPxGridListEditor.Model.ModelClass.KeyProperty;
            
        }
    }
}
