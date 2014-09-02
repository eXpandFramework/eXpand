using System;

using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.Editors;
#if (CodeFirst)
using DevExpress.Persistent.BaseImpl.EF;
#endif
using EFDemo.Module.Data;

namespace EFDemo.Module.Controllers {
	public partial class PopupNotesController : ViewController {
        private void ShowNotesAction_Execute(Object sender, PopupWindowShowActionExecuteEventArgs args) {
            DemoTask task = (DemoTask)View.CurrentObject;
            View.ObjectSpace.SetModified(task);
            foreach(Note note in args.PopupWindow.View.SelectedObjects) {
                if(!String.IsNullOrEmpty(task.Description)) {
                    task.Description += Environment.NewLine;
                }
                task.Description += note.Text;
            }
            ViewItem item = ((DetailView)View).FindItem("Description");
            ((PropertyEditor)item).ReadValue();
            if(View is DetailView && ((DetailView)View).ViewEditMode == ViewEditMode.View) {
                View.ObjectSpace.CommitChanges();
            }
        }
        private void ShowNotesAction_CustomizePopupWindowParams(Object sender, CustomizePopupWindowParamsEventArgs args) {
            IObjectSpace objectSpace = Application.CreateObjectSpace();
            args.View = Application.CreateListView(Application.FindListViewId(typeof(Note)), new CollectionSource(objectSpace, typeof(Note)), true);
        }
        public PopupNotesController()
			: base() {
			InitializeComponent();
			RegisterActions(components);
		}
	}
}
