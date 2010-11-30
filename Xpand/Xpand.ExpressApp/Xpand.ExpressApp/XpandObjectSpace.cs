using System;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.DC;
using DevExpress.Xpo;

using Xpand.Xpo;

namespace Xpand.ExpressApp {
    public class XpandObjectSpace : ObjectSpace {
        public XpandObjectSpace(UnitOfWork unitOfWork, ITypesInfo typesInfo)
            : base(unitOfWork, typesInfo) {
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
