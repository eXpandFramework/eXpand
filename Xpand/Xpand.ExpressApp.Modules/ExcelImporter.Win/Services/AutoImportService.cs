using System;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Windows.Forms;
using DevExpress.ExpressApp;
using DevExpress.Persistent.Base;
using JetBrains.Annotations;
using Xpand.ExpressApp.ExcelImporter.BusinessObjects;
using Xpand.ExpressApp.ExcelImporter.Services;
using Xpand.ExpressApp.ExcelImporter.Win.BusinessObjects;
using Xpand.XAF.Modules.Reactive.Extensions;
using Xpand.XAF.Modules.Reactive.Services;

namespace Xpand.ExpressApp.ExcelImporter.Win.Services {
    public static class AutoImportService {
        internal static IObservable<Unit> Connect(this XafApplication application) {
            var modified = application.WhenDetailViewCreated()
                .Select(_ => _.e.View).Where(_ => _.ObjectTypeInfo.Type == typeof(ExcelImport))
                .SelectMany(_ => _.ObjectSpace.WhenCommited().Select(tuple => (ExcelImport) _.CurrentObject));

            var existingCanAutoImport = application.WhenSetupComplete()
                .SelectMany(_ => Observable.Using(() => application.CreateObjectSpace(typeof(ExcelImport)),objectSpace => objectSpace.GetObjectsQuery<ExcelImport>().WhereCanAutoImport().ToObservable()));

            var modifiedCanAutoImport = modified.SelectMany(_ => _.WhereCanAutoImport(application))
                .Select(import => import);

            var distinctCanAutoImport = modifiedCanAutoImport.Merge(existingCanAutoImport).Distinct(_ => _.Oid);

            var excelImportFileDrops = distinctCanAutoImport
                .SelectMany(_ => Observable.Using(
                    () => {
                        var fileDropWatcher = new FileDropWatcher(_.AutoImportFrom, _.AutoImportRegex, _.AutoImportSearchType);
                        fileDropWatcher.Start();
                        return fileDropWatcher;
                    },
                    watcher => {
                        var importDropped = watcher.Dropped.Where(dropped => _.WhereCanAutoImport(application).Any())
                            .Select(fileDropped => (excelImport:_,fileDropped,watcher));
                        return Observable.Start(watcher.PollExisting).CombineLatest(importDropped,(unit, tuple) => tuple);
                    }))
                .Replay().RefCount();

            var pollExisting = modifiedCanAutoImport
                .WithLatestFrom(excelImportFileDrops.Select(_ => _.watcher).Distinct(), (import,watcher) => watcher)
                .Do(_ => _.PollExisting())
                .ToUnit();

            return excelImportFileDrops
                .Do(_ => application.Import(_.fileDropped, (_.excelImport.Oid,_.watcher)))
                .ToUnit()
                .Catch<Unit,Exception>(exception => Unit.Default.AsObservable().ObserveOn(((Control) application.MainWindow.Template)).SelectMany(_ => Observable.Empty<Unit>()))
                .Merge(pollExisting);
        }

        private static void Import(this XafApplication application,FileDropped dropped, (Guid excelImportOid, FileDropWatcher watcher) tuple) {
            using (var objectSpace = application.CreateObjectSpace()){
                var excelImport = objectSpace.GetObjectByKey<ExcelImport>(tuple.excelImportOid);
                var creationTime = new FileInfo(dropped.FullPath).CreationTime;
                if (IsAlreadyImported(excelImport, creationTime, dropped.Name))
                    return;
                var autoImportedFile = objectSpace.CreateObject<AutoImportedFile>();
                autoImportedFile.FileName = dropped.Name;
                autoImportedFile.StartTime = DateTime.Now;
                autoImportedFile.CreationTime = creationTime;
                autoImportedFile.ExcelImport=excelImport;
                objectSpace.CommitChanges();
                var import = 0;
                excelImport.AutoImportedFiles.Add(autoImportedFile);
                if (!application.Execute(tuple, excelImport,
                    failedResultsObjectSpace => import = excelImport.Import(failedResultsObjectSpace, File.ReadAllBytes(dropped.FullPath)),
                    $"{excelImport.Name} import failed - Please use the UI to run the import")) 
                    return;
                
                ImportNotification importNotification = null;
                string notificationMessage;
                if (excelImport.AutoImportNotification == AutoImportNotification.Always)
                    importNotification = objectSpace.CreateObject<ImportNotification>();
                if (excelImport.FailedResults.Any()) {
                    autoImportedFile.Succeded = false;
                    if (excelImport.StopAutoImportOnFailure)
                        tuple.watcher.Stop();
                    if (excelImport.AutoImportNotification == AutoImportNotification.Failures)
                        importNotification = objectSpace.CreateObject<ImportNotification>();
                    notificationMessage = $"Importing of {excelImport.Name} failed please check the {nameof(ExcelImport.FailedResults)} in the UI and run the import again.";
                }
                else {
                    autoImportedFile.Succeded = true;
                    notificationMessage = $"Importing of {excelImport.Name} succeded {import} objects updated";
                }

                if (importNotification != null) {
                    importNotification.NotificationMessage = notificationMessage;
                    importNotification.AlarmTime = DateTime.Now;
                }

                if (!application.Execute(tuple, excelImport, space => objectSpace.CommitChanges(),
                    $"Exception when importing {excelImport.Name} - {{0}}"))
                    return;
                autoImportedFile.EndTime = DateTime.Now;
                objectSpace.CommitChanges();
            }
        }

        private static ExcelImport[] WhereCanAutoImport(this IQueryable<ExcelImport> excelImports) {
            return excelImports
                .Where(_ => _.AutoImportFrom!=null&&_.AutoImportFrom!=""&&!(_.AutoImportedFiles.Any(file => !file.Succeded) && _.StopAutoImportOnFailure))
                .ToArray().Where(_ => Directory.Exists(Path.GetFullPath(_.AutoImportFrom))).ToArray();
        }

        private static ExcelImport[] WhereCanAutoImport(this ExcelImport excelImport,XafApplication application) {
            using (var objectSpace = application.CreateObjectSpace()){
                return objectSpace.GetObjectsQuery<ExcelImport>().Where(_ => _.Oid == excelImport.Oid).WhereCanAutoImport();
            }
        }

        private static bool Execute(this XafApplication application, (Guid excelImportOid, FileDropWatcher watcher) tuple,
            ExcelImport excelImport, [InstantHandle] Action<IObjectSpace> action, string message) {
            try {
                using (var failedResultsObjectSpace = application.CreateObjectSpace()){
                    action(failedResultsObjectSpace);
                }
            }
            catch (Exception e) {
                if (excelImport.StopAutoImportOnFailure) tuple.watcher.Stop();
                using (var space = application.CreateObjectSpace()) {
                    var importNotification = space.CreateObject<ImportNotification>();
                    importNotification.AlarmTime = DateTime.Now;
                    importNotification.NotificationMessage = string.Format(message, e.Message);
                    space.CommitChanges();
                }

                Tracing.Tracer.LogError(e);
                return false;
            }

            return true;
        }

        private static bool IsAlreadyImported(this ExcelImport excelImport, DateTime creationTime, string fileName) {
            return excelImport.AutoImportedFiles.Any(file =>
                file.FileName.ToLower() == fileName.ToLower() &&
                file.CreationTime.ToString(CultureInfo.InvariantCulture) ==
                creationTime.ToString(CultureInfo.InvariantCulture));
        }

    }

}