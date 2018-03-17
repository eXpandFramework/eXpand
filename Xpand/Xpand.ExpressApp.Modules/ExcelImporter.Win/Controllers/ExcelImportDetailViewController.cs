using System.ComponentModel;
using System.Data;
using System.IO;
using System.Linq;
using ExcelDataReader;
using Xpand.ExpressApp.ExcelImporter.BusinessObjects;
using Xpand.Persistent.Base;

namespace Xpand.ExpressApp.ExcelImporter.Win.Controllers{
    public class ExcelImportDetailViewController : ExcelImporter.Controllers.ExcelImportDetailViewController{
        protected override void OnActivated(){
            base.OnActivated();
            
            ExcelImport.File.PropertyChanged+=FileOnPropertyChanged;
            if (File.Exists(ExcelImport.FullName)){
                using (var fileStream = File.OpenRead(ExcelImport.FullName)){
                    ExcelImport.File.LoadFromStream(ExcelImport.FullName,fileStream);
                }

                ExcelImport.File.FileName = Path.GetFileName(ExcelImport.FullName);
            }
            ObjectSpace.Committing+=ObjectSpaceOnCommitting;
        }


        protected override void OnDeactivated(){
            base.OnDeactivated();
            ObjectSpace.Committing-=ObjectSpaceOnCommitting;
            ExcelImport.File.PropertyChanged-=FileOnPropertyChanged;
        }

        private void ObjectSpaceOnCommitting(object sender, CancelEventArgs cancelEventArgs){
            ValidateFile();
        }


        private void FileOnPropertyChanged(object sender, PropertyChangedEventArgs e){
            if (e.PropertyName==nameof(XpandFileData.FullName)){
                if (File.Exists(ExcelImport.File.FullName)){
                    ExcelImport.FullName = ExcelImport.File.FullName;
                    using (var fileStream = File.OpenRead(ExcelImport.FullName)){
                        ParseFile(fileStream, ExcelImport);
                    }
                }
            }
        }

        private static void ParseFile(Stream stream, ExcelImport excelImport){
            using (var excelDataReader = ExcelReaderFactory.CreateReader(stream)){
                using (var dataSet = GetDataSet(excelDataReader, excelImport)){
                    excelImport.SheetNames = dataSet.Tables.Cast<DataTable>().Select(table => table.TableName).ToList();
                }
            }
        }
    }
}