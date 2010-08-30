using System;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Model;

namespace eXpand.ExpressApp {
    public class DetailView:DevExpress.ExpressApp.DetailView {
        

        public DetailView(IModelDetailView info, ObjectSpace objectSpace, object obj, XafApplication application, bool isRoot) : base(info, objectSpace, obj, application, isRoot) {
        }

        public DetailView(ObjectSpace objectSpace, object obj, XafApplication application, bool isRoot) : base(objectSpace, obj, application, isRoot) {
        }

        protected override ViewItem CreateItem(IModelDetailViewItem info) {
            if (Application == null)
                return null;
            Type objType = ObjectTypeInfo != null ? ObjectTypeInfo.Type : null;
            return Application.EditorFactory.CreateDetailViewEditor(false, info, objType, Application, ObjectSpace);

        }
    }

}