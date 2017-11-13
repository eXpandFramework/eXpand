using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.Editors;
using DevExpress.Persistent.BaseImpl;
using MainDemo.Module.BusinessObjects;
using System;

namespace MainDemo.Module.Controllers {
	public partial class PopupNotesController : ViewController {
		public PopupNotesController()
			: base() {
			InitializeComponent();
			RegisterActions(components);
		}
		private void ShowNotesAction_Execute(object sender, PopupWindowShowActionExecuteEventArgs args) {
			DemoTask task = (DemoTask)View.CurrentObject;
			foreach(Note note in args.PopupWindowViewSelectedObjects) {
				if(!string.IsNullOrEmpty(task.Description)) {
					task.Description += Environment.NewLine;
				}
				task.Description += note.Text;
			}
            if(((DetailView)View).ViewEditMode == ViewEditMode.View) {
				View.ObjectSpace.CommitChanges();
			}
		}
		private void ShowNotesAction_CustomizePopupWindowParams(object sender, CustomizePopupWindowParamsEventArgs args) {
			IObjectSpace objectSpace = Application.CreateObjectSpace(typeof(Note));
            string noteListViewId = Application.FindLookupListViewId(typeof(Note));
            CollectionSourceBase collectionSource = Application.CreateCollectionSource(objectSpace, typeof(Note), noteListViewId);
            args.View = Application.CreateListView(noteListViewId, collectionSource, true);
		}
	}
}
