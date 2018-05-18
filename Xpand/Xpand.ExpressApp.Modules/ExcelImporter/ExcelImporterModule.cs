using System;
using DevExpress.ExpressApp.DC;
using System.ComponentModel;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Model.Core;
using DevExpress.ExpressApp.Notifications;
using DevExpress.ExpressApp.Xpo;
using DevExpress.Utils;
using Xpand.ExpressApp.ExcelImporter.Services;
using Xpand.Persistent.Base.General;
using Xpand.Persistent.Base.ModelAdapter;

namespace Xpand.ExpressApp.ExcelImporter{
    [ToolboxItem(true)]
    [ToolboxTabName(XpandAssemblyInfo.TabWinWebModules)]
    public sealed partial class ExcelImporterModule : XpandModuleBase{
        private NotificationsModule _notificationsModule;

        public ExcelImporterModule(){
            InitializeComponent();
        }

        public override void CustomizeTypesInfo(ITypesInfo typesInfo){
            base.CustomizeTypesInfo(typesInfo);
            CalculatedPersistentAliasHelper.CustomizeTypesInfo(typesInfo);
        }

        public override void Setup(ApplicationModulesManager moduleManager){
            base.Setup(moduleManager);
            if (InterfaceBuilder.RuntimeMode){
                _notificationsModule = Application.FindModule<NotificationsModule>();
                _notificationsModule.NotificationsRefreshInterval = TimeSpan.FromSeconds(5);
            }
        }

        public override void AddGeneratorUpdaters(ModelNodesGeneratorUpdaters updaters){
            base.AddGeneratorUpdaters(updaters);
            updaters.Add(new ExcelImporterLocalizationUpdater());
        }
    }
}