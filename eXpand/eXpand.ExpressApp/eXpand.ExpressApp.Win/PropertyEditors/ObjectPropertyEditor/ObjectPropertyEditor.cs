using System;
using DevExpress.ExpressApp;

namespace eXpand.ExpressApp.Win.PropertyEditors.ObjectPropertyEditor{
    public class ObjectPropertyEditor:DevExpress.ExpressApp.Win.Editors.ObjectPropertyEditor
    {
        public ObjectPropertyEditor(Type objectType, DictionaryNode info) : base(objectType, info)
        {
        }

        protected override object CreateControlCore()
        {
            return new ObjectEdit();
        }
    }
}