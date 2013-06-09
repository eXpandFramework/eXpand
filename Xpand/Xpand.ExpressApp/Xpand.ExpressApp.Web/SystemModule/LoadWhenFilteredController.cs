namespace Xpand.ExpressApp.Web.SystemModule
{
    public class LoadWhenFilteredController : ExpressApp.SystemModule.LoadWhenFilteredController
    {
        protected override string GetActiveFilter() {
            return View.Model.Filter;
        }
    }
}
