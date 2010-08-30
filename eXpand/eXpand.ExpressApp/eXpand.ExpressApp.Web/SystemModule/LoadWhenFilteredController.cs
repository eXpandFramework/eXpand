using DevExpress.ExpressApp.Web.SystemModule;

namespace eXpand.ExpressApp.Web.SystemModule
{
    public class LoadWhenFilteredController : ExpressApp.SystemModule.LoadWhenFilteredController
    {
        protected override string GetActiveFilter() {
            return ((IModelListViewWeb)View.Model).FilterExpression;
        }
    }
}
