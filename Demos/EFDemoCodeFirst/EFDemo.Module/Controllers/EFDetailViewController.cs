using System;
using System.Drawing;

using DevExpress.ExpressApp;
using DevExpress.ExpressApp.SystemModule;
using EFDemo.Module.Data;

namespace EFDemo.Module.Controllers {
	public class EFDetailViewController : ViewController {
		private void ObjectSpace_ObjectChanged(Object sender, ObjectChangedEventArgs e) {
			if(
				(e.MemberInfo != null)
				&&
				!e.MemberInfo.IsPersistent
				&&
				(
					e.MemberInfo.MemberType.IsEnum
					||
					typeof(Image).IsAssignableFrom(e.MemberInfo.MemberType)
				)
			) {
				ObjectSpace.SetModified(null);
			}
		}
		private void ObjectSpace_Committed(Object sender, EventArgs e) {
			if(View.CurrentObject is Event) {
				LinkToListViewController linkToListViewController = Frame.GetController<LinkToListViewController>();
				if((linkToListViewController != null) && (linkToListViewController.Link != null)
						&& (linkToListViewController.Link.ListView != null) && linkToListViewController.Link.ListView.IsRoot) {
					linkToListViewController.Link.ListView.ObjectSpace.Refresh();
				}
			}
		}
		protected override void OnActivated() {
			base.OnActivated();
			ObjectSpace.ObjectChanged += new EventHandler<ObjectChangedEventArgs>(ObjectSpace_ObjectChanged);
			ObjectSpace.Committed += new EventHandler(ObjectSpace_Committed);
		}
		protected override void OnDeactivated() {
			base.OnDeactivated();
			ObjectSpace.ObjectChanged -= new EventHandler<ObjectChangedEventArgs>(ObjectSpace_ObjectChanged);
			ObjectSpace.Committed -= new EventHandler(ObjectSpace_Committed);
		}
		public EFDetailViewController() {
			TypeOfView = typeof(DetailView);
		}
	}
}
