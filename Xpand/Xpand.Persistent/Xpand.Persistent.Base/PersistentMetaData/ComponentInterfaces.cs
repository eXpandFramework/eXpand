using System;
using System.ComponentModel;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Utils;
using DevExpress.Xpo.DB;

namespace Xpand.Persistent.Base.PersistentMetaData {
    public interface ISettingsStorage {
        SettingsStorage CreateLogonParameterStoreCore();
    }
    public interface IConnectionString {
        string ConnectionString { get; set; }
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


    public interface IConfirmationRequired {
        event CancelEventHandler ConfirmationRequired;
    }


    public interface IXafApplicationDataStore {
        IDataStore GetDataStore(IDataStore dataStore);
    }

    public interface IXafApplication : IConfirmationRequired, IXafApplicationDataStore, IWorldCreatorModule {
        string ModelAssemblyFilePath { get; }
        event EventHandler UserDifferencesLoaded;
        string RaiseEstablishingConnection();
        ApplicationModulesManager ApplicationModulesManager { get; }
        AutoCreateOption AutoCreateOption { get; }
        void WriteLastLogonParameters(DetailView view, object logonObject);
        event EventHandler<ViewShownEventArgs> AfterViewShown;
        void OnAfterViewShown(Frame frame, Frame sourceFrame);
    }

}
