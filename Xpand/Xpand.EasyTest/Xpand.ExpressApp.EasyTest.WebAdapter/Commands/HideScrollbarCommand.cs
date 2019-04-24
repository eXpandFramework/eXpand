using DevExpress.EasyTest.Framework;
using mshtml;

namespace Xpand.ExpressApp.EasyTest.WebAdapter.Commands {
    public class HideScrollBarCommand : Xpand.EasyTest.Commands.HideScrollBarCommand {
        protected override void InternalExecute(ICommandAdapter adapter){
            base.InternalExecute(adapter);
            var webAdapter = ((XpandWebCommandAdapter) adapter).WebBrowser;
            var webBrowser2 = webAdapter.Browser;
            var document = (IHTMLDocument2)webBrowser2.Document;
            document.body.style.setAttribute("-ms-overflow-style", "none");
        }
    }
}
