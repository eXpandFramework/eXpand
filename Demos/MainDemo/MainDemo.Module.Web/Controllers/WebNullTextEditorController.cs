using System;

using DevExpress.Web;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Web.Editors;
using DevExpress.ExpressApp.Utils;
using DevExpress.ExpressApp.Editors;

namespace MainDemo.Module.Web.Controllers {
    public partial class WebNullTextEditorController : ViewController {
		private void InitNullText(WebPropertyEditor propertyEditor) {
			if(propertyEditor.ViewEditMode == DevExpress.ExpressApp.Editors.ViewEditMode.Edit) {
				((ASPxDateEdit)propertyEditor.Editor).NullText = CaptionHelper.NullValueText;
			}
		}
		private void propertyEditor_ControlCreated(object sender, EventArgs e) {
			InitNullText((WebPropertyEditor)sender);
		}

		protected override void OnActivated() {
			base.OnActivated();
			WebPropertyEditor propertyEditor = ((DetailView)View).FindItem("Anniversary") as WebPropertyEditor;
			if(propertyEditor != null) {
				if(propertyEditor.Control != null) {
					InitNullText(propertyEditor);
				}
				else {
					propertyEditor.ControlCreated += new EventHandler<EventArgs>(propertyEditor_ControlCreated);
				}
			}
		}
		protected override void OnDeactivated() {
			base.OnDeactivated();
			ViewItem propertyEditor = ((DetailView)View).FindItem("Anniversary");
			if(propertyEditor != null) {
				propertyEditor.ControlCreated -= new EventHandler<EventArgs>(propertyEditor_ControlCreated);
			}
		}

		public WebNullTextEditorController() {
            InitializeComponent();
            RegisterActions(components);
        }
    }
}
