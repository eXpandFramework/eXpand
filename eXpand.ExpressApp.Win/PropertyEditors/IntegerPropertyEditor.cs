﻿using System;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Win.Editors;
using DevExpress.Utils;
using DevExpress.XtraEditors.Repository;
using GridListEditor=eXpand.ExpressApp.Win.ListEditors.GridListEditor;

namespace eXpand.ExpressApp.Win.PropertyEditors
{
    [PropertyEditor(typeof (int))]
    [PropertyEditor(typeof (int?))]
    public class IntegerPropertyEditor : DevExpress.ExpressApp.Win.Editors.IntegerPropertyEditor
    {
        public IntegerPropertyEditor(Type objectType, DictionaryNode info) : base(objectType, info)
        {
        }

        protected override void SetupRepositoryItem(RepositoryItem item)
        {
            base.SetupRepositoryItem(item);

            var integerEdit = (RepositoryItemIntegerEdit) item;
//            integerEdit.Mask.Culture = GridListEditor.GetCulture(Info);
            integerEdit.Init(EditMask, DisplayFormat);
            var repositoryItemIntegerEdit = (RepositoryItemIntegerEdit) item;
            if (View != null)
            {
                Type type = View.ObjectTypeInfo.Type.GetProperty(PropertyName).PropertyType;
                bool b = type == typeof (int?);
                repositoryItemIntegerEdit.AllowNullInput =
                    b
                        ? DefaultBoolean.True
                        : DefaultBoolean.Default;
            }
        }
    }
}