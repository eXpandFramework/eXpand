using DevExpress.EasyTest.Framework;

namespace Xpand.ExpressApp.EasyTest.WebAdapter.Commands {
    public class HideScrollBarCommand : Xpand.EasyTest.Commands.HideScrollBarCommand {
        protected override void InternalExecute(ICommandAdapter adapter){
            base.InternalExecute(adapter);
            var webAdapter = ((XpandWebCommandAdapter) adapter).WebAdapter;
            var document = webAdapter.GetDocument();
            document.body.style.setAttribute("-ms-overflow-style", "none");
        }
    }
}
