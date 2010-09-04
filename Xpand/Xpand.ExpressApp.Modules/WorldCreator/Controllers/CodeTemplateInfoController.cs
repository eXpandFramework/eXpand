using DevExpress.ExpressApp;
using Xpand.ExpressApp.WorldCreator.Observers;

namespace Xpand.ExpressApp.WorldCreator.Controllers {
    public class CodeTemplateInfoController : ViewController
    {
        protected override void OnActivated()
        {
            base.OnActivated();
            new CodeTemplateInfoObserver(ObjectSpace);
        }
    }
}