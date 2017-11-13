using System;
using System.Collections;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.Editors;
using MainDemo.Module.BusinessObjects;

namespace MainDemo.Module.Controllers {
	public partial class ClearContactTasksController : ViewController {
		public ClearContactTasksController() {
			InitializeComponent();
			RegisterActions(components);
		}

		private void ClearTasksAction_Execute(Object sender, SimpleActionExecuteEventArgs e) {
			while(((Contact)View.CurrentObject).Tasks.Count > 0) {
				((Contact)View.CurrentObject).Tasks.Remove(((Contact)View.CurrentObject).Tasks[0]);
			}
			ObjectSpace.SetModified(View.CurrentObject);
		}

		private void ClearTasksController_Activated(object sender, EventArgs e) {
			ClearTasksAction.Enabled.SetItemValue("EditMode", ((DetailView)View).ViewEditMode == ViewEditMode.Edit);
			((DetailView)View).ViewEditModeChanged += new EventHandler<EventArgs>(ClearTasksController_ViewEditModeChanged);
		}
		void ClearTasksController_ViewEditModeChanged(object sender, EventArgs e) {
			ClearTasksAction.Enabled.SetItemValue("EditMode", ((DetailView)View).ViewEditMode == ViewEditMode.Edit);
		}
	}
}
