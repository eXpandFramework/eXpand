using System;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.DC;
using DevExpress.Xpo;
using Xpand.ExpressApp.Attributes;
using Xpand.Xpo;

namespace Xpand.ExpressApp {
    public class XpandObjectSpace : ObjectSpace {
        public XpandObjectSpace(UnitOfWork unitOfWork, ITypesInfo typesInfo)
            : base(unitOfWork, typesInfo) {
        }

        protected override void SetModified(object obj, ObjectChangedEventArgs args) {
            if (session.GetClassInfo(args.Object).FindMember(args.PropertyName) is ISupportCancelModification)
                return;
            base.SetModified(obj, args);
        }
        public new Action<ResolveSessionEventArgs> AsyncServerModeSourceDismissSession {
            get { return base.AsyncServerModeSourceDismissSession; }
            set { base.AsyncServerModeSourceDismissSession = value; }
        }
        public new Action<ResolveSessionEventArgs> AsyncServerModeSourceResolveSession {
            get { return base.AsyncServerModeSourceResolveSession; }
            set { base.AsyncServerModeSourceResolveSession = value; }
        }

        protected override UnitOfWork CreateUnitOfWork(IDataLayer dataLayer) {
            return new XpandUnitOfWork(dataLayer);
        }
    }
}
