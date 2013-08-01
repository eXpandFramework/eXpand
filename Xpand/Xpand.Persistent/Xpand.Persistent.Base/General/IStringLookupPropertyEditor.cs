using System;
using System.ComponentModel;
using DevExpress.ExpressApp;

namespace Xpand.Persistent.Base.General {
    public interface IObjectSpaceHolder {
        IObjectSpace ObjectSpace { get; }
    }

    public interface IStringLookupPropertyEditor : IObjectSpaceHolder {
        event EventHandler<HandledEventArgs> ItemsCalculating;
    }
}