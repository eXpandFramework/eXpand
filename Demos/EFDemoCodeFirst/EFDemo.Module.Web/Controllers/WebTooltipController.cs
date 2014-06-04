using System;

using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Web.Editors.ASPx;
using DevExpress.Web.ASPxGridView;

namespace EFDemo.Module.Web.Controllers {
    public partial class WebTooltipController : ViewController {
        public WebTooltipController() {
            InitializeComponent();
            RegisterActions(components);
        }
        private void WebTooltipController_ViewControlsCreated(object sender, EventArgs e) {
            ASPxGridListEditor listEditor = ((ListView)View).Editor as ASPxGridListEditor;
            if(listEditor != null) {
                ASPxGridView gridControl = listEditor.Grid;
                foreach(GridViewColumn column in gridControl.Columns) {
                    if((column as GridViewDataColumn) != null)
                        column.ToolTip = "Click to sort by " + column.Caption;
                }
            }
        }
    }
}
