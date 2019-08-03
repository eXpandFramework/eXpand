using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.Localization;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.SystemModule;
using DevExpress.ExpressApp.Utils;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.Validation;
using Xpand.ExpressApp.ExcelImporter.BusinessObjects;
using Xpand.ExpressApp.ExcelImporter.Services;
using Xpand.Persistent.Base.Validation;
using Xpand.XAF.Modules.ProgressBarViewItem;
using Xpand.XAF.Modules.Reactive.Services;

namespace Xpand.ExpressApp.ExcelImporter.Controllers{
    public class ExcelImportDetailViewController : ObjectViewController<DetailView,ExcelImport>{

        private const string ExcelMapActionName = "ExcelMap";
        private const string ImportExcelActionName = "ImportExcel";
        
        public SingleChoiceAction MapAction{ get; }

        public ExcelImportDetailViewController(){
            MapAction = new SingleChoiceAction(this, ExcelMapActionName, "ExcelImport") {
                 ItemType = SingleChoiceActionItemType.ItemIsOperation
            };
            MapAction.Items.Add(new ChoiceActionItem("Map", "Map"));
            MapAction.Items.Add(new ChoiceActionItem("Reset", "Reset"));
            MapAction.Execute+=ExcelMappingActionOnExecute;
            ImportAction = new SimpleAction(this,ImportExcelActionName,"ExcelImport"){Caption = "Import"};
            ImportAction.Execute+=ImportActionOnExecute;
            ImportAction.Executing+=ImportActionOnExecuting;
        }


        private void ExcelMappingActionOnExecute(object sender, SingleChoiceActionExecuteEventArgs e) {
            if ((string) e.SelectedChoiceActionItem.Data == "Reset") {
                ObjectSpace.Delete(ExcelImport.ExcelColumnMaps);
                ObjectSpace.CommitChanges();
                MapAction.DoExecute(MapAction.FindItemByIdPath("Map"));
            }
            else {
                if (!ExcelImport.ExcelColumnMaps.Any())
                    Map();
                ObjectSpace.CommitChanges();
                var parameters = e.ShowViewParameters;
                


                var dialogController = new DialogController();
                parameters.Controllers.Add(dialogController);
                parameters.CreatedView=Application.CreateDashboardView(Application.CreateObjectSpace(), "ExcelColumnMapMasterDetail", true);
                parameters.TargetWindow=TargetWindow.NewModalWindow;
                ShowMapConfigView(parameters);
                dialogController.AcceptAction.Executed+=AcceptActionOnExecuted;
            }
            
        }

        private void AcceptActionOnExecuted(object sender, ActionBaseEventArgs e) {
            ((IObjectSpaceLink) ExcelImport).ObjectSpace.ReloadObject(ExcelImport);
            ExcelImport.ValidateForImport();
        }

        protected virtual void ShowMapConfigView(ShowViewParameters parameters) {
            
        }

        private void Map() {
            ValidateFile();
            var excelImport = ExcelImport;
            excelImport.Map();   
        }

        private void ImportActionOnExecuting(object sender, CancelEventArgs cancelEventArgs){
            ValidateFile();
            ExcelImport.ValidateForImport();
        }

        protected ExcelImport ExcelImport => ((ExcelImport) View.CurrentObject);
        protected virtual void ValidateFile(){
            if (ExcelImport.File.Content == null){
                var result = Validator.RuleSet.NewRuleSetValidationMessageResult(ObjectSpace, "Invalid file", "Save",
                    View.CurrentObject, View.ObjectTypeInfo.Type, new List<string>{nameof(ExcelImport.File)});
                throw new ValidationException("", result);
            }
            
        }

        private void ImportActionOnExecute(object sender, SimpleActionExecuteEventArgs e){
            var progressBarViewItem = View.GetItems<ProgressBarViewItemBase>().First();
            progressBarViewItem.Start();
            var progressObserver = GetProgressObserver(ExcelImport,progressBarViewItem);
            ObjectSpace.SetModified(View.CurrentObject);
            var importParameters = ExcelImport.GetImportParameters();

            
            Observable.Start(async () => {
                var importObjectSpace = Application.CreateObjectSpace(ExcelImport.GetType());
                var failedResultsObjectSpace = Application.CreateObjectSpace(ExcelImport.GetType());
                var excelImport = importObjectSpace.GetObjectByKey<ExcelImport>(ExcelImport.Oid);
                excelImport.Import(failedResultsObjectSpace,ExcelImport.File.Content, progressObserver,importParameters);
                await ObjectSpace.WhenCommiting().FirstAsync()
                    .SelectMany(Synchronize)
                    .Do(i => {
                        if (!string.IsNullOrWhiteSpace(excelImport.ValidationContexts)) {
                            Validator.RuleSet.ValidateAll(importObjectSpace, importObjectSpace.ModifiedObjects,excelImport.ValidationContexts);
                        }
                    }).DefaultIfEmpty();
                return (importObjectSpace,failedResultsObjectSpace);
            })
            .SelectMany(task => task)
            .Do(_ => {
                _.failedResultsObjectSpace.Dispose();
                _.importObjectSpace.CommitChanges();
                _.importObjectSpace.Dispose();
            })
            .Catch<(IObjectSpace,IObjectSpace),Exception>(exception => Synchronize(exception).SelectMany(i => Observable.Throw<(IObjectSpace, IObjectSpace)>(exception)))
            .Subscribe();
        }

