using DevExpress.XtraEditors.Drawing;
using DevExpress.XtraEditors.Registrator;
using DevExpress.XtraEditors.ViewInfo;

namespace eXpand.ExpressApp.Win.PropertyEditors.LookupPropertyEditor{
    public class RepositoryItemLookupEdit:DevExpress.ExpressApp.Win.Editors.RepositoryItemLookupEdit
    {
        internal const string EditorName = "eXpandLookupEdit";
        public new static void Register()
        {
            if (!EditorRegistrationInfo.Default.Editors.Contains(EditorName))
            {
                EditorRegistrationInfo.Default.Editors.Add(new EditorClassInfo(EditorName, typeof(LookupEdit),
                                                                               typeof(RepositoryItemLookupEdit), typeof(PopupBaseEditViewInfo),
                                                                               new ButtonEditPainter(), true, EditImageIndexes.LookUpEdit, typeof(DevExpress.Accessibility.PopupEditAccessible)));
            }
        }
        static RepositoryItemLookupEdit() {
            Register();
        }
        //public void Init(XafApplication application, ITypeInfo editValueTypeInfo, IMemberInfo defaultMember, ObjectSpace objectSpace)
        //{
        //    this.application = application;
        //    this.editValueTypeInfo = editValueTypeInfo;
        //    this.defaultMember = defaultMember;
        //    this.objectSpace = objectSpace;
        //}

        public override string EditorTypeName { get { return EditorName; } }    
    }
}