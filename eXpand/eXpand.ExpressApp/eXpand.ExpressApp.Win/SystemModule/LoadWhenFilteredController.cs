using DevExpress.ExpressApp.Win.SystemModule;

namespace eXpand.ExpressApp.Win.SystemModule {
    public class LoadWhenFilteredController : ExpressApp.SystemModule.LoadWhenFilteredController
    {
        protected override string GetActiveFilter() {
            return ((IModelListViewWin)View.Model).ActiveFilterString;
        }
    }
}