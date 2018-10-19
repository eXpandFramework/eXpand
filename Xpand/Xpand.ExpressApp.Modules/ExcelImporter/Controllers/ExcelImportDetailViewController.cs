using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.SystemModule;
using DevExpress.ExpressApp.Utils;
using DevExpress.Persistent.Validation;
using Xpand.ExpressApp.Editors;
using Xpand.ExpressApp.ExcelImporter.BusinessObjects;
using Xpand.ExpressApp.ExcelImporter.Services;
using Xpand.Persistent.Base.General;
using Xpand.Persistent.Base.Validation;

namespace Xpand.ExpressApp.ExcelImporter.Controllers{
    public class ExcelImportDetailViewController : ObjectViewController<DetailView,ExcelImport>{
        protected Subject<Unit> Terminator=new Subject<Unit>();

        public const string ExcelMapActionName = "ExcelMap";
        public const string ImportExcelActionName = "ImportExcel";
        
        public SingleChoiceAction MapAction{ get; }

        public ExcelImportDetailViewController(){
            MapAction = new SingleChoiceAction(this, ExcelMapActionName, "ExcelImport") {
                Caption = "Map", ItemType = SingleChoiceActionItemType.ItemIsOperation
            };
            MapAction.Items.Add(new ChoiceActionItem("Map", "Map"));
            MapAction.Items.Add(new ChoiceActionItem("Configure", "Configure"));
            MapAction.Execute+=ExcelMappingActionOnExecute;
            ImportAction = new SimpleAction(this,ImportExcelActionName,"ExcelImport"){Caption = "Import"};
            ImportAction.Execute+=ImportActionOnExecute;
            ImportAction.Executing+=ImportActionOnExecuting;
        }

        private void ExcelMappingActionOnExecute(object sender, SingleChoiceActionExecuteEventArgs e) {
            if ((string) e.SelectedChoiceActionItem.Data=="Configure") {
                if (!ExcelImport.ExcelColumnMaps.Any())
                    Map();
                ObjectSpace.CommitChanges();
                var parameters = e.ShowViewParameters;
                var dialogController = new DialogController();
                parameters.Controllers.Add(dialogController);
                parameters.CreatedView=Application.CreateDashboardView(Application.CreateObjectSpace(), "ExcelColumnMapMasterDetail", true);
                parameters.TargetWindow=TargetWindow.NewModalWindow;
                ShowMapConfigView(parameters);
            }
            else {
                Map();
            }

        }

        protected virtual void ShowMapConfigView(ShowViewParameters parameters) {
            
        }

        protected virtual DashboardView CreateView() {
            var dashboardView = Application.CreateDashboardView(Application.CreateObjectSpace(), "ExcelColumnMapMasterDetail", true);
            dashboardView.Disposing+= (o, args) => {
                Frame.GetController<RefreshController>().RefreshAction.DoExecute();
//                Observable.Start(async () => await Task.Delay(3000))
//                    .Do(unit => Frame.GetController<RefreshController>().RefreshAction.DoExecute())
//                    .Subscribe();

            };
            return dashboardView;
        }


        private void Map() {
            ValidateFile();
            
            ObjectSpace.Delete(ExcelImport.ExcelColumnMaps);

            var excelImport = ExcelImport;
            excelImport.Map();
            var listPropertyEditor = View.GetItems<ListPropertyEditor>().First(editor =>
                editor.MemberInfo.Name == nameof(BusinessObjects.ExcelImport.ExcelColumnMaps));
            foreach (var columnMap in excelImport.ExcelColumnMaps){
                listPropertyEditor.ListView.CollectionSource.Add(columnMap);    
            }
            View.FindItem(nameof(BusinessObjects.ExcelImport.ExcelColumnMaps)).Refresh();

            
        }

        protected override void OnActivated(){
            base.OnActivated();
            ObjectSpace.ObjectChanged+=ObjectSpaceOnObjectChanged;
        }

        protected override void OnDeactivated(){
            base.OnDeactivated();
            Terminator.OnNext(Unit.Default);
            ObjectSpace.ObjectChanged-=ObjectSpaceOnObjectChanged;
        }

        private void ObjectSpaceOnObjectChanged(object sender, ObjectChangedEventArgs e){
            if (Equals(e.Object, ExcelImport) && (e.PropertyName == nameof(BusinessObjects.ExcelImport.FullName) ||
                                                  e.PropertyName == nameof(BusinessObjects.ExcelImport.SheetName) ||
                                                  e.PropertyName == nameof(BusinessObjects.ExcelImport.UseHeaderRows)||
                                                  e.PropertyName == nameof(BusinessObjects.ExcelImport.Type))){

                ObjectSpace.Delete(ExcelImport.ExcelColumnMaps);
                TypeChange();
            }
        }

        protected virtual void TypeChange(){
        }

        private void ImportActionOnExecuting(object sender, CancelEventArgs cancelEventArgs){
            ValidateFile();
        }

