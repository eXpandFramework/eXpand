using System;
using System.ComponentModel;
using System.Data;
using System.IO;
using System.Linq;
using System.Reactive.Linq;
using System.Threading;
using System.Threading.Tasks;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.SystemModule;
using ExcelDataReader;
using Xpand.ExpressApp.ExcelImporter.Services;
using Xpand.Persistent.Base;

namespace Xpand.ExpressApp.ExcelImporter.Win.Controllers{
    public class ExcelImportDetailViewController : ExcelImporter.Controllers.ExcelImportDetailViewController{
        private SynchronizationContext _synchronizationContext;

        protected override void OnActivated(){
            base.OnActivated();

            ExcelImport.File.PropertyChanged += FileOnPropertyChanged;
            if (File.Exists(ExcelImport.FullName)){
                using (var fileStream = File.OpenRead(ExcelImport.FullName)){
                    ExcelImport.File.LoadFromStream(ExcelImport.FullName, fileStream);
                }

                ExcelImport.File.FileName = Path.GetFileName(ExcelImport.FullName);
            }
        }

        protected override void ShowMapConfigView(ShowViewParameters parameters) {
            base.ShowMapConfigView(parameters);
            parameters.CreatedView.Disposing += (sender, args) => {
                Observable.FromAsync(async () => await Task.Delay(100))
                    .ObserveOn(SynchronizationContext.Current)
                    .Do(unit => Frame.GetController<RefreshController>().RefreshAction.DoExecute())
                    .Subscribe();
            };
        }

        protected override void OnViewControlsCreated(){
            base.OnViewControlsCreated();
            _synchronizationContext = SynchronizationContext.Current;
        }
    

        protected override IObservable<T> Synchronise<T>(T i) {
            return base.Synchronise(i).ObserveOn(_synchronizationContext);
        }


        protected override void OnDeactivated(){
            base.OnDeactivated();
            ExcelImport.File.PropertyChanged -= FileOnPropertyChanged;
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
