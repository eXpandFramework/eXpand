using System;
using System.ComponentModel;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.ConditionalAppearance;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Model.Core;
using DevExpress.ExpressApp.Notifications;
using DevExpress.ExpressApp.Validation;
using DevExpress.ExpressApp.Xpo;
using DevExpress.Utils;
using Xpand.ExpressApp.ExcelImporter.Services;
using Xpand.ExpressApp.SystemModule;
using Xpand.ExpressApp.Validation;
using Xpand.Persistent.Base.General;
using Xpand.Persistent.Base.ModelAdapter;
using Xpand.XAF.Modules.AutoCommit;
using Xpand.XAF.Modules.CloneModelView;
using Xpand.XAF.Modules.HideToolBar;
using Xpand.XAF.Modules.MasterDetail;
using Xpand.XAF.Modules.SuppressConfirmation;
using Xpand.XAF.Modules.ViewEditMode;

namespace Xpand.ExpressApp.ExcelImporter {
    [ToolboxItem(true)]
    [ToolboxTabName(XpandAssemblyInfo.TabWinWebModules)]
    public sealed class ExcelImporterModule : XpandModuleBase {
        private NotificationsModule _notificationsModule;

        public ExcelImporterModule() {
            RequiredModuleTypes.Add(typeof(DevExpress.ExpressApp.SystemModule.SystemModule));
            RequiredModuleTypes.Add(typeof(ConditionalAppearanceModule));
            RequiredModuleTypes.Add(typeof(NotificationsModule));
            RequiredModuleTypes.Add(typeof(XpandSystemModule));
            RequiredModuleTypes.Add(typeof(ValidationModule));
            RequiredModuleTypes.Add(typeof(XpandValidationModule));
            RequiredModuleTypes.Add(typeof(MasterDetailModule));
            RequiredModuleTypes.Add(typeof(AutoCommitModule));
            RequiredModuleTypes.Add(typeof(ViewEditModeModule));
            RequiredModuleTypes.Add(typeof(SuppressConfirmationModule));
            RequiredModuleTypes.Add(typeof(CloneModelViewModule));
            RequiredModuleTypes.Add(typeof(HideToolBarModule));
            RequiredModuleTypes.Add(typeof(Xpand.XAF.Modules.ProgressBarViewItem.ProgressBarViewItemModule));
        }

        public override void CustomizeTypesInfo(ITypesInfo typesInfo) {
            base.CustomizeTypesInfo(typesInfo);
            CalculatedPersistentAliasHelper.CustomizeTypesInfo(typesInfo);
        }

        public override void Setup(ApplicationModulesManager moduleManager) {
            base.Setup(moduleManager);
            if (InterfaceBuilder.RuntimeMode) {
                _notificationsModule = Application.FindModule<NotificationsModule>();
                _notificationsModule.NotificationsRefreshInterval = TimeSpan.FromSeconds(5);
            }
        }

        public override void AddGeneratorUpdaters(ModelNodesGeneratorUpdaters updaters) {
            base.AddGeneratorUpdaters(updaters);
            updaters.Add(new ExcelImporterLocalizationUpdater());
        }
    }
}