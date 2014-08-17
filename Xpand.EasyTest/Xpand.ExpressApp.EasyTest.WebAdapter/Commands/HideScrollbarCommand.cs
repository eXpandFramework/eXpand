using DevExpress.EasyTest.Framework;
using mshtml;

namespace Xpand.ExpressApp.EasyTest.WebAdapter.Commands {
    public class HideScrollBarCommand : Xpand.EasyTest.Commands.HideScrollBarCommand {
        protected override void InternalExecute(ICommandAdapter adapter){
            base.InternalExecute(adapter);
            var webAdapter = ((XpandWebCommandAdapter) adapter).WebAdapter;
            var document = webAdapter.GetDocument(webAdapter.WebBrowsers.Count - 1);
            document.body.style.setAttribute("-ms-overflow-style", "none");
        }
    }
}
