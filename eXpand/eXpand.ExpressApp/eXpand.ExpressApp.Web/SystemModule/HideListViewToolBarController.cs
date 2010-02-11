using System.Web.UI;
using DevExpress.ExpressApp;
using eXpand.ExpressApp.Core.DictionaryHelpers;

namespace eXpand.ExpressApp.Web.SystemModule {
    public partial class HideListViewToolBarController : ViewController<ListView> {
        public const string HideToolBarAttributeName = "HideToolBar";
        public HideListViewToolBarController() {
            InitializeComponent();
            RegisterActions(components);
        }

        protected override void OnViewControlsCreated() {
            base.OnViewControlsCreated();
            if (Frame.Template != null) {
                Control control = ((Control)Frame.Template).FindControl("ToolBar");
                control.Visible = !View.Info.GetAttributeBoolValue(HideToolBarAttributeName);
            }
        }

        public override Schema GetSchema()
        {
            DictionaryNode injectAttribute = new SchemaHelper().InjectAttribute(HideToolBarAttributeName, ModelElement.ListView);
            return new Schema(injectAttribute);
        }
    }
}