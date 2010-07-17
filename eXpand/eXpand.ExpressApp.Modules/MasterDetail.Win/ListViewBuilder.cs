using System;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Model;
using eXpand.ExpressApp.Win;
using eXpand.ExpressApp.Win.ListEditors;
using XafGridView = DevExpress.ExpressApp.Win.Editors.XafGridView;

namespace eXpand.ExpressApp.MasterDetail.Win.Logic {
    public class ListViewBuilder {
        readonly XafApplication _xafApplication;
        readonly ObjectSpace _objectSpace;
        GridListEditor _gridListEditor;
        XafGridView _xafGridView;

        public ListViewBuilder(XafApplication xafApplication,ObjectSpace objectSpace) {
            _xafApplication = xafApplication;
            _objectSpace = objectSpace;
        }

        public ListView CreateListView(IModelListView modelListView)
        {
            Type type = modelListView.ModelClass.TypeInfo.Type;
            CollectionSourceBase collectionSourceBase = _xafApplication.CreateCollectionSource(_objectSpace.CreateNestedObjectSpace(), type, modelListView.Id);
            ListView listView = _xafApplication.CreateListView(modelListView, collectionSourceBase, true);
//            listView.CreateControls();
            return listView;
        }

        public ListView CreateListView(IModelListView modelListView, XafGridView xafGridView) {
            _xafGridView = xafGridView;
            Type type = modelListView.ModelClass.TypeInfo.Type;
            CollectionSourceBase collectionSourceBase = _xafApplication.CreateCollectionSource(_objectSpace.CreateNestedObjectSpace(), type, modelListView.Id);
            _gridListEditor = new GridListEditor(modelListView);
            _gridListEditor.CustomGridViewCreate+=GridListEditorOnCustomGridViewCreate;
            _gridListEditor.CustomGridCreate+=GridListEditorOnCustomGridCreate;
            
            
            
            ((ISupportCustomListEditorCreation) _xafApplication).CustomCreateListEditor +=OnCustomCreateListEditor;
            var listView = _xafApplication.CreateListView(modelListView, collectionSourceBase, false);
            listView.CreateControls();


            return listView;
        }

        void GridListEditorOnCustomGridCreate(object sender, CustomGridCreateEventArgs customGridCreateEventArgs) {
            _gridListEditor.CustomGridCreate -= GridListEditorOnCustomGridCreate;
//            customGridCreateEventArgs.Handled = true;
            customGridCreateEventArgs.Grid = _xafGridView.GridControl;
        }

        void GridListEditorOnCustomGridViewCreate(object sender, CustomGridViewCreateEventArgs args) {
            _gridListEditor.CustomGridViewCreate -= GridListEditorOnCustomGridViewCreate;
            args.GridView = (ExpressApp.Win.ListEditors.XafGridView) _xafGridView;
//            args.Handled = true;
        }

        void OnCustomCreateListEditor(object sender, CreatingListEditorEventArgs args) {
            ((ISupportCustomListEditorCreation) _xafApplication).CustomCreateListEditor-=OnCustomCreateListEditor;
            args.Handled = true;
            args.ListEditor = _gridListEditor;
        }

    }
}