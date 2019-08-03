using System;
using System.ComponentModel;
using System.Reactive;
using System.Reactive.Linq;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.ConditionalAppearance;
using DevExpress.ExpressApp.Model.Core;
using DevExpress.ExpressApp.Notifications;
using DevExpress.ExpressApp.Validation;
using DevExpress.Utils;
using Xpand.ExpressApp.ExcelImporter.BusinessObjects;
using Xpand.ExpressApp.ExcelImporter.Services;
using Xpand.ExpressApp.SystemModule;
using Xpand.ExpressApp.Validation;
using Xpand.Persistent.Base.General;
using Xpand.Persistent.Base.ModelAdapter;
using Xpand.XAF.Modules.AutoCommit;
using Xpand.XAF.Modules.CloneModelView;
using Xpand.XAF.Modules.HideToolBar;
using Xpand.XAF.Modules.MasterDetail;
using Xpand.XAF.Modules.ProgressBarViewItem;
using Xpand.XAF.Modules.Reactive.Services;
using Xpand.XAF.Modules.SuppressConfirmation;
using Xpand.XAF.Modules.ViewEditMode;

namespace Xpand.ExpressApp.ExcelImporter {
    [ToolboxItem(true)]
    [ToolboxTabName(XpandAssemblyInfo.TabWinWebModules)]
    public sealed class ExcelImporterModule : XpandModuleBase {
        public const string ExcelImporter = "Xpand.ExcelImporter";
        private NotificationsModule _notificationsModule;

        public ExcelImporterModule() {
            RequiredModuleTypes.Add(typeof(DevExpress.ExpressApp.SystemModule.SystemModule));
            RequiredModuleTypes.Add(typeof(ConditionalAppearanceModule));
            RequiredModuleTypes.Add(typeof(NotificationsModule));
            RequiredModuleTypes.Add(typeof(ValidationModule));
            RequiredModuleTypes.Add(typeof(XpandSystemModule));
            RequiredModuleTypes.Add(typeof(XpandValidationModule));
            RequiredModuleTypes.Add(typeof(MasterDetailModule));
            RequiredModuleTypes.Add(typeof(AutoCommitModule));
            RequiredModuleTypes.Add(typeof(ViewEditModeModule));
            RequiredModuleTypes.Add(typeof(SuppressConfirmationModule));
            RequiredModuleTypes.Add(typeof(CloneModelViewModule));
            RequiredModuleTypes.Add(typeof(HideToolBarModule));
            RequiredModuleTypes.Add(typeof(ProgressBarViewItemModule));
        }

        public override void Setup(ApplicationModulesManager moduleManager) {
            base.Setup(moduleManager);
            if (InterfaceBuilder.RuntimeMode) {
                _notificationsModule = Application.FindModule<NotificationsModule>();
                _notificationsModule.NotificationsRefreshInterval = TimeSpan.FromSeconds(5);
                var excelImportDetailView = Application.WhenDetailViewCreated().ToDetailView()
                    .When(typeof(ExcelImport));
                Application.WhenMasterDetailDashboardViewItems().CombineLatest(excelImportDetailView,(tuple, view) =>
                        (tuple.listViewItem,excelImport:(ExcelImport)view.CurrentObject))
                    .Select(_ => {
                        var listView = ((ListView) _.listViewItem.InnerView);
                        var criteriaOperator = listView.ObjectSpace.GetCriteriaOperator<ExcelColumnMap>(map =>map.ExcelImport.Oid == _.excelImport.Oid);
                        listView.CollectionSource.Criteria[GetType().Name] =criteriaOperator;
                        return Unit.Default;
                    })
                    .Subscribe();
            }
        }

        public override void AddGeneratorUpdaters(ModelNodesGeneratorUpdaters updaters) {
            base.AddGeneratorUpdaters(updaters);
            updaters.Add(new ExcelImporterLocalizationUpdater());
        }
    }
}