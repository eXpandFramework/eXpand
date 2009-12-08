using System;
using eXpand.ExpressApp.Enums;

namespace eXpand.ExpressApp.Attributes {
    public class AllowEditAttribute : Attribute {
        readonly bool _allowEdit;

        readonly AllowEditEnum _allowEditEnum;

        public AllowEditAttribute(bool allowEdit, AllowEditEnum allowEditEnum) {
            _allowEdit = allowEdit;
            _allowEditEnum = allowEditEnum;
        }

        public bool AllowEdit {
            get { return _allowEdit; }
        }

        public AllowEditEnum AllowEditEnum {
            get { return _allowEditEnum; }
        }
    }
}