using System;
using DevExpress.LookAndFeel;
using DevExpress.Utils.Design;

namespace Xpand.ExpressApp.Win.ListEditors.GridListEditors.ColumnView.Design {
    public abstract class ColumnViewDesignerForm : BaseDesignerForm {
        readonly string _title;

        protected ColumnViewDesignerForm(UserLookAndFeel userLookAndFeel, string title)
            : base(userLookAndFeel) {
            _title = title;
        }
        protected override string GetDefaultProductInfo() {
            return string.Empty;
        }
        protected override void CreateDesigner() {
            ActiveDesigner = GetActiveDesigner();
        }

        protected abstract BaseDesigner GetActiveDesigner();
        protected override string RegistryStorePath { get { return "Software\\" + _title + "\\Designer\\" + GetType().Name + "\\"; } }
        protected override Type ResolveType(string type) {
            return typeof(ColumnViewDesignerForm).Assembly.GetType(type) ?? base.ResolveType(type);
        }
    }
}