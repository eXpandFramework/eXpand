using System.Web.UI;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Web.Editors.ASPx;

namespace eXpand.ExpressApp.Web.SystemModule
{
    public partial class FilterByPropertyPathViewController : ExpressApp.SystemModule.FilterByPropertyPathViewController
    {
        public FilterByPropertyPathViewController()
        {
            InitializeComponent();
            RegisterActions(components);
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
