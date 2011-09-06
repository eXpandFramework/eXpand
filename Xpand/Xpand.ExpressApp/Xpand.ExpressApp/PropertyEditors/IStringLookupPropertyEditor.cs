using System;
using System.ComponentModel;

namespace Xpand.ExpressApp.PropertyEditors {
    public interface IStringLookupPropertyEditor {
        event EventHandler<HandledEventArgs> ItemsCalculating;
    }
}