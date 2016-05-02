using System;
using System.ComponentModel;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Web;
using DevExpress.Persistent.Base;
using Xpand.Persistent.Base.General.Model;
using Xpand.Persistent.Base.General.Model.VisibilityCalculators;

namespace Xpand.ExpressApp.FileAttachment.Web.PropertyEditors{
    public interface IModelPropertyEditorFileData{
        [Category(AttributeCategoryNameProvider.Xpand)]
        [ModelBrowsable(typeof(EditorTypeVisibilityCalculator<FileDataPropertyEditorController, IModelDetailView>))]
        string AttachmentImage { get; set; }
    }
    public class FileDataPropertyEditorController:Controller,IModelExtender {
        public void ExtendModelInterfaces(ModelInterfaceExtenders extenders){
            extenders.Add<IModelPropertyEditor, IModelPropertyEditorFileData>();
        }
    }
    [PropertyEditor(typeof(IFileData), true)]
    public class FileDataPropertyEditor : DevExpress.ExpressApp.FileAttachments.Web.FileDataPropertyEditor{
        public FileDataPropertyEditor(Type objectType, IModelMemberViewItem info) : base(objectType, info){
        }

        private IFileData FileData{
            get { return MemberInfo.GetValue(CurrentObject) as IFileData; }
        }

        protected override WebControl CreateEditModeControlCore(){
            if (!string.IsNullOrWhiteSpace(((IModelPropertyEditorFileData)Model).AttachmentImage)){
                var fileDataEdit = new FileDataEdit(ViewEditMode, FileData, !AllowEdit, ImmediatePostData);
                fileDataEdit.CreateCustomFileDataObject +=CreateFileDataObject;
                return fileDataEdit;
            }
            return base.CreateEditModeControlCore();
        }
    }

    public class FileDataEdit : DevExpress.ExpressApp.FileAttachments.Web.FileDataEdit{
        public FileDataEdit(ViewEditMode mode, IFileData fileData, bool readOnly,bool postDataImmediatelly) : base(mode, fileData, readOnly, postDataImmediatelly){
        }

        protected override void OnPreRender(EventArgs e){
            base.OnPreRender(e);
            var table = (TableEx) Rows[0].Cells[0].Controls[0];
            var fileAnchor = (HtmlAnchor) table.Rows[0].Cells[0].Controls[0];
            fileAnchor.Controls.Clear();
            fileAnchor.Controls.Add(new Literal(){
                Text = "<img src=\"\" />"
            });
        }
    }
}