using System;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Model;

namespace eXpand.ExpressApp {
    public class DetailView:DevExpress.ExpressApp.DetailView {
        public event EventHandler<CreateDetailViewEditorArgs> CreatingDetailViewEditor;

        protected void OnCreatingDetailViewEditor(CreateDetailViewEditorArgs e) {
            EventHandler<CreateDetailViewEditorArgs> handler = CreatingDetailViewEditor;
            if (handler != null) handler(this, e);
        }

        public DetailView(IModelDetailView info, ObjectSpace objectSpace, object obj, XafApplication application, bool isRoot) : base(info, objectSpace, obj, application, isRoot) {
        }

        public DetailView(ObjectSpace objectSpace, object obj, XafApplication application, bool isRoot) : base(objectSpace, obj, application, isRoot) {
        }

        protected override ViewItem CreateItem(IModelDetailViewItem info) {
            if (Application == null)
                return null;
            Type objType = ObjectTypeInfo != null ? ObjectTypeInfo.Type : null;
            bool needProtectedContentEditor = false;
            if (objType != null && info is IModelPropertyEditor){
                needProtectedContentEditor = !DataManipulationRight.CanRead(objType, ((IModelPropertyEditor)info).PropertyName, CurrentObject, null);
            }
            var createDetailViewEditorArgs = new CreateDetailViewEditorArgs(needProtectedContentEditor, info,  CurrentObject);
            OnCreatingDetailViewEditor(createDetailViewEditorArgs);
            return Application.EditorFactory.CreateDetailViewEditor(createDetailViewEditorArgs.NeedProtectedContentEditor, createDetailViewEditorArgs.ModelDetailViewItem, objType, Application, ObjectSpace);

        }
    }

    public class CreateDetailViewEditorArgs : EventArgs {
        readonly bool _needProtectedContentEditor;
        readonly IModelDetailViewItem _modelDetailViewItem;
        readonly object _currentObject;

        public CreateDetailViewEditorArgs(bool needProtectedContentEditor, IModelDetailViewItem modelDetailViewItem, object currentObject) {
            _needProtectedContentEditor = needProtectedContentEditor;
            _modelDetailViewItem = modelDetailViewItem;
            _currentObject = currentObject;
        }

        public bool NeedProtectedContentEditor {
            get { return _needProtectedContentEditor; }
        }

        public IModelDetailViewItem ModelDetailViewItem {
            get { return _modelDetailViewItem; }
        }


        public object CurrentObject {
            get { return _currentObject; }
        }
    }
}