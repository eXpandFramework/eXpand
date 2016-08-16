using System;
using System.ComponentModel;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Utils;

namespace Xpand.Persistent.Base.General {
    [Obsolete("", true)]
    public interface ISettingsStorage {
        SettingsStorage CreateLogonParameterStoreCore();
    }

    public class CreatingListEditorEventArgs : HandledEventArgs {
        public CreatingListEditorEventArgs(IModelListView modelListView, CollectionSourceBase collectionSource) {
            ModelListView = modelListView;
            CollectionSource = collectionSource;
        }

        public IModelListView ModelListView { get; }

        public CollectionSourceBase CollectionSource { get; }

        public ListEditor ListEditor { get; set; }
    }

    public interface IWinApplication : IXafApplication {
        void LogOff();
    }
    public interface IWebApplication : IXafApplication, IWriteSecuredLogonParameters {
        void LogOff();
    }

    public interface IWriteSecuredLogonParameters {
        event HandledEventHandler CustomWriteSecuredLogonParameters;
    }
    [Obsolete("not used",true)]
    public interface IConfirmationRequired {
        event CancelEventHandler ConfirmationRequired;
    }

    public interface ILayoutManager {
    }

    public interface IWindowCreating {
        event EventHandler<WindowCreatingEventArgs> WindowCreating;
    }

    public interface ITestXafApplication {
        
    }
    public interface IXafApplication :   IWindowCreating {
        string ModelAssemblyFilePath { get; }
        void WriteLastLogonParameters(DetailView view, object logonObject);
    }

    public class WindowCreatingEventArgs : HandledEventArgs{
        public Window Window { get; set; }
    }
}
