using System;
using System.ComponentModel;
using System.Data;
using System.IO;
using System.Linq;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Win.Editors;
using DevExpress.XtraEditors.Repository;
using ExcelDataReader;
using Xpand.ExpressApp.ExcelImporter.BusinessObjects;
using Xpand.ExpressApp.ExcelImporter.Controllers;
using Xpand.Persistent.Base;

namespace Xpand.ExpressApp.ExcelImporter.Win.Controllers{
    public class ExcelImportDetailViewController : ExcelImporter.Controllers.ExcelImportDetailViewController{
        protected override void OnActivated(){
            base.OnActivated();

            ExcelImport.File.PropertyChanged += FileOnPropertyChanged;
            if (File.Exists(ExcelImport.FullName)){
                using (var fileStream = File.OpenRead(ExcelImport.FullName)){
                    ExcelImport.File.LoadFromStream(ExcelImport.FullName, fileStream);
                }

                ExcelImport.File.FileName = Path.GetFileName(ExcelImport.FullName);
            }

            ObjectSpace.Committing += ObjectSpaceOnCommitting;
        }

        protected override void OnViewControlsCreated(){
            base.OnViewControlsCreated();
            if (!ExcelImport.IsNewObject){
                var listPropertyEditor = View.GetItems<ListPropertyEditor>().First(editor =>
                    editor.MemberInfo.Name == nameof(ExcelImport.ExcelColumnMaps));
                ((GridListEditor) listPropertyEditor.ListView.Editor).ControlsCreated+=OnControlsCreated;
            }
        }

        private void OnControlsCreated(object sender, EventArgs eventArgs){
            PopulatePropertyNames();
        }

        protected override void OnDeactivated(){
            base.OnDeactivated();
            ObjectSpace.Committing -= ObjectSpaceOnCommitting;
            ExcelImport.File.PropertyChanged -= FileOnPropertyChanged;
        }

        protected override void TypeChange(){
            base.TypeChange();
            PopulatePropertyNames();
        }

        private void PopulatePropertyNames(){
            var listPropertyEditor = View.GetItems<ListPropertyEditor>().First(editor =>
                editor.MemberInfo.Name == nameof(ExcelImport.ExcelColumnMaps));
            var gridView = ((GridListEditor) listPropertyEditor.ListView.Editor).GridView;
            var repositoryItem = (RepositoryItemComboBox) gridView.Columns[nameof(ExcelColumnMap.PropertyName)].ColumnEdit;
            repositoryItem.Items.Clear();
            repositoryItem.Items.AddRange(ExcelImport.TypePropertyNames);
        }

        private void ObjectSpaceOnCommitting(object sender, CancelEventArgs cancelEventArgs){
            ValidateFile();
        }


        private void FileOnPropertyChanged(object sender, PropertyChangedEventArgs e){
            if (e.PropertyName == nameof(XpandFileData.FullName)){
                if (File.Exists(ExcelImport.File.FullName))
                    ExcelImport.FullName = ExcelImport.File.FullName;
                try {
                    ParseStream();
                }
                catch{
                    ExcelImport.File = new XpandFileData();
                    ExcelImport.File.PropertyChanged -= FileOnPropertyChanged;
                    ExcelImport.File.PropertyChanged += FileOnPropertyChanged;
                    throw;
                }
            }
        }

        private void ParseStream(){
            using (var fileStream = File.Open(ExcelImport.FullName, FileMode.Open, FileAccess.Read,
                FileShare.Read)){
                byte[] bytes;
                using (var memoryStream = new MemoryStream()){
                    fileStream.CopyTo(memoryStream);
                    bytes = memoryStream.ToArray();
                }

                var content = ExcelImport.GetXlsContent(ExcelImport.FullName, bytes);
                using (var excelDataReader = ExcelReaderFactory.CreateReader(content)){
                    using (var dataSet = excelDataReader.GetDataSet(ExcelImport)){
                        ExcelImport.SheetNames = dataSet.Tables.Cast<DataTable>().Select(table => table.TableName).ToList();
                    }
                }
            }
        }
    }
}
