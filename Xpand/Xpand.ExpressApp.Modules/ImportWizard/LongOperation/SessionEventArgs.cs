using System;
using DevExpress.Xpo;

namespace Xpand.ExpressApp.ImportWizard.LongOperation {
    public class SessionEventArgs : EventArgs {
        public SessionEventArgs(UnitOfWork session) {
            Session = session;
        }

        public UnitOfWork Session { get; private set; }
    }
}