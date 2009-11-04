using System;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Win.Editors;
using DevExpress.Utils;
using DevExpress.XtraEditors.Repository;

namespace eXpand.ExpressApp.Win.PropertyEditors.BooleanPropertyEditor {
    public class BooleanHAlignFarPropertyEditor : DevExpress.ExpressApp.Win.Editors.BooleanPropertyEditor
    {
        public BooleanHAlignFarPropertyEditor(Type objectType, DictionaryNode info)
            : base(objectType, info)
        {
        }

        protected override void SetupRepositoryItem(RepositoryItem item)
        {
            base.SetupRepositoryItem(item);
            var ri = (RepositoryItemBooleanEdit)item;
            ri.GlyphAlignment = HorzAlignment.Far;
            ri.Appearance.TextOptions.HAlignment = HorzAlignment.Far;
            ri.Appearance.Options.UseTextOptions = true;
            
        }
    }
}