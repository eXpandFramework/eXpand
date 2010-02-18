using System.Web.UI;
using DevExpress.ExpressApp;

namespace eXpand.ExpressApp.Web.SystemModule {
    public partial class HideToolBarController : ViewController {
        public const string HideToolBarAttributeName = "HideToolBar";
        public HideToolBarController() {
            InitializeComponent();
            RegisterActions(components);
        }

        protected override void OnViewControlsCreated() {
            base.OnViewControlsCreated();
            if (Frame.Template != null) {
                Control control = ((Control)Frame.Template).FindControl("ToolBar");
                if (control != null) control.Visible = !View.Info.GetAttributeBoolValue(HideToolBarAttributeName);
            }
        }

        public override Schema GetSchema()
        {
            const string CommonTypeInfos =
                @"<Element Name=""Application"">
                    <Element Name=""Views"" >
                        <Element Name=""ListView"" >
                            <Attribute Name=""" +HideToolBarAttributeName +@""" Choice=""Default,AlwaysEnable""/>
                        </Element>
                        <Element Name=""DetailView"" >
                            <Attribute Name=""" +HideToolBarAttributeName +@""" Choice=""Default,AlwaysEnable""/>
                        </Element>
                    </Element>
                </Element>";

            return new Schema(new DictionaryXmlReader().ReadFromString(CommonTypeInfos));
        }
    }
}