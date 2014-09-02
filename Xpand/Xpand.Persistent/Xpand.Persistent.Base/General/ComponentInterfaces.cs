﻿using System;
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

    public interface ILayoutManager {
    }

    public interface IWindowCreating {
        event EventHandler<WindowCreatingEventArgs> WindowCreating;
    }

    public interface IXafApplication : IConfirmationRequired,  IWindowCreating {
        string ModelAssemblyFilePath { get; }
        void WriteLastLogonParameters(DetailView view, object logonObject);
    }

    public class WindowCreatingEventArgs : HandledEventArgs{
        public Window Window { get; set; }
    }
}
