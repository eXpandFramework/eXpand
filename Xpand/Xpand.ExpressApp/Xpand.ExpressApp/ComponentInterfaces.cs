using System;
using System.ComponentModel;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Utils;
using DevExpress.Xpo.DB;

namespace Xpand.ExpressApp {
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

    internal interface IWinApplication : IXafApplication {
        void LogOff();
    }

    public interface ISettingsStorage {
        SettingsStorage CreateLogonParameterStoreCore();
    }

    public interface IConfirmationRequired {
        event CancelEventHandler ConfirmationRequired;
    }



    internal interface IXafApplication : ISettingsStorage, IConfirmationRequired {
        string ConnectionString { get; set; }
        string ModelAssemblyFilePath { get; }
        event EventHandler UserDifferencesLoaded;
        IDataStore GetDataStore(IDataStore dataStore);
        string RaiseEstablishingConnection();
        ApplicationModulesManager ApplicationModulesManager { get; }

        void WriteLastLogonParameters(DetailView view, object logonObject);
        event EventHandler<ViewShownEventArgs> AfterViewShown;
        void OnAfterViewShown(Frame frame, Frame sourceFrame);
    }






}
