using System.Drawing;
using System.Windows.Forms;
using DevExpress.Data.Filtering;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Win.Editors;
using DevExpress.Utils.Frames;
using View = DevExpress.ExpressApp.View;
using System.Linq;

namespace Xpand.ExpressApp.Win.SystemModule {
    public class FilterByPropertyPathViewController : ExpressApp.SystemModule.FilterByPropertyPathViewController {
        protected override string GetActiveFilter(IModelListView modelListView) {
            return modelListView.Filter;
        }

        protected override void SetActiveFilter(IModelListView modelListView, string filter) {
            modelListView.Filter = filter;
        }

        protected override void AddFilterPanel(string text, object viewSiteControl) {
            Control.ControlCollection controlCollection = ((Control)viewSiteControl).Controls;
            if (string.IsNullOrEmpty(text)) {
                FilterPanel filterPanel = controlCollection.OfType<FilterPanel>().FirstOrDefault();
                if (filterPanel != null) controlCollection.Remove(filterPanel);
            } else {
                var filterPanel = new FilterPanel {
                    BackColor = Color.LightGoldenrodYellow,
                    Dock = DockStyle.Bottom,
                    MaxRows = 25,
                    TabIndex = 0,
                    TabStop = false,
                    MinimumSize = new Size(350, 33),
                    Text = text
                };

                Control.ControlCollection collection = controlCollection;
                collection.Add(filterPanel);
            }
        }

        protected override void OnPropertyPathFilterParsed(CriteriaOperator criteriaOperator) {
            new AsyncServerModeCriteriaProccessor(View.ObjectTypeInfo).Process(criteriaOperator);
        }

        protected override void SynchronizeInfo(View view) {
            view.SaveModel();
        }

        #region Nested type: FilterPanel
        public class FilterPanel : NotePanel8_1 {
        }
        #endregion
    }
}