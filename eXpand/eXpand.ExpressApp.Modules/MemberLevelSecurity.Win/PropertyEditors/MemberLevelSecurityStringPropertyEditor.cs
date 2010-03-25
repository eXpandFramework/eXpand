using System;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Win.Editors;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Mask;
using DevExpress.XtraEditors.Repository;

namespace eXpand.ExpressApp.MemberLevelSecurity.Win.PropertyEditors
{
    public class MemberLevelSecurityStringPropertyEditor : StringPropertyEditor
    {
        

        public MemberLevelSecurityStringPropertyEditor(Type objectType, DictionaryNode info)
            : base(objectType, info)
        {
        }

        protected override object CreateControlCore()
        {
            return new ButtonEdit();
        }


        protected override RepositoryItem CreateRepositoryItem()
        {
            return new RepositoryItemButtonEdit();
        }

        protected override void SetupRepositoryItem(RepositoryItem item)
        {
            var repositoryItemButtonEdit = (RepositoryItemButtonEdit)item;
            repositoryItemButtonEdit.Buttons.Clear();
            if (!string.IsNullOrEmpty(EditMask))
            {
                repositoryItemButtonEdit.Mask.EditMask = EditMask;
                switch (EditMaskType)
                {
                    case EditMaskType.RegEx:
                        repositoryItemButtonEdit.Mask.UseMaskAsDisplayFormat = false;
                        repositoryItemButtonEdit.Mask.MaskType = MaskType.RegEx;
                        break;
                    default:
                        repositoryItemButtonEdit.Mask.MaskType = MaskType.Simple;
                        break;
                }
            }
            OnCustomSetupRepositoryItem(new CustomSetupRepositoryItemEventArgs(item));
        }
    }
}