using System;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Threading;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.DC;
using DevExpress.Persistent.Base;
using JetBrains.Annotations;
using Xpand.ExpressApp.ExcelImporter.BusinessObjects;
using Xpand.ExpressApp.ExcelImporter.Services;
using Xpand.ExpressApp.ExcelImporter.Win.BusinessObjects;
using Xpand.Extensions.Reactive.Transform;
using Xpand.XAF.Modules.Reactive.Services;

namespace Xpand.ExpressApp.ExcelImporter.Win.Services {
    public static class AutoImportService {
        static readonly Subject<(ExcelImport, FileDropped)> DroppedSubject=new();


        internal static IObservable<(ExcelImport excelImport, FileDropped fileDropped, FileDropWatcher watcher)>
            TraceDroppedFiles(this IObservable<(ExcelImport excelImport, FileDropped fileDropped, FileDropWatcher watcher)> source,XafApplication application) {
            return source.Do(_ => DroppedSubject.OnNext((_.excelImport, _.fileDropped)))
                .Where(_ => _.excelImport.WhereCanAutoImport(application).Any());
        }

        [SuppressMessage("ReSharper", "SuspiciousTypeConversion.Global")]
        internal static IObservable<Unit> Connect(this XafApplication application) {
            var modified = application.WhenDetailViewCreated()
                .Select(_ => _.e.View).Where(_ => _.ObjectTypeInfo.Type == typeof(ExcelImport))
                .SelectMany(_ => _.ObjectSpace.WhenCommitted().Select(tuple => (ExcelImport) _.CurrentObject)).Publish().RefCount();
            
            
            var existingCanAutoImport = application.WhenSetupComplete()
                .SelectMany(_ => Observable.Using(() => application.CreateObjectSpace(typeof(ExcelImport)),
                    objectSpace => objectSpace.GetObjectsQuery<ExcelImport>().WhereCanAutoImport().ToObservable()));

            var watchers = existingCanAutoImport
                .Merge(modified.SelectMany(_ => _.WhereCanAutoImport(application)))
                .Distinct(_ => _.Oid)
                .Select(_ => {
                    var watcher = new FileDropWatcher(_.AutoImportFrom, _.AutoImportRegex, _.AutoImportSearchType);
                    watcher.Start();
                    return (watcher,excelImport:_);
                })
                .Replay();
            watchers.Connect();
            var startedWatchers=modified.SelectMany(_ => _.WhereCanAutoImport(application))
                .SelectMany(import => watchers.Where(_ => _.excelImport.Oid==import.Oid&&!_.watcher.Monitoring).Do(_ => _.watcher.Start())).Publish().RefCount();
            var changeWatchersMonitoring = startedWatchers
                .Merge(modified.Where(_ => string.IsNullOrEmpty(_.AutoImportFrom)||!Directory.Exists(_.AutoImportFrom))
                    .SelectMany(import => watchers.Where(_ => _.excelImport.Oid==import.Oid&&_.watcher.Monitoring).Do(_ => _.watcher.Stop())))
                .ToUnit();

            var excelImportFileDrops = watchers.SelectMany(_ => _.watcher.Dropped.Select(fileDropped => (_.excelImport,fileDropped,_.watcher))).Publish().RefCount();
                            
            var pollExisting = watchers.Merge(startedWatchers)
                .Do(_ => _.watcher.PollExisting())
                .ToUnit();
            
            return application.WhenWindowCreated().When(TemplateContext.ApplicationWindow)
                    .SelectMany(tuple => {
                        var importConcurrencyLimit = ((IModelOptionsAutoImportConcurrencyLimit) application.Model.Options).ImportConcurrencyLimit;
                        return excelImportFileDrops
                            .TraceDroppedFiles(tuple.Application)
                            .Select(_ => Observable.Start(() => application.Import(_.fileDropped, (_.excelImport.Oid,_.watcher))))
                            .Merge(importConcurrencyLimit).ToUnit()
                            .Merge(DroppedSubject.Do(_ => AddDroppedFiles(_,application)).ToUnit())
                            .Catch<Unit,Exception>(_ => Unit.Default.ReturnObservable().ObserveOn(SynchronizationContext.Current).SelectMany(_ => Observable.Empty<Unit>()))
                            .Merge(pollExisting)
                            .Merge(changeWatchersMonitoring);
                    })
                ;
        }

