using System;
using System.Linq;
using System.Collections.Generic;

using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.Editors;
using EFDemo.Module.Data;

namespace EFDemo.Module.Controllers {
	public partial class ClearContactTasksController : ViewController {
		private void ClearTasksController_Activated(Object sender, EventArgs e) {
			ClearTasksAction.Enabled.SetItemValue("EditMode", ((DetailView)View).ViewEditMode == ViewEditMode.Edit);
			((DetailView)View).ViewEditModeChanged += new EventHandler<EventArgs>(ClearTasksController_ViewEditModeChanged);
		}
		private void ClearTasksController_ViewEditModeChanged(Object sender, EventArgs e) {
			ClearTasksAction.Enabled.SetItemValue("EditMode", ((DetailView)View).ViewEditMode == ViewEditMode.Edit);
		}
		private void ClearTasksAction_Execute(Object sender, SimpleActionExecuteEventArgs e) {
			((Contact)View.CurrentObject).Tasks.Clear();
			((DetailView)View).FindItem("Tasks").Refresh();
			ObjectSpace.SetModified(View.CurrentObject);
		}
		public ClearContactTasksController() {
			InitializeComponent();
			RegisterActions(components);
		}
	}
}
