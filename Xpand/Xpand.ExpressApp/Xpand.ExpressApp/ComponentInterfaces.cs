using System;
using System.ComponentModel;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Utils;
using DevExpress.Xpo.DB;

namespace Xpand.ExpressApp {
    internal interface ISupportCustomListEditorCreation {
        event EventHandler<CreatingListEditorEventArgs> CustomCreateListEditor;
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

    internal interface IWinApplication : IXafApplication {
        void LogOff();
    }

    internal interface IXafApplication {
        IDataStore GetDataStore(IDataStore dataStore);
        string RaiseEstablishingConnection();
        ApplicationModulesManager ApplicationModulesManager { get; }
        event EventHandler UserDifferencesLoaded;
    }

    internal interface ISupportFullConnectionString {
        string ConnectionString { get; set; }
    }

    internal interface ISupportLogonParameterStore {
        SettingsStorage CreateLogonParameterStoreCore();
        void WriteLastLogonParameters(DetailView view, object logonObject);
    }


    internal interface ISupportConfirmationRequired {
        event CancelEventHandler ConfirmationRequired;
    }

    internal interface ISupportAfterViewShown {
        event EventHandler<ViewShownEventArgs> AfterViewShown;
        void OnAfterViewShown(Frame frame, Frame sourceFrame);
    }

}