        private static void AddDroppedFiles((ExcelImport excelImport, FileDropped fileDropped) tuple,XafApplication application) {
            using var objectSpace = application.CreateObjectSpace();
            var excelImport =objectSpace.GetObject( tuple.excelImport);
            var droppedFile = excelImport.ObjectSpace.CreateObject<DroppedFile>();
            droppedFile.FileName = Path.GetFileName(tuple.fileDropped.FullPath);
            droppedFile.DateTime=DateTime.Now;
            excelImport.DroppedFiles.Add(droppedFile);
            excelImport.ObjectSpace.CommitChanges();
        }

        private static void Import(this XafApplication application,FileDropped dropped, (Guid excelImportOid, FileDropWatcher watcher) tuple) {
            using var objectSpace = application.CreateObjectSpace();
            var excelImport = objectSpace.GetObjectByKey<ExcelImport>(tuple.excelImportOid);
            var creationTime = new FileInfo(dropped.FullPath).CreationTime;
            var skipAutoImportReason = SkipAutoImportReason(excelImport, creationTime, dropped);
            if (skipAutoImportReason!=ExcelImporter.BusinessObjects.SkipAutoImportReason.None) {
                excelImport.DroppedFiles.OrderByDescending(_ => _.DateTime).First().SkipReason=skipAutoImportReason;
                objectSpace.CommitChanges();
                return;
            }
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
                notificationMessage = $"Importing of {excelImport.Name} succeed {import} objects updated";
            }

            if (importNotification != null) {
                importNotification.NotificationMessage = notificationMessage;
                importNotification.AlarmTime = DateTime.Now;
            }
            autoImportedFile.EndTime = DateTime.Now;

            application.Execute(tuple, excelImport, _ => objectSpace.CommitChanges(),$"Exception when importing {excelImport.Name} - {{0}}");
        }

        private static ExcelImport[] WhereCanAutoImport(this IQueryable<ExcelImport> excelImports) {
            return excelImports
                .Where(_ => _.AutoImportFrom!=null&&_.AutoImportFrom!="")
                .ToArray()
                .Where(_ => Directory.Exists(Path.GetFullPath(_.AutoImportFrom)))
                .ToArray();
        }

        private static ExcelImport[] WhereCanAutoImport(this ExcelImport excelImport,XafApplication application) {
            using var objectSpace = application.CreateObjectSpace();
            return objectSpace.GetObjectsQuery<ExcelImport>().Where(_ => _.Oid == excelImport.Oid).WhereCanAutoImport();
        }

        private static bool Execute(this XafApplication application, (Guid excelImportOid, FileDropWatcher watcher) tuple,
            ExcelImport excelImport, [InstantHandle] Action<IObjectSpace> action, string message) {
            try {
                using var failedResultsObjectSpace = application.CreateObjectSpace();
                action(failedResultsObjectSpace);
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

        private static SkipAutoImportReason SkipAutoImportReason(this ExcelImport excelImport, DateTime creationTime, FileDropped fileDropped) {
            var isAlreadyImported = excelImport.AutoImportedFiles.Any(file =>
                file.Succeded == true && file.FileName.ToLower() == fileDropped.Name.ToLower() && file.CreationTime.ToString(CultureInfo.InvariantCulture) ==
                creationTime.ToString(CultureInfo.InvariantCulture));
            if (isAlreadyImported)
                return ExcelImporter.BusinessObjects.SkipAutoImportReason.AlreadImported;
            if (excelImport.StopAutoImportOnFailure) {
                var lastImportedFile = excelImport.AutoImportedFiles.OrderByDescending(_ => _.EndTime).FirstOrDefault(file => file.FileName==fileDropped.Name);
                if (lastImportedFile?.Succeded != null && !lastImportedFile.Succeded.Value)
                    return ExcelImporter.BusinessObjects.SkipAutoImportReason.StopAutoImportOnFailure;
            }
            return ExcelImporter.BusinessObjects.SkipAutoImportReason.None;
        }

    }

    public interface IModelOptionsAutoImportConcurrencyLimit {
        [Category("eXpand.ExcelImporter")]
        int ImportConcurrencyLimit { get; set; }
    }

    [DomainLogic(typeof(IModelOptionsAutoImportConcurrencyLimit))]
    public class ModelOptionsAutoImportConcurrencyLimitLogic{
        public static int Get_ImportConcurrencyLimit(IModelOptionsAutoImportConcurrencyLimit limit) {
            return Environment.ProcessorCount;
        }
    }
}