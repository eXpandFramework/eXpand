using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Win.Editors;
using DevExpress.XtraEditors;
using Xpand.ExpressApp.SystemModule;
using Xpand.ExpressApp.Win.ListEditors.GridListEditors.ColumnView.Model;

namespace Xpand.ExpressApp.Win.SystemModule.ToolTip {
    public class WinToolTipsController : ToolTipsController {
        protected override void SetListViewToolTips() {
            var editor = ((ListView)View).Editor as WinColumnsListEditor;
            if (editor != null) {
                if (editor.Columns != null)
                    foreach (var columnWrapper in editor.Columns) {
                        var column = columnWrapper.Column();
                        if (column != null && TooltipCalculator.HasToolTip(column.Model()))
                            column.ToolTip = TooltipCalculator.GetToolTip(column.Model());
                    }
            }
        }

        protected override void SetDetailViewToolTips() {
            foreach (PropertyEditor editor in ((DetailView)View).GetItems<PropertyEditor>()) {
                var edit = editor.Control as BaseEdit;
                if (edit != null) {
                    IModelMemberViewItem modelMemberViewItem = editor.Model;
                    if (modelMemberViewItem.ModelMember.MemberInfo.MemberType.IsEnum) {
                        edit.EditValueChanged += (sender, args) => {
                            if (TooltipCalculator.HasToolTip(modelMemberViewItem))
                                edit.ToolTip = TooltipCalculator.GetToolTip(modelMemberViewItem, edit.EditValue);
                        };
                    }
                    if (TooltipCalculator.HasToolTip(modelMemberViewItem))
                        edit.ToolTip = TooltipCalculator.GetToolTip(modelMemberViewItem);
                }
            }
        }
    }

}