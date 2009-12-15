using DevExpress.ExpressApp;
using eXpand.ExpressApp.WorldCreator.Observers;

namespace eXpand.ExpressApp.WorldCreator.Controllers {
    public class CodeTemplateInfoController : ViewController
    {
        protected override void OnActivated()
        {
            base.OnActivated();
            new CodeTemplateInfoObserver(ObjectSpace);
        }
    }
}