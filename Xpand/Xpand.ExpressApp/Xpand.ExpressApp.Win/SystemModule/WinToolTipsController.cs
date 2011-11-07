using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Win.Editors;
using DevExpress.XtraEditors;
using Xpand.ExpressApp.SystemModule;

namespace Xpand.ExpressApp.Win.SystemModule {
    public class WinToolTipsController : ToolTipsController {
        protected override void SetListViewToolTips() {
            var editor = ((ListView)View).Editor as GridListEditor;
            if (editor != null) {
                foreach (ColumnWrapper columnWrapper in editor.Columns) {
                    XafGridColumn column = ((XafGridColumnWrapper)columnWrapper).Column;
                    if (TooltipCalculator.HasToolTip(column.Model))
                        column.ToolTip = TooltipCalculator.GetToolTip(column.Model);
                }
            }
        }



        protected override void SetDetailViewToolTips() {
            foreach (PropertyEditor editor in ((DetailView)View).GetItems<PropertyEditor>()) {
                if (editor.Control != null && (editor.Control is BaseEdit)) {
                    IModelMemberViewItem modelMemberViewItem = editor.Model;
                    var baseEdit = ((BaseEdit)(editor.Control));
                    if (modelMemberViewItem.ModelMember.MemberInfo.MemberType.IsEnum) {
                        baseEdit.EditValueChanged += (sender, args) => {
                            if (TooltipCalculator.HasToolTip(modelMemberViewItem))
                                baseEdit.ToolTip = TooltipCalculator.GetToolTip(modelMemberViewItem, baseEdit.EditValue);
                        };
                    }
                    if (TooltipCalculator.HasToolTip(modelMemberViewItem))
                        baseEdit.ToolTip = TooltipCalculator.GetToolTip(modelMemberViewItem);
                }
            }
        }
    }

}