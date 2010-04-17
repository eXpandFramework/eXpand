using System;
using DevExpress.ExpressApp.Model;

namespace eXpand.ExpressApp.Win.PropertyEditors.ObjectPropertyEditor{
    public class ObjectPropertyEditor:DevExpress.ExpressApp.Win.Editors.ObjectPropertyEditor
    {
        public ObjectPropertyEditor(Type objectType, IModelMemberViewItem model)
            : base(objectType, model)
        {
        }

        protected override object CreateControlCore()
        {
            return new ObjectEdit();
        }
    }
}