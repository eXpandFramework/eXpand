﻿using System;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Editors;
using DevExpress.Utils;
using DevExpress.XtraEditors.Repository;

namespace eXpand.ExpressApp.Win.PropertyEditors
{
    [PropertyEditor(typeof (DateTime))]
    [PropertyEditor(typeof (DateTime?))]
    public class DatePropertyEditor : DevExpress.ExpressApp.Win.Editors.DatePropertyEditor
    {
        public DatePropertyEditor(Type objectType, DictionaryNode info) : base(objectType, info)
        {
            ValueStoring += ValueStoringMethod;
        }

        private void ValueStoringMethod(object sender, EventArgs e)
        {
        }

        protected override void SetupRepositoryItem(RepositoryItem item)
        {
            base.SetupRepositoryItem(item);
            var repositoryItemIntegerEdit = (RepositoryItemDateEdit) item;
            if (View != null)
            {
                Type type = View.ObjectTypeInfo.FindMember(PropertyName).MemberType;
                bool b = type == typeof (DateTime?);
                if (b)
                    repositoryItemIntegerEdit.NullDate = null;
                repositoryItemIntegerEdit.AllowNullInput =
                    b
                        ? DefaultBoolean.True
                        : DefaultBoolean.Default;
            }
        }
    }
}