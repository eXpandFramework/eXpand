using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Win.Editors;
using DevExpress.XtraEditors;
using Xpand.ExpressApp.SystemModule;

namespace Xpand.ExpressApp.Win.SystemModule {
    public class WinToolTipsController : ToolTipsController {
        protected override void SetListViewToolTips() {
            var editor = ((ListView) View).Editor as GridListEditor;
            if (editor != null) {
                foreach (ColumnWrapper columnWrapper in editor.Columns) {
                    XafGridColumn column = ((XafGridColumnWrapper) columnWrapper).Column;
                    column.ToolTip = GetToolTip(column.Model.ModelMember);
                }
            }
        }

        protected override void SetDetailViewToolTips() {
            foreach (PropertyEditor editor in ((DetailView) View).GetItems<PropertyEditor>()) {
                if (editor.Control != null && (editor.Control is BaseEdit))
                    ((BaseEdit) (editor.Control)).ToolTip = GetToolTip(editor.Model.ModelMember);
            }
        }
    }
}