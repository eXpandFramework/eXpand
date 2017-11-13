using System;

using DevExpress.XtraEditors;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Utils;

namespace MainDemo.Module.Win.Controllers {
	public partial class WinNullTextEditorController : ViewController {
		private void InitNullText(PropertyEditor propertyEditor) {
			((BaseEdit)propertyEditor.Control).Properties.NullText = CaptionHelper.NullValueText;
		}
		private void propertyEditor_ControlCreated(Object sender, EventArgs e) {
			InitNullText((PropertyEditor)sender);
		}
		private void WinNullTextEditorController_ItemsChanged(Object sender, ViewItemsChangedEventArgs e) {
			if((e.ChangedType == ViewItemsChangedType.Added) && (e.Item.Id == "Anniversary")) {
				TryInitializeAnniversaryItem();
			}
		}

		protected override void OnActivated() {
			base.OnActivated();
			((CompositeView)View).ItemsChanged += WinNullTextEditorController_ItemsChanged;
			TryInitializeAnniversaryItem();
		}
		protected override void OnDeactivated() {
			base.OnDeactivated();
			((CompositeView)View).ItemsChanged -= WinNullTextEditorController_ItemsChanged;
			ViewItem propertyEditor = ((DetailView)View).FindItem("Anniversary");
			if(propertyEditor != null) {
				propertyEditor.ControlCreated -= new EventHandler<EventArgs>(propertyEditor_ControlCreated);
			}
		}

		public WinNullTextEditorController() {
			InitializeComponent();
			RegisterActions(components);
		}
		public void TryInitializeAnniversaryItem() {
			PropertyEditor propertyEditor = ((DetailView)View).FindItem("Anniversary") as PropertyEditor;
			if(propertyEditor != null) {
				if(propertyEditor.Control != null) {
					InitNullText(propertyEditor);
				}
				else {
					propertyEditor.ControlCreated += new EventHandler<EventArgs>(propertyEditor_ControlCreated);
				}
			}
		}
	}
}
