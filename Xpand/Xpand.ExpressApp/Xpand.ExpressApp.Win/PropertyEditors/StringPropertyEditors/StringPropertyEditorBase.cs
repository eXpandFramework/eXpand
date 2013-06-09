using System;
using System.Collections.Generic;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Win.Editors;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.Repository;

namespace Xpand.ExpressApp.Win.PropertyEditors.StringPropertyEditors {
    public abstract class StringPropertyEditorBase : DXPropertyEditor, IComplexViewItem {
        protected LookupEditorHelper helper;

        protected StringPropertyEditorBase(Type objectType, IModelMemberViewItem model)
            : base(objectType, model) {
            ImmediatePostData = model.ImmediatePostData;
        }

        protected override void SetupRepositoryItem(RepositoryItem item) {
            ((RepositoryItemComboBox)item).Items.AddRange(ComboBoxItems);
            base.SetupRepositoryItem(item);
        }

        protected abstract List<ComboBoxItem> ComboBoxItems { get; }

        protected override object CreateControlCore() {
            return new ComboBoxEdit();
        }

        protected override RepositoryItem CreateRepositoryItem() {
            var repositoryItemComboBox = new RepositoryItemComboBox();
            repositoryItemComboBox.Items.AddRange(ComboBoxItems);
            return repositoryItemComboBox;
        }

        public void Setup(IObjectSpace objectSpace, XafApplication application) {
            if (helper == null)
                helper = new LookupEditorHelper(application, objectSpace, ObjectTypeInfo, Model);
            helper.SetObjectSpace(objectSpace);
        }
    }
}