        protected virtual IObserver<ImportProgress> GetProgressObserver(ExcelImport excelImport,ProgressBarViewItemBase progressBarViewItem) {
            var progress = CreateProgressObserver();
            BeginImport.OnNext((excelImport, progress));
            var resultMessage = (CaptionHelper.GetLocalizedText(ExcelImporterLocalizationUpdater.ExcelImport,
                    ExcelImporterLocalizationUpdater.ImportSucceded),CaptionHelper.GetLocalizedText(ExcelImporterLocalizationUpdater.ExcelImport,
                ExcelImporterLocalizationUpdater.ImportFailed));
            
            var dataRowProgress = progress.OfType<ImportDataRowProgress>().Where(excelImport);
            
            var progressEnd = ProgressEnd(excelImport, progressBarViewItem, progress, resultMessage,Application.Model.BOModel.ToDictionary(c => c.TypeInfo.FullName,c => c.Caption))
                .Publish().RefCount();
            progress.OfType<ImportProgressStart>().Where(excelImport)
                .Do(OnStart).TakeUntil(progressEnd).Subscribe();
            
            Observable
                .Interval(TimeSpan.FromMilliseconds(progressBarViewItem.PollingInterval))
                .WithLatestFrom(dataRowProgress, (l, importProgress) => ( (decimal)importProgress.Percentage))
                .Select(Synchronize).Concat()
                .TakeUntil(progressEnd)
                .Do(progressBarViewItem)
                .Subscribe();
            return progress;
        }

        protected virtual void OnStart(ImportProgressStart importProgressStart) {
            
        }

        public Subject<(ExcelImport excelImport, ISubject<ImportProgress> progress)> BeginImport { get; } = new Subject<(ExcelImport excelImport, ISubject<ImportProgress> progress)>();

        protected virtual ISubject<ImportProgress> CreateProgressObserver(){
            return Subject.Synchronize(new Subject<ImportProgress>());
        }

        protected  virtual IObservable<T> Synchronize<T>(T t=default) {
            return Observable.Return(t);
        }

        protected virtual IObservable<Unit> ProgressEnd(ExcelImport excelImport,
            ProgressBarViewItemBase progressBarViewItem,
            ISubject<ImportProgress> progress, (string successMsg, string failedMsg) resultMessage,
            Dictionary<string, string> boCaptions) {
            var boModel = Application.Model.BOModel;
            return progress.OfType<ImportProgressComplete>().Where(excelImport).FirstAsync()
                .Select(Synchronize).Concat()
                .Do(_ => {
                    ExcelImport.FailedResults.Reload();
                    progressBarViewItem.SetFinishOptions(GetFinishOptions(_, resultMessage, boModel));
                })
                .ToUnit();
        }

        protected virtual void OnSetPosition(ProgressBarViewItemBase progressBarViewItem, int percentage){
            progressBarViewItem.SetPosition ( percentage);
        }

        protected virtual MessageOptions GetFinishOptions(ImportProgressComplete progressComplete,
            (string successMsg, string failedMsg) resultMessage, IModelBOModel boModel ){
            var failedResults = progressComplete.FailedResults;
            string message;
            var informationType=InformationType.Success;
            if (failedResults.Any()){
                informationType = InformationType.Error;
                message = string.Format(resultMessage.failedMsg, failedResults.GroupBy(r => r.Index).Count(), progressComplete.TotalRecordsCount);
            }
            else{
                message =string.Format(resultMessage.successMsg, progressComplete.TotalRecordsCount);
            }
            var messageOptions = NewMessageOptions( );
            messageOptions.Type=informationType;
            messageOptions.Message=message;
            if (progressComplete is ImportProgressException progressException) {
                messageOptions.Type=InformationType.Error;
                messageOptions.Message = progressException.Exception.Message;
                if (progressException.Exception is MemberNotFoundException memberNotFoundException) {
                    var exceptionMessage = SystemExceptionLocalizer.GetExceptionMessage(ExceptionId.CannotFindThePropertyWithinTheClass, memberNotFoundException.MemberName, boModel[memberNotFoundException.TypeName]);
                    messageOptions.Message=exceptionMessage;
                }
                if (progressException.Exception is KeyMemberNotMappedException keyMemberNotMappedException) {
                    var modelClass = boModel.GetClass(keyMemberNotMappedException.Type);
                    messageOptions.Message=$"Default member for {modelClass.Caption} is missing";
                }
            }
            return messageOptions;
        }

        protected virtual MessageOptions NewMessageOptions(){
            var messageOptions = new MessageOptions{
                Duration = Int32.MaxValue,
                Win = {Type = WinMessageType.Alert},
                Web = {Position = InformationPosition.Top, CanCloseOnOutsideClick = false, CanCloseOnClick = true}
            };
            return messageOptions;
        }

        public SimpleAction ImportAction{ get;  }
    }
}