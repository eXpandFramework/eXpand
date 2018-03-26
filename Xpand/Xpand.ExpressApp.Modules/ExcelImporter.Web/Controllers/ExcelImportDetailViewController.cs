using System;
using System.Data;
using System.IO;
using System.Linq;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.FileAttachments.Web;
using DevExpress.ExpressApp.Model;
using ExcelDataReader;
using Xpand.ExpressApp.ExcelImporter.BusinessObjects;

namespace Xpand.ExpressApp.ExcelImporter.Web.Controllers{
    public class ExcelImportDetailViewController:ExcelImporter.Controllers.ExcelImportDetailViewController{
        private IModelMember _propertyNameModelMember;
        protected override void OnActivated(){
            base.OnActivated();
            if (View is DetailView detailView){
                detailView.GetItems<FileDataPropertyEditor>().First().ControlCreated += OnControlCreated;
                _propertyNameModelMember = Application.Model.BOModel.GetClass(typeof(ExcelColumnMap)).FindMember(nameof(ExcelColumnMap.PropertyName));
            }
        }

        protected override void TypeChange(){
            base.TypeChange();
            _propertyNameModelMember.PredefinedValues = string.Join(";", ExcelImport.TypePropertyNames);
        }

        private void OnControlCreated(object sender, EventArgs eventArgs){
            ((FileDataPropertyEditor) sender).Editor.FileDataLoading+=OnFileDataLoading;
        }

        private  void ParseStream(Stream stream){
            using (var excelDataReader = ExcelReaderFactory.CreateReader(stream)){
                using (var dataSet = GetDataSet(excelDataReader, ExcelImport)){
                    ExcelImport.SheetNames = dataSet.Tables.Cast<DataTable>().Select(table => table.TableName).ToList();
                }
            }
        }

        private void OnFileDataLoading(object sender, FileDataLoadingEventArgs e){
            ParseStream(e.UploadedFile.FileContent);
        }
    }
}