using System.Web.UI;
using System.Web.UI.WebControls;
using DevExpress.ExpressApp.Templates;
using DevExpress.ExpressApp.Web.Controls;
using Xpand.ExpressApp.AdditionalViewControlsProvider.Logic;
using Xpand.ExpressApp.Logic;

namespace Xpand.ExpressApp.AdditionalViewControlsProvider.Web.Logic {
    public class AdditionalViewControlsRuleViewController : AdditionalViewControlsProvider.Logic.AdditionalViewControlsRuleViewController {
        protected override void AddControl(object control, object controls, LogicRuleInfo<IAdditionalViewControlsRule> info) {
            var viewSiteControl = GetContainerControl((IViewSiteTemplate)Frame.Template, info.Rule) as ViewSiteControl;
            if (viewSiteControl != null) {
                //                if (info.Rule.Position!=Position.DetailViewItem) {
                //                    var hintPanel = (HintPanel) control;
                //                    hintPanel.Label.Text = "sdfsdfsdf";
                //                    hintPanel.Label.Changed+=LabelOnChanged;
                //                    if (!(viewSiteControl.Parent is Panel))
                //                        throw new NotSupportedException("Your ViewSiteControl should be inside a panel. Modify default.aspx page");
                //                    viewSiteControl.Control.Controls.Add((Control) control);
                //                    XpandModuleBase.Control = viewSiteControl;
                //                }

                var parent = ((ViewSiteControl)GetContainerControl((IViewSiteTemplate)Frame.Template, info.Rule)).Parent;
                var tableCell = (TableCell)parent;
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
}