using System;
using System.Data;
using System.IO;
using System.Linq;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.FileAttachments.Web;
using DevExpress.ExpressApp.Web.Editors.ASPx;
using DevExpress.Web;
using ExcelDataReader;
using Xpand.ExpressApp.ExcelImporter.Controllers;

namespace Xpand.ExpressApp.ExcelImporter.Web.Controllers{
    public class ExcelImportDetailViewController:ExcelImporter.Controllers.ExcelImportDetailViewController{
        private ListPropertyEditor _listPropertyEditor;
        private FileDataPropertyEditor _fileDataPropertyEditor;
        private ASPxGridListEditor _asPxGridListEditor;

        protected override void OnActivated(){
            base.OnActivated();
            if (View is DetailView detailView){
                _fileDataPropertyEditor = detailView.GetItems<FileDataPropertyEditor>().First();
                _fileDataPropertyEditor.ControlCreated += FileDataPropertyEditorOnControlCreated;
                _listPropertyEditor = detailView.GetItems<ListPropertyEditor>().First(editor => editor.MemberInfo.Name==nameof(BusinessObjects.ExcelImport.ExcelColumnMaps));
                _listPropertyEditor.ControlCreated+=ListPropertyEditorOnControlCreated;
            }
        }

        private void ListPropertyEditorOnControlCreated(object sender, EventArgs e) {
            _asPxGridListEditor = ((ASPxGridListEditor) _listPropertyEditor.ListView.Editor);
            _asPxGridListEditor.ControlsCreated+=ASPxGridListEditorOnControlsCreated;
            
        }

        private void ASPxGridListEditorOnControlsCreated(object sender, EventArgs e) {
            var items = _asPxGridListEditor.Grid.Columns
                .OfType<GridViewDataComboBoxColumn>().First().PropertiesComboBox.Items;
            items.Clear();
            items.AddRange(ExcelImport.TypePropertyNames);
        }

        protected override void OnDeactivated() {
            base.OnDeactivated();
            if (_listPropertyEditor != null) {
                _listPropertyEditor.ControlCreated -= ListPropertyEditorOnControlCreated;
                _fileDataPropertyEditor.ControlCreated-=FileDataPropertyEditorOnControlCreated;
            }
        }

        private void FileDataPropertyEditorOnControlCreated(object sender, EventArgs eventArgs){
            ((FileDataPropertyEditor) sender).Editor.FileDataLoading+=OnFileDataLoading;
        }

        private  void ParseStream(UploadedFile uploadedFile) {
            using (var stream = ExcelImport.GetXlsContent(uploadedFile.FileName, uploadedFile.FileBytes)){
                using (var excelDataReader = ExcelReaderFactory.CreateReader(stream)){
                    using (var dataSet = excelDataReader.GetDataSet( ExcelImport)){
                        ExcelImport.SheetNames = dataSet.Tables.Cast<DataTable>().Select(table => table.TableName).ToList();
                    }
                }
            }
        }

        private void OnFileDataLoading(object sender, FileDataLoadingEventArgs e){
            ParseStream(e.UploadedFile);
        }
    }
}