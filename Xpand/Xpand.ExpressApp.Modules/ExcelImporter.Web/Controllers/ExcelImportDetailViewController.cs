using System;
using System.IO;
using System.Linq;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.FileAttachments.Web;
using DevExpress.ExpressApp.SystemModule;
using DevExpress.ExpressApp.Web;
using DevExpress.ExpressApp.Web.Editors.ASPx;
using DevExpress.ExpressApp.Web.Templates;
using DevExpress.Web;
using ExcelDataReader;
using Xpand.ExpressApp.ExcelImporter.BusinessObjects;
using Xpand.ExpressApp.ExcelImporter.Services;

namespace Xpand.ExpressApp.ExcelImporter.Web.Controllers{
    public class ExcelImportDetailViewController:ExcelImporter.Controllers.ExcelImportDetailViewController,IXafCallbackHandler{
        private ListPropertyEditor _listPropertyEditor;
        private FileDataPropertyEditor _fileDataPropertyEditor;
        private ASPxGridListEditor _asPxGridListEditor;
        private Exception _exception;

        protected override void OnActivated(){
            base.OnActivated();
            _fileDataPropertyEditor = View.GetItems<FileDataPropertyEditor>().First();
            _fileDataPropertyEditor.ControlCreated += FileDataPropertyEditorOnControlCreated;
            _listPropertyEditor = View.GetItems<ListPropertyEditor>().First(editor => editor.MemberInfo.Name==nameof(BusinessObjects.ExcelImport.ExcelColumnMaps));
            _listPropertyEditor.ControlCreated+=ListPropertyEditorOnControlCreated;
        }

        protected override void OnDeactivated() {
            base.OnDeactivated();
            if (_fileDataPropertyEditor != null)
                _fileDataPropertyEditor.ControlCreated -= FileDataPropertyEditorOnControlCreated;
            if (_listPropertyEditor != null) {
                _listPropertyEditor.ControlCreated -= ListPropertyEditorOnControlCreated;
            }
        }

        protected XafCallbackManager CallbackManager => WebWindow.CurrentRequestPage != null ? ((ICallbackManagerHolder)WebWindow.CurrentRequestPage).CallbackManager : null;

        protected override void OnViewControlsCreated() {
            base.OnViewControlsCreated();
            CallbackManager.RegisterHandler(GetType().FullName, this);
        }

        protected override IObservable<T> Synchronize<T>(T t = default) {
            if (t is Exception exception) {
                _exception = exception;
            }
            return base.Synchronize(t);
        }

        protected override void ShowMapConfigView(ShowViewParameters parameters) {
            base.ShowMapConfigView(parameters);
            parameters.Controllers.OfType<DialogController>().First().Disposed +=
                (_, _) => ObjectSpace.Refresh();
        }

        private void FileDataPropertyEditorOnControlCreated(object sender, EventArgs eventArgs){
            ((FileDataPropertyEditor) sender).Editor.FileDataLoading+=OnFileDataLoading;
        }

        private void ListPropertyEditorOnControlCreated(object sender, EventArgs e) {
            _asPxGridListEditor = ((ASPxGridListEditor) _listPropertyEditor.ListView.Editor);
            _asPxGridListEditor.ControlsCreated+=ASPxGridListEditorOnControlsCreated;
        }

        private void ASPxGridListEditorOnControlsCreated(object sender, EventArgs e) {
            PopulatePropertyNames();
        }

        private void PopulatePropertyNames(){
            var dataComboBoxColumn = _asPxGridListEditor.Grid.Columns
                .OfType<GridViewDataComboBoxColumn>().First(column => column.FieldName == nameof(ExcelColumnMap.PropertyName));
            var comboBox = dataComboBoxColumn.PropertiesComboBox;
            comboBox.DropDownStyle = DropDownStyle.DropDownList;
            var items = comboBox.Items;
            items.Clear();
            items.AddRange(ExcelImport.TypePropertyNames.ToArray());
        }

        private  void ParseStream(UploadedFile uploadedFile) {
            using var stream = ExcelImport.GetXlsContent(uploadedFile.FileName, uploadedFile.FileBytes);
            var temp = new DirectoryInfo(Path.GetTempPath());
            temp = temp.CreateSubdirectory(Application.Title);
            var path = Path.Combine(temp.FullName,$"{ExcelImport.Oid}{Path.GetExtension(uploadedFile.FileName)}");
            using (var fileStream = File.Create(path)){
                stream.CopyTo(fileStream);
            }
            using (var excelDataReader = ExcelReaderFactory.CreateReader(stream)){
                ExcelImport.SheetNames = excelDataReader.Sheets().ToList();
            }
        }

        private void OnFileDataLoading(object sender, FileDataLoadingEventArgs e){
            ParseStream(e.UploadedFile);
        }

        public void ProcessAction(string parameter) {
            if (_exception != null) {
                ErrorHandling.Instance.SetPageError(_exception);
                _exception = null;
            }
        }
    }
}