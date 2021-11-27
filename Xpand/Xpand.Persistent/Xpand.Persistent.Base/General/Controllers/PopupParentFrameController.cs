using System;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Editors;

namespace Xpand.Persistent.Base.General.Controllers {
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "XAF0004:Implement XAF Controller constructors correctly", Justification = "<Pending>")]
    public class PopupParentFrameController :ViewController<ObjectView>{
        public PopupParentFrameController(Frame parentFrame){
            ParentFrame = parentFrame;
        }

        protected override void OnActivated(){
            base.OnActivated();
            foreach (var item in View.GetItems<ListPropertyEditor>()){
                item.ControlCreated+=ItemOnControlCreated;
            }
        }

        private void ItemOnControlCreated(object sender, EventArgs eventArgs){
            ((ListPropertyEditor) sender).Frame.RegisterController(new PopupParentFrameController(ParentFrame));
        }

        public Frame ParentFrame { get; }
    }
}
