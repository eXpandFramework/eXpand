using DevExpress.ExpressApp.Win.Editors;
using DevExpress.XtraGrid;
using Xpand.ExpressApp.SystemModule;

namespace Xpand.ExpressApp.Win.SystemModule{
    public class LockListEditorDataUpdatesController : ExpressApp.SystemModule.LockListEditorDataUpdatesController{
        public override IListEditorLocker ListEditorLocker => new ListEditorLocker((GridListEditor) View.Editor);
    }

    internal class ListEditorLocker : IListEditorLocker{
        private readonly GridControl _gridControl;

        public ListEditorLocker(GridListEditor gridListEditor){
            _gridControl = gridListEditor.Grid;
            _gridControl.MainView.BeginDataUpdate();
            
        }

        public void Dispose(){
            _gridControl.MainView.EndDataUpdate();
        }
    }
}