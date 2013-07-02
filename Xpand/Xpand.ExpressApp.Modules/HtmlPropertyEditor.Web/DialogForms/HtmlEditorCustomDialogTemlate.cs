using System;
using System.IO;
using System.Web.UI;
using System.Web.UI.WebControls;
using DevExpress.Web.ASPxClasses.Internal;
using DevExpress.Web.ASPxEditors;
using DevExpress.Web.ASPxHtmlEditor;
using DevExpress.Web.ASPxUploadControl;
using Xpand.Utils.Helpers;

namespace Xpand.ExpressApp.HtmlPropertyEditor.Web.DialogForms {
    public class HtmlEditorCustomDialogTemlate : HtmlEditorUserControl {
        ASPxUploadControl _uploadControl;

        protected override void Render(HtmlTextWriter writer) {
            var internalTable = (InternalTable)Parent.Parent.Controls[1].Controls[0];
            var tableCell = (ASPxButton)internalTable.Rows[0].Cells[0].Controls[0];
            tableCell.ClientSideEvents.Click = typeof(HtmlEditorCustomDialogTemlate).GetDxScriptFromResource("OnOkButtonClick_InsertFileForm.js");
            base.Render(writer);
        }

        protected override void OnInit(EventArgs e) {
            base.OnInit(e);
            _uploadControl = new ASPxUploadControl { ID = "uploadFile", Width = Unit.Pixel(300), ClientInstanceName = "_dxeUplFile" };
            _uploadControl.ClientSideEvents.FileUploadComplete = typeof(HtmlEditorCustomDialogTemlate).GetDxScriptFromResource("EditorFileUploadComplete.js");
            _uploadControl.FileUploadComplete += UploadControlOnFileUploadComplete;
            Controls.Add(_uploadControl);
        }

        protected bool HasFile() {
            return _uploadControl.UploadedFiles != null && _uploadControl.UploadedFiles.Length > 0 && _uploadControl.UploadedFiles[0].FileName != "";
        }

        protected string SaveUploadFile() {
            string fileName = "";
            if (HasFile()) {
                string uploadFolder = HtmlEditor.SettingsImageUpload.UploadImageFolder;
                fileName = MapPath(uploadFolder) + _uploadControl.UploadedFiles[0].FileName;
                try {
                    _uploadControl.UploadedFiles[0].SaveAs(fileName, false);
                } catch (IOException) {
                    fileName = MapPath(uploadFolder) + _uploadControl.GetRandomFileName();
                    _uploadControl.UploadedFiles[0].SaveAs(fileName);
                }
            }
            return Path.GetFileName(fileName);
        }

        void UploadControlOnFileUploadComplete(object sender, FileUploadCompleteEventArgs args) {
            try {
                args.CallbackData = SaveUploadFile() + "," + Path.GetFileNameWithoutExtension(_uploadControl.UploadedFiles[0].FileName);
            } catch (Exception e) {
                args.IsValid = false;
                args.ErrorText = e.Message;
            }
        }
    }
}
