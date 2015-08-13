using System;
using DevExpress.ExpressApp.FileAttachments.Web;
using Xpand.Persistent.Base.General.Controllers;

namespace Xpand.ExpressApp.FileAttachment.Web {
    public class UploadControlPropertyEditorProviderController : Persistent.Base.General.Controllers.UploadControlPropertyEditorProviderController {
        protected override void OnActivated() {
            base.OnActivated();
            foreach (var fileDataPropertyEditor in View.GetItems<FileDataPropertyEditor>()) {
                fileDataPropertyEditor.ControlCreated += FileDataPropertyEditorOnControlCreated;
            }
        }

        void FileDataPropertyEditorOnControlCreated(object sender, EventArgs eventArgs) {
            var fileDataPropertyEditor = ((FileDataPropertyEditor) sender);
            fileDataPropertyEditor.ControlCreated-=FileDataPropertyEditorOnControlCreated;
            EventHandler[] eventHandlers = {null};
            eventHandlers[0] = (o, args) => {
                var uploadControl = fileDataPropertyEditor.Editor.UploadControl;
                var controlProvider = new ASPxPropertyEditorUploadControlProvider(uploadControl, fileDataPropertyEditor);
                OnUploadControlProviderCreated(new ASPxPropertyEditorUploadControlProviderArgs(controlProvider));
                fileDataPropertyEditor.Editor.Load -= eventHandlers[0];
            };
            if (fileDataPropertyEditor.Editor != null) fileDataPropertyEditor.Editor.Load += eventHandlers[0];
        }

    }

}
