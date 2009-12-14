using DevExpress.ExpressApp;
using eXpand.ExpressApp.WorldCreator.Observers;

namespace eXpand.ExpressApp.WorldCreator.Controllers {
    public class CodeTemplateController : ViewController
    {
        protected override void OnViewChanging(View view)
        {
            base.OnViewChanging(view);
            new CodeTemplateObserver(view.ObjectSpace);
        }


    }
}