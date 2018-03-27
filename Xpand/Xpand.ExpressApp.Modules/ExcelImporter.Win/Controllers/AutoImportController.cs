using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using DevExpress.ExpressApp;
using DevExpress.Persistent.Base;
using JetBrains.Annotations;
using Xpand.ExpressApp.ExcelImporter.BusinessObjects;
using Xpand.ExpressApp.ExcelImporter.Controllers;
using Xpand.ExpressApp.ExcelImporter.Services;
using Xpand.ExpressApp.ExcelImporter.Win.BusinessObjects;

namespace Xpand.ExpressApp.ExcelImporter.Win.Controllers{
    public class AutoImportController:WindowController{
        readonly Dictionary<Guid,FileDropWatcher> _fileDropWatchers=new Dictionary<Guid, FileDropWatcher>();

        public AutoImportController(){
            TargetWindowType=WindowType.Main;
        }

        protected override void OnActivated(){
            base.OnActivated();
            var objectSpaceCreated = Observable.FromEventPattern<DetailViewCreatedEventArgs>(h => Application.DetailViewCreated += h,h => Application.DetailViewCreated -= h)
                .Where(pattern => pattern.EventArgs.View.ObjectTypeInfo.Type == typeof(ExcelImport))
                .Select(pattern => pattern.EventArgs.View.ObjectSpace);
            
            var modifiedExcelImports=objectSpaceCreated
                .SelectMany(space => Observable.FromEventPattern<CancelEventArgs>(h => space.Committing += h, h => space.Committing -= h)
                .SelectMany(pattern => ((IObjectSpace) pattern.Sender).ModifiedObjects.OfType<ExcelImport>()))
                .Where(import => {
                    if (CanAutoImport(import)){
                        if (!_fileDropWatchers.ContainsKey(import.Oid)){
                            _fileDropWatchers.Add(import.Oid,null);
                            return true;
                        }
                    }
                    else{
                        if (_fileDropWatchers.ContainsKey(import.Oid))
                            StopWatching(import);
                    }

                    return false;
                }).Delay(TimeSpan.FromSeconds(5));

            objectSpaceCreated
                .SelectMany(space => Observable.FromEventPattern<ObjectsManipulatingEventArgs>(h=>space.ObjectDeleted+=h,h=>space.ObjectDeleted-=h)
                .SelectMany(pattern => pattern.EventArgs.Objects.OfType<ExcelImport>()))
                .Subscribe(import => {
                    if (_fileDropWatchers.ContainsKey(import.Oid)){
                        StopWatching(import);
                    }
                });

            var objectSpace = Application.CreateObjectSpace();
            var existingExcelImports = objectSpace.GetObjectsQuery<ExcelImport>()
                .ToArray()
                .Where(CanAutoImport)
                .ToObservable()
                .Do(import => _fileDropWatchers.Add(import.Oid,null));

            existingExcelImports.Merge(modifiedExcelImports)
                .Subscribe(import => {
                    var fileDropWatcher = new FileDropWatcher(GetFullPath(import), import.AutoImportRegex,import.AutoImportSearchType);
                    _fileDropWatchers[import.Oid] = fileDropWatcher;
                    fileDropWatcher.Start();
                    fileDropWatcher.Dropped.ObserveOn(Scheduler.Default).Subscribe(dropped => Import(dropped, (import.Oid, fileDropWatcher)));
                    fileDropWatcher.PollExisting();
                });            
        }

        private void StopWatching(ExcelImport import){
            var fileDropWatcher = _fileDropWatchers[import.Oid];
            fileDropWatcher.Stop();
            fileDropWatcher.Dispose();
            _fileDropWatchers.Remove(import.Oid);
        }

        private bool CanAutoImport(ExcelImport import){
            return !string.IsNullOrWhiteSpace(import.AutoImportFrom) &&
                   (Directory.Exists(GetFullPath(import)) && !AutoImportIsStopped(import));
        }

        private  string GetFullPath(ExcelImport import){
            return Path.GetFullPath(import.AutoImportFrom);
        }


        private void Import(FileDropped dropped, (Guid excelImportOid, FileDropWatcher watcher) tuple){
            using (var objectSpace = Application.CreateObjectSpace()){
                var excelImport = objectSpace.GetObjectByKey<ExcelImport>(tuple.excelImportOid);
                var creationTime = new FileInfo(dropped.FullPath).CreationTime;
                if (IsAlreadyImported(excelImport,creationTime,dropped.Name))
                    return;
                var autoImportedFile = objectSpace.CreateObject<AutoImportedFile>();
                autoImportedFile.FileName = dropped.Name;
                autoImportedFile.StartTime=DateTime.Now;
                autoImportedFile.CreationTime=creationTime;
                int import = 0;
                if (!Execute(tuple, excelImport,() => import=excelImport.Import(File.ReadAllBytes(dropped.FullPath)),$"Validation Exceptions when importing {excelImport.Name} - Please use the UI to run the import")) 
                    return;
                excelImport.AutoImportedFiles.Add(autoImportedFile);
                ImportNotification importNotification = null;
                string notificationMessage;
                if (excelImport.AutoImportNotification == AutoImportNotification.Always)
                    importNotification = objectSpace.CreateObject<ImportNotification>();
                if (excelImport.FailedResultList.FailedResults.Any()){
                    autoImportedFile.Succeded = false;
                    if (excelImport.StopAutoImportOnFailure)
                        tuple.watcher.Stop();
                    if (excelImport.AutoImportNotification == AutoImportNotification.Failures)
                        importNotification = objectSpace.CreateObject<ImportNotification>();
                    notificationMessage =
                        $"Importing of {excelImport.Name} failed please check the {nameof(ExcelImport.FailedResultList)} in the UI and run the import again. To re-enable the auto- import delete the failed entry on the {nameof(ExcelImport.AutoImportedFiles)} list.";
                }
                else{
                    autoImportedFile.Succeded = true;
                    notificationMessage = $"Importing of {excelImport.Name} succeded {import} objects updated";
                }

                if (importNotification != null){
                    importNotification.NotificationMessage = notificationMessage;
                    importNotification.AlarmTime=DateTime.Now;
                }

                if (!Execute(tuple, excelImport,() => objectSpace.CommitChanges(),$"Exception when importing {excelImport.Name} - {{0}}")) 
                    return;
                autoImportedFile.EndTime = DateTime.Now;
                objectSpace.CommitChanges();
            }
        }

        private bool Execute((Guid excelImportOid, FileDropWatcher watcher) tuple,
            ExcelImport excelImport, [InstantHandle]Action action, string message){
            try{
                action();
            }
            catch (Exception e){
                if (excelImport.StopAutoImportOnFailure){
                    tuple.watcher.Stop();
                }
                using (var space = Application.CreateObjectSpace()){
                    var importNotification = space.CreateObject<ImportNotification>();
                    importNotification.AlarmTime = DateTime.Now;
                    importNotification.NotificationMessage = string.Format(message,e.Message);
                    space.CommitChanges();
                }

                Tracing.Tracer.LogError(e);
                return false;

            }

            return true;
        }

        private bool IsAlreadyImported(ExcelImport excelImport, DateTime creationTime, string fileName){
            return excelImport.AutoImportedFiles.Any(file => file.FileName.ToLower()==fileName.ToLower()&&file.CreationTime.ToString() ==creationTime.ToString());
        }

        private bool AutoImportIsStopped(ExcelImport import){
            return import.AutoImportedFiles.Any(file => !file.Succeded) && import.StopAutoImportOnFailure;
        }
    }
}