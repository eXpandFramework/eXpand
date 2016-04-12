using System.Web.UI;
using DevExpress.Web;

namespace Xpand.ExpressApp.Web.SystemModule {
    public class FilterByPropertyPathViewController : ExpressApp.SystemModule.FilterByPropertyPathViewController {
        public class FilterPanel : ASPxPanel {
            private readonly ASPxLabel label;
            public FilterPanel() {
                Paddings.PaddingBottom = 8;

                var innerHintPanel = new ASPxPanel();
                innerHintPanel.Paddings.Assign(new Paddings(8, 8, 8, 8));
                innerHintPanel.BackColor = System.Drawing.Color.LightGoldenrodYellow;
                Controls.Add(innerHintPanel);
                label = new ASPxLabel();
                innerHintPanel.Controls.Add(label);
            }

            public ASPxLabel Label {
                get { return label; }
            }
        }

        protected override void AddFilterPanel(string text, object viewSiteControl) {
            if (!string.IsNullOrEmpty(text)) {
                ControlCollection collection = ((Control)viewSiteControl).Controls;
                var filterPanel = new FilterPanel();
                filterPanel.Label.Text = text;
                collection.Add(filterPanel);
            }
        }

        protected override string GetActiveFilter(DevExpress.ExpressApp.Model.IModelListView modelListView) {
            return modelListView.Filter;
        }

        protected override void SetActiveFilter(DevExpress.ExpressApp.Model.IModelListView modelListView, string filter) {
            modelListView.Filter = filter;
        }
    }
}
