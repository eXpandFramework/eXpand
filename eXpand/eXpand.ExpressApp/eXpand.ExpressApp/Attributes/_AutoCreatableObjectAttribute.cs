using System;
using DevExpress.ExpressApp.Editors;

namespace eXpand.ExpressApp.Attributes {
    [AttributeUsage(AttributeTargets.Class, Inherited = true, AllowMultiple = false)]
    public class AutoCreatableObjectAttribute : Attribute
    {
        private readonly bool autoCreatable = true;
        private ViewEditMode viewEditMode = ViewEditMode.Edit;
        public AutoCreatableObjectAttribute()
        {
        }
        public AutoCreatableObjectAttribute(bool autoCreatable)
        {
            this.autoCreatable = autoCreatable;
        }
        public bool AutoCreatable
        {
            get { return autoCreatable; }
        }
        public ViewEditMode ViewEditMode
        {
            get { return viewEditMode; }
            set { viewEditMode = value; }
        }

    }
}