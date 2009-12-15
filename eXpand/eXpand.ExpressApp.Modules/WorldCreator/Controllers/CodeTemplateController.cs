using DevExpress.ExpressApp;
using eXpand.ExpressApp.WorldCreator.Observers;

namespace eXpand.ExpressApp.WorldCreator.Controllers {
    public class CodeTemplateController : ViewController
    {
        protected override void OnActivated()
        {
            base.OnActivated();
            new CodeTemplateObserver(ObjectSpace);
        }

    }
}