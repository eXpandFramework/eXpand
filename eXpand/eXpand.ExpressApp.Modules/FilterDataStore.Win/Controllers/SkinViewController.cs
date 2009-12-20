using DevExpress.ExpressApp;
using eXpand.ExpressApp.FilterDataStore.Win.Providers;

namespace eXpand.ExpressApp.FilterDataStore.Win.Controllers
{
    public partial class SkinViewController : ViewController
    {
        public SkinViewController()
        {
            InitializeComponent();
            RegisterActions(components);
        }
        protected override void OnActivated()
        {
            base.OnActivated();
            SkinFilterProvider.Skin = Application.Model.RootNode.FindChildNode("Options").GetAttributeValue("Skin");
        }
    }
}
