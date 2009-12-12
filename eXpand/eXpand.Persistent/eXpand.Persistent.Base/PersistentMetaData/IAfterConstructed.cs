using System;
using DevExpress.ExpressApp.SystemModule;

namespace eXpand.Persistent.Base.PersistentMetaData {
    public interface IAfterConstructed {
        event EventHandler<ObjectCreatedEventArgs> AfterConstructed;
    }
}