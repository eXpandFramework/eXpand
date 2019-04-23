using System;
using System.Linq;
using System.Reactive.Linq;
using DevExpress.ExpressApp;
using DevExpress.Xpo;
using DevExpress.Xpo.Metadata.Helpers;
using Xpand.ExpressApp.ExcelImporter.BusinessObjects;
using Xpand.ExpressApp.ExcelImporter.Services;
using Xpand.Persistent.Base.General;
using Xpand.XAF.Modules.Reactive.Services.Controllers;

namespace Xpand.ExpressApp.ExcelImporter.Controllers {
    public class DeferredDeletionController:ObjectViewController<DetailView,ExcelImport> {
        protected override void OnActivated() {
            base.OnActivated();
            Frame.GetController<ExcelImportDetailViewController>().BeginImport
                .SelectMany(_ => _.progress.OfType<RequestImportTargetObject>().Where(_.excelImport))
                .Select(_ => {
                    if (_.Args.objectType.GetTypeInfo().FindAttributes<DeferredDeletionAttribute>().Any()) {
                        var session = _.ObjectSpace.Session();
                        _.TargetObject = session.FindObject(_.Args.objectType, _.Args.criteria,true);
                        if (_.TargetObject != null) {
                            var memberInfo = session.GetClassInfo(_.TargetObject).FindMember(GCRecordField.StaticName);
                            memberInfo.SetValue(_.TargetObject,null);
                        }
                    }
                    
                    return _;

                })
                .TakeUntil(this.WhenDeactivated())
                .Subscribe();
        }
    }
}
