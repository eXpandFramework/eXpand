using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Win.SystemModule;
using eXpand.ExpressApp.FilterDataStore.Win.Providers;

namespace eXpand.ExpressApp.FilterDataStore.Win.Controllers
{
    public class SkinViewController : ViewController
    {
        public SkinViewController() { }

        protected override void OnActivated()
        {
            base.OnActivated();
            SkinFilterProvider.Skin = ((IModelApplicationOptionsSkin)Application.Model.Options).Skin;
        }
    }
}
