using System;
using System.ComponentModel;
using System.Drawing;
using System.Reactive.Linq;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.FileAttachments.Win;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Notifications;
using DevExpress.ExpressApp.Notifications.Win;
using DevExpress.ExpressApp.SystemModule;
using DevExpress.ExpressApp.Validation.Win;
using DevExpress.ExpressApp.Win.SystemModule;
using DevExpress.Utils;
using Xpand.ExpressApp.ExcelImporter.Win.BusinessObjects;
using Xpand.ExpressApp.ExcelImporter.Win.Services;
using Xpand.ExpressApp.Validation.Win;
using Xpand.ExpressApp.Win.SystemModule;
using Xpand.Persistent.Base.General;
using Xpand.XAF.Modules.Reactive;
using Xpand.XAF.Modules.Reactive.Extensions;
using Xpand.XAF.Modules.Reactive.Services;

namespace Xpand.ExpressApp.ExcelImporter.Win {
    [ToolboxBitmap(typeof(ExcelImporterWinModule))]
    [ToolboxItem(true)]
    [ToolboxTabName(XpandAssemblyInfo.TabWinModules)]
    public sealed class ExcelImporterWinModule : XpandModuleBase {
        public ExcelImporterWinModule() {
            RequiredModuleTypes.Add(typeof(SystemModule));
            RequiredModuleTypes.Add(typeof(FileAttachmentsWindowsFormsModule));
            RequiredModuleTypes.Add(typeof(XpandSystemWindowsFormsModule));
            RequiredModuleTypes.Add(typeof(ExcelImporterModule));
            RequiredModuleTypes.Add(typeof(XpandValidationWinModule));
            RequiredModuleTypes.Add(typeof(ValidationWindowsFormsModule));
            RequiredModuleTypes.Add(typeof(NotificationsModule));
            RequiredModuleTypes.Add(typeof(NotificationsWindowsFormsModule));
            RequiredModuleTypes.Add(typeof(SystemWindowsFormsModule));
            RequiredModuleTypes.Add(typeof(ReactiveModule));
        }

        public override void ExtendModelInterfaces(ModelInterfaceExtenders extenders) {
            base.ExtendModelInterfaces(extenders);
            extenders.Add<IModelOptions,IModelOptionsAutoImportConcurrencyLimit>();
        }

        public override void Setup(XafApplication application) {
            base.Setup(application);
            application.Connect()
                .TakeUntil(this.WhenDisposed())
                .Subscribe();
            application.WhenSetupComplete()
                .Do(_ => {
                    var notificationsModule = Application.Modules.FindModule<NotificationsModule>();
                    notificationsModule.DefaultNotificationsProvider.NotificationTypesInfo.Add(
                        application.TypesInfo.FindTypeInfo(typeof(ImportNotification)));
                })
                .Subscribe();
        }
    }
}