using System;
using System.Collections.Generic;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.NodeWrappers;
using DevExpress.ExpressApp.Win.Editors;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.Repository;

namespace eXpand.ExpressApp.Win.PropertyEditors.StringPropertyEditors{
    public abstract class StringPropertyEditorBase : DXPropertyEditor, IComplexPropertyEditor{
        protected LookupEditorHelper helper;

        protected StringPropertyEditorBase(Type objectType, DictionaryNode info)
            : base(objectType, info)
        {
            if (string.IsNullOrEmpty(info.GetAttributeValue(DetailViewItemInfoNodeWrapper.ImmediatePostDataAttribute))){
                ImmediatePostData = true;
            }
        }

        protected override void SetupRepositoryItem(RepositoryItem item)
        {
            ((RepositoryItemComboBox) item).Items.AddRange(ComboBoxItems);
            base.SetupRepositoryItem(item);
            
        }

        protected abstract List<ComboBoxItem> ComboBoxItems { get;  }

        protected override object CreateControlCore()
        {
            return new ComboBoxEdit();
        }

        protected override RepositoryItem CreateRepositoryItem()
        {
            var repositoryItemComboBox = new RepositoryItemComboBox();
            repositoryItemComboBox.Items.AddRange(ComboBoxItems);
            return repositoryItemComboBox;
        }

        public void Setup(ObjectSpace objectSpace, XafApplication application)
        {
            if (helper == null)
                helper = new LookupEditorHelper(application, objectSpace, ObjectTypeInfo, Info);
            helper.SetObjectSpace(objectSpace);
        }
    }
}