using System;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Win.Editors;
using DevExpress.Utils;
using DevExpress.XtraEditors.Repository;

namespace Xpand.ExpressApp.Win.PropertyEditors.NullAble.BooleanPropertyEditor {
    
    public class BooleanHAlignFarPropertyEditor : DevExpress.ExpressApp.Win.Editors.BooleanPropertyEditor {
        public BooleanHAlignFarPropertyEditor(Type objectType, IModelMemberViewItem member)
            : base(objectType, member) {
        }

        protected override void SetupRepositoryItem(RepositoryItem item) {
            base.SetupRepositoryItem(item);
            var ri = item as RepositoryItemBooleanEdit;
            if (ri != null) {
                ri.GlyphAlignment = HorzAlignment.Far;
                ri.Appearance.TextOptions.HAlignment = HorzAlignment.Far;
                ri.Appearance.Options.UseTextOptions = true;
            }
        }
    }
}