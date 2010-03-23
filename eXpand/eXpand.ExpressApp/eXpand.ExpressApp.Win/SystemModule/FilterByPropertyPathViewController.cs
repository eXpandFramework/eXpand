using System.Drawing;
using System.Windows.Forms;
using DevExpress.ExpressApp.Win.Editors;
using DevExpress.Utils.Frames;
using View = DevExpress.ExpressApp.View;

namespace eXpand.ExpressApp.Win.SystemModule {
    public partial class FilterByPropertyPathViewController : ExpressApp.SystemModule.FilterByPropertyPathViewController {
        public FilterByPropertyPathViewController() {
            InitializeComponent();
            RegisterActions(components);
        }

        protected override string FilterStringAttributeName {
            get { return GridListEditor.ActiveFilterString; }
        }

        protected override void AddFilterPanel(string text, object viewSiteControl) {
            var filterPanel = new FilterPanel {
                                                  BackColor = Color.LightGoldenrodYellow,
                                                  Dock = DockStyle.Bottom,
                                                  MaxRows = 25,
                                                  TabIndex = 0,
                                                  TabStop = false,
                                                  MinimumSize = new Size(350, 33),
                                                  Text = text
                                              };
            Control.ControlCollection collection = ((Control) viewSiteControl).Controls;
            collection.Add(filterPanel);
        }

        protected override void SynchronizeInfo(View view) {
            view.SynchronizeInfo();
        }
        #region Nested type: FilterPanel
        public class FilterPanel : NotePanel8_1 {
        }
        #endregion
    }
}