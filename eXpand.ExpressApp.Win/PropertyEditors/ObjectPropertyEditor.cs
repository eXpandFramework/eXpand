using System;
using DevExpress.ExpressApp;
using eXpand.ExpressApp.Win.Editors;

namespace eXpand.ExpressApp.Win.PropertyEditors
{
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