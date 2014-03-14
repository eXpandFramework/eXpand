using System;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Win.Core;
using DevExpress.ExpressApp.Win.Editors;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Repository;
using Xpand.ExpressApp.Win.Editors;
using EditorAliases = Xpand.Persistent.Base.General.EditorAliases;

namespace Xpand.ExpressApp.Win.PropertyEditors{
    [PropertyEditor(typeof(Enum), EditorAliases.FilterableEnumPropertyEditor, false)]
    public class FilterableEnumPropertyEditor : DXPropertyEditor, IComplexViewItem {
        private void UpdateControlWithCurrentObject() {
            var control = Control as IGridInplaceEdit;
            if (control != null)
                control.GridEditingObject = CurrentObject;
        }

        protected override object CreateControlCore() {
            return new XafEnumEdit();
        }
        protected override RepositoryItem CreateRepositoryItem() {
            return new RepositoryItemXafEnumEdit();
        }

        protected override void SetupRepositoryItem(RepositoryItem item) {
            base.SetupRepositoryItem(item);

            var enumEditRepositoryItem = item as RepositoryItemXafEnumEdit;

            if (enumEditRepositoryItem != null) enumEditRepositoryItem.Setup(Application, ObjectSpace, Model);

            UpdateControlWithCurrentObject();
        }
        protected override void OnCurrentObjectChanged() {
            base.OnCurrentObjectChanged();
            UpdateControlWithCurrentObject();
        }

        public FilterableEnumPropertyEditor(Type objectType, IModelMemberViewItem model)
            : base(objectType, model) {
            ImmediatePostData = model.ImmediatePostData;
        }

        public new ComboBoxEdit Control {
            get { return (ComboBoxEdit)base.Control; }
        }

        public void Setup(IObjectSpace objectSpace, XafApplication application) {
            Application = application;
            ObjectSpace = objectSpace;
        }

        public XafApplication Application {
            get;
            private set;
        }
        public IObjectSpace ObjectSpace {
            get;
            private set;
        }
    }
}