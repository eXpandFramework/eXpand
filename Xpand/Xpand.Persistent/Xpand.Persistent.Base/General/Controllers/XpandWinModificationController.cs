using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.SystemModule;

namespace Xpand.Persistent.Base.General.Controllers {
    public class XpandWinModificationController:ViewController<ListView>{
        private readonly SimpleAction _editAction;

        public XpandWinModificationController(){
            _editAction = new SimpleAction(this,"Edit","EasyTest");
            _editAction.Execute+=EditActionOnExecute;
        }

        public SimpleAction EditAction{
            get { return _editAction; }
        }

        private void EditActionOnExecute(object sender, SimpleActionExecuteEventArgs simpleActionExecuteEventArgs){
            Frame.GetController<ListViewProcessCurrentObjectController>().ProcessCurrentObjectAction.DoExecute();
        }
    }
}
