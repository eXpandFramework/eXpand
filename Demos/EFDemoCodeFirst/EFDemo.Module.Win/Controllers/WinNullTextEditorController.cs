using System;
using System.Collections.Generic;

using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Editors;
using DevExpress.XtraEditors;
using DevExpress.ExpressApp.Utils;

namespace EFDemo.Module.Win.Controllers {
    public partial class WinNullTextEditorController : ViewController {
        public WinNullTextEditorController() {
            InitializeComponent();
            RegisterActions(components);
        }
        private void InitNullText(PropertyEditor propertyEditor) {
            ((BaseEdit)propertyEditor.Control).Properties.NullText = CaptionHelper.NullValueText;
        }
        private void WinNullTextEditorController_Activated(object sender, EventArgs e) {
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
        private void propertyEditor_ControlCreated(object sender, EventArgs e) {
            InitNullText((PropertyEditor)sender);
        }
    }
}