        protected ExcelImport ExcelImport => ((ExcelImport) View.CurrentObject);
        protected void ValidateFile(){
            if (ExcelImport.File.Content == null){
                var result = Validator.RuleSet.NewRuleSetValidationMessageResult(ObjectSpace, "Invalid file", "Save",
                    View.CurrentObject, View.ObjectTypeInfo.Type, new List<string>{nameof(ExcelImport.File)});
                throw new ValidationException("", result);
            }
        }

        private void ImportActionOnExecute(object sender, SimpleActionExecuteEventArgs e){
            var importToTypeInfo = ExcelImport.Type.GetTypeInfo();
            if (ExcelImport.ImportStrategy != ImportStrategy.CreateAlways && importToTypeInfo.GetKeyMember() == null)
                throw new UserFriendlyException(
                    $"{Application.Model.BOModel.GetClass(importToTypeInfo.Type)} DefaultMember is not set, please use the {nameof(ImportStrategy.CreateAlways)} strategy instead.");

            var progressBarViewItem = View.GetItems<ProgressViewItem>().First();
            progressBarViewItem.Start();
            var progressObserver = GetProgressObserver(ExcelImport,progressBarViewItem);
            ObjectSpace.SetModified(View.CurrentObject);
            Observable.Start(async () => {
                var space = Application.CreateObjectSpace(ExcelImport.GetType());
                var excelImport = space.GetObjectByKey<ExcelImport>(ExcelImport.Oid);
                excelImport.Import(ExcelImport.File.Content, progressObserver);
                await ObjectSpace.WhenCommiting().FirstAsync();
                return space;
            })
            .SelectMany(task => task)
            .Subscribe(space => {
                    space.CommitChanges();
                    space.Dispose();
                });
        }

        protected virtual IObserver<ImportProgress> GetProgressObserver(ExcelImport excelImport,ProgressViewItem progressBarViewItem) {
            var progress = Subject.Synchronize(new Subject<ImportProgress>());
            var resultMessage = (CaptionHelper.GetLocalizedText(ExcelImporterLocalizationUpdater.ExcelImport,
                    ExcelImporterLocalizationUpdater.ImportSucceded),CaptionHelper.GetLocalizedText(ExcelImporterLocalizationUpdater.ExcelImport,
                ExcelImporterLocalizationUpdater.ImportFailed));
            var dataRowProgress = progress.OfType<ImportDataRowProgress>().Where(excelImport);
            
            var progressEnd = ProgressEnd(excelImport, progressBarViewItem, progress, resultMessage);
            Observable
                .Interval(TimeSpan.FromMilliseconds(progressBarViewItem.PollingInterval))
                .WithLatestFrom(dataRowProgress, (l, importProgress) => ( importProgress.Percentage))
                .Select(Synchronise).Concat( )
                .Finally(() => OnSetPosition(progressBarViewItem, 0))
                .TakeUntil(progressEnd)
                .Subscribe(percentage => OnSetPosition(progressBarViewItem, percentage),exception => {} );
            return progress;
        }

        protected  virtual IObservable<T> Synchronise<T>(T i) {
            return Observable.Return(i);
        }

        protected virtual IObservable<Unit> ProgressEnd(ExcelImport excelImport, ProgressViewItem progressBarViewItem,
            ISubject<ImportProgress> progress, (string successMsg, string failedMsg) resultMessage) {
            Terminator.OnNext(Unit.Default);
            return progress.OfType<ImportProgressComplete>().Where(excelImport).FirstAsync()
                .Select(Synchronise).Concat()
                .Do(_ => progressBarViewItem.SetFinishOptions(GetFinishOptions(_, resultMessage)))
                .ToUnit()
                .Merge(Terminator);
        }

        protected virtual void OnSetPosition(ProgressViewItem progressBarViewItem, int percentage){
            progressBarViewItem.SetPosition ( percentage);
        }

        protected virtual MessageOptions GetFinishOptions(ImportProgressComplete progressComplete,
            (string successMsg, string failedMsg) resultMessage){
            var failedResults = ExcelImport.FailedResultList.FailedResults;
            string message;
            var informationType=InformationType.Success;
            if (failedResults.Any()){
                informationType = InformationType.Error;
                message = string.Format(resultMessage.successMsg, failedResults.GroupBy(r => r.Index).Count(), progressComplete.FailedRecordsCount);
            }
            else{
                message =string.Format(resultMessage.successMsg, progressComplete.TotalRecordsCount);
            }
            var messageOptions = new MessageOptions {
                Message = message,
                Type = informationType,
                Duration = Int32.MaxValue,
                Win = {Type = WinMessageType.Alert},
                Web = {Position = InformationPosition.Top, CanCloseOnOutsideClick = false, CanCloseOnClick = true}
            };
            return messageOptions;
        }

        public SimpleAction ImportAction{ get;  }
    }
}