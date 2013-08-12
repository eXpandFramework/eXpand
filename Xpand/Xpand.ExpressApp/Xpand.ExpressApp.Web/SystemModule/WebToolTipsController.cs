﻿using System.Web.UI.WebControls;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Web.Editors.ASPx;
using Xpand.ExpressApp.SystemModule;

namespace Xpand.ExpressApp.Web.SystemModule {
    public class WebToolTipsController : ToolTipsController {
        protected override void SetListViewToolTips() {
            var editor = ((DevExpress.ExpressApp.ListView)View).Editor as ASPxGridListEditor;
            if (editor != null) {
                foreach (ColumnWrapper columnWrapper in editor.Columns) {
                    if (columnWrapper is ASPxGridViewColumnWrapper) {
                        GridViewDataColumnWithInfo column =
                            ((ASPxGridViewColumnWrapper)columnWrapper).Column;
                        if (TooltipCalculator.HasToolTip(column.Model))
                            column.ToolTip = TooltipCalculator.GetToolTip(column.Model);
                    }
                }
            }
        }
        protected override void SetDetailViewToolTips() {
            foreach (PropertyEditor editor in ((DetailView)View).GetItems<PropertyEditor>()) {
                if (editor.Control != null && (editor.Control is WebControl) && TooltipCalculator.HasToolTip(editor.Model))
                    ((WebControl)(editor.Control)).ToolTip = TooltipCalculator.GetToolTip(editor.Model);
            }
        }
    }
}