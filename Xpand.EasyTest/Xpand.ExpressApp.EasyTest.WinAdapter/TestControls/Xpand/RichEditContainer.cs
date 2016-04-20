using DevExpress.EasyTest.Framework;
using DevExpress.ExpressApp.EasyTest.WinAdapter.TestControls;

namespace Xpand.ExpressApp.EasyTest.WinAdapter.TestControls.Xpand {
    public class RichEditContainer: TestControlTextValidated<Win.PropertyEditors.RichEdit.RichEditContainer>, IControlText {
        public RichEditContainer(Win.PropertyEditors.RichEdit.RichEditContainer control) : base(control){
        }

        protected override string GetText(){
            return control.RichEditControl.Text;
        }

        protected override void InternalSetText(string text){
            control.RichEditControl.Text = text;
        }
    }
}
