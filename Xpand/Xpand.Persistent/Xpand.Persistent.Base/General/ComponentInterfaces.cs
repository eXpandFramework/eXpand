using System;
using System.ComponentModel;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Utils;
using DevExpress.Xpo.DB;

namespace Xpand.Persistent.Base.General {
    [Obsolete("", true)]
    public interface ISettingsStorage {
        SettingsStorage CreateLogonParameterStoreCore();
    }

    public class CreatingListEditorEventArgs : HandledEventArgs {
        readonly IModelListView _modelListView;
        readonly CollectionSourceBase _collectionSource;

        public CreatingListEditorEventArgs(IModelListView modelListView, CollectionSourceBase collectionSource) {
            _modelListView = modelListView;
            _collectionSource = collectionSource;
        }

        public IModelListView ModelListView {
            get { return _modelListView; }
        }

        public CollectionSourceBase CollectionSource {
            get { return _collectionSource; }
        }

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

    public interface IConfirmationRequired {
        event CancelEventHandler ConfirmationRequired;
    }


    public interface IXafApplicationDataStore {
        IDataStore GetDataStore(IDataStore dataStore);
    }

    public interface IXafApplicationDirectory {
        string BinDirectory { get;  }
    }
    public interface ILayoutManager {
    }

    public interface IAutoCreateOption {
        AutoCreateOption AutoCreateOption { get; }
    }

    public interface IWindowCreating {
        event EventHandler<WindowCreatingEventArgs> WindowCreating;
    }

    public interface IXafApplication : IConfirmationRequired, IXafApplicationDataStore, IAutoCreateOption, IWindowCreating {
        string ModelAssemblyFilePath { get; }
        ApplicationModulesManager ApplicationModulesManager { get; }
        void WriteLastLogonParameters(DetailView view, object logonObject);
    }

    public class WindowCreatingEventArgs : HandledEventArgs{
        public Window Window { get; set; }
    }
}
