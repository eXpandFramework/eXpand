using DevExpress.ExpressApp;
using Xpand.ExpressApp.WorldCreator.System.Observers;

namespace Xpand.ExpressApp.WorldCreator.Controllers{
    public class CodeTemplateInfoController : ViewController{
        protected override void OnActivated(){
            base.OnActivated();
            new CodeTemplateInfoObserver(ObjectSpace);
        }
    }
}