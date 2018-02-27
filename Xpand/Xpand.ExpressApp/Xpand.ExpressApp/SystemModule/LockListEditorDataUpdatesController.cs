using System;
using DevExpress.ExpressApp;

namespace Xpand.ExpressApp.SystemModule{
    public interface IListEditorLocker:IDisposable{
    }

    public static class LockListEditorExtension{
        public static void UpdateListEditor(this Controller controller,Action action){
            using (var listEditorLocker = controller.Frame?.GetController<LockListEditorDataUpdatesController>().ListEditorLocker){
                if (listEditorLocker != null)
                    action();
            }
        }
    }
    public abstract class LockListEditorDataUpdatesController:ViewController<ListView>{
        public abstract IListEditorLocker ListEditorLocker{ get; }
    }
}