using System;
using System.Windows.Forms;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Model;
using DevExpress.XtraEditors.Repository;

namespace Xpand.ExpressApp.Win.PropertyEditors.StringPropertyEditors {
    [PropertyEditor(typeof(string), false)]
    public class MemoPropertyEditor : StringPropertyEditor {
        public MemoPropertyEditor(Type objectType, IModelMemberViewItem model)
            : base(objectType, model) {
        }

        protected override RepositoryItem CreateRepositoryItem() {
            return new RepositoryItemMemoEdit { MaxLength = Model.MaxLength, ScrollBars = ScrollBars.Both };
        }
    }
}
