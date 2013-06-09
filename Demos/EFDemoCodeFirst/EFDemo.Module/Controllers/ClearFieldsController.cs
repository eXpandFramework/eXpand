using System;
using System.Linq;

using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Editors;

namespace EFDemo.Module.Controllers {
	public partial class ClearFieldsController : ViewController {
		private void ClearFieldsController_Activated(Object sender, EventArgs e) {
			ClearFieldsAction.Enabled.SetItemValue("EditMode", ((DetailView)View).ViewEditMode == ViewEditMode.Edit);
			((DetailView)View).ViewEditModeChanged += new EventHandler<EventArgs>(ClearFieldsController_ViewEditModeChanged);
		}
		private void ClearFieldsController_ViewEditModeChanged(Object sender, EventArgs e) {
			ClearFieldsAction.Enabled.SetItemValue("EditMode", ((DetailView)View).ViewEditMode == ViewEditMode.Edit);
		}
		private void ClearFieldsAction_Execute(Object sender, SimpleActionExecuteEventArgs e) {
			foreach(PropertyEditor item in ((DetailView)View).GetItems<PropertyEditor>()) {
				if(item.AllowEdit) {
					try {
						item.PropertyValue = null;
					}
					catch(IntermediateMemberIsNullException) {
						item.Refresh();
					}
				}
			}
		}
		public ClearFieldsController() {
			InitializeComponent();
			RegisterActions(components);
		}
	}
}
