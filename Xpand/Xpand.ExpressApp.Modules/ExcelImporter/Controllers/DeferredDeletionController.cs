using System;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using DevExpress.ExpressApp;
using DevExpress.Xpo;
using DevExpress.Xpo.Metadata.Helpers;
using Xpand.ExpressApp.ExcelImporter.BusinessObjects;
using Xpand.ExpressApp.ExcelImporter.Services;
using Xpand.Persistent.Base.General;

namespace Xpand.ExpressApp.ExcelImporter.Controllers {
    public class DeferredDeletionService {
        
    }
    public class DeferredDeletionController:ObjectViewController<DetailView,ExcelImport> {
        private readonly Subject<Unit> _terminator=new Subject<Unit>();
        protected override void OnDeactivated() {
            base.OnDeactivated();
            _terminator.OnNext(Unit.Default);
        }

        protected override void OnActivated() {
            base.OnActivated();
            var excelImportDetailViewController = Frame.GetController<ExcelImportDetailViewController>();
            excelImportDetailViewController.BeginImport
                .TakeUntil(_terminator)
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

                .Subscribe();
        }
    }
}
