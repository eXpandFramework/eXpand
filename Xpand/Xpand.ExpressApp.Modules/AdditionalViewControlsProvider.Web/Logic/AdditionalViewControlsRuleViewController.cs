using System.Web.UI;
using System.Web.UI.WebControls;
using DevExpress.ExpressApp.Templates;
using Xpand.ExpressApp.AdditionalViewControlsProvider.Logic;
using Xpand.ExpressApp.Logic;

namespace Xpand.ExpressApp.AdditionalViewControlsProvider.Web.Logic {
    public class AdditionalViewControlsRuleViewController : AdditionalViewControlsProvider.Logic.AdditionalViewControlsRuleViewController {
        protected override void AddControl(object control, object controls, LogicRuleInfo<IAdditionalViewControlsRule> info) {

            var tableCell = (TableCell)GetContainerControl((IViewSiteTemplate)Frame.Template, info.Rule);
            var tableRow = ((TableRow)tableCell.Parent);
            var row = new TableRow();
            var tableRowCollection = ((Table)tableRow.Parent).Rows;
            var cellCollection = row.Cells;
            ((Control)control).Visible = true;
            var cell = new TableCell();
            cell.Controls.Add((Control)control);
            cellCollection.Add(cell);
            switch (info.Rule.Position) {
                case Position.Top: {
                        tableRowCollection.AddAt(0, row);
                        break;
                    }
                default:
                    tableRowCollection.Add(row);
                    break;
            }

        }
    }
}