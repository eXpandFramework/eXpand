using System.Web.UI;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Web.Editors.ASPx;
using DevExpress.Web.ASPxClasses;
using DevExpress.Web.ASPxEditors;
using DevExpress.Web.ASPxPanel;

namespace eXpand.ExpressApp.Web.SystemModule
{
    public partial class FilterByPropertyPathViewController : ExpressApp.SystemModule.FilterByPropertyPathViewController
    {
        public FilterByPropertyPathViewController()
        {
            InitializeComponent();
            RegisterActions(components);
        }
        public class FilterPanel : ASPxPanel
        {
            private readonly ASPxLabel label;
            public FilterPanel()
            {
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
            ControlCollection collection = ((Control)viewSiteControl).Controls;
            var filterPanel = new FilterPanel();
            filterPanel.Label.Text = text;            
            collection.Add(filterPanel);
        }

        protected override void SynchronizeInfo(View view) {
            var listEditor = ( ((ListView) view).Editor);
            var editor = listEditor as ASPxGridListEditor;
            if (editor != null) {
                var dictionaryNode = editor.GetUserDiffs((Control)listEditor.Control);
                view.Info.CombineWith(dictionaryNode);
            }
        }

        protected override string FilterStringAttributeName {
            get { return ASPxGridViewSettingsInfoNodeWrapper.FilterExpressionAttribute; }
        }
    }
}
