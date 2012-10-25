using System;
using System.ComponentModel;
using DevExpress.ExpressApp;

namespace Xpand.ExpressApp.PropertyEditors {
    public interface IObjectSpaceHolder {
        IObjectSpace ObjectSpace { get; }
    }

    public interface IStringLookupPropertyEditor : IObjectSpaceHolder {
        event EventHandler<HandledEventArgs> ItemsCalculating;
    }
}