using System;
using System.ComponentModel;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Model;

namespace eXpand.ExpressApp {
    public interface ISupportCustomListEditorCreation
    {
        event EventHandler<CreatingListEditorEventArgs> CustomCreateListEditor;
    }
    public class CreatingListEditorEventArgs : HandledEventArgs
    {
        readonly IModelListView _modelListView;
        readonly CollectionSourceBase _collectionSource;

        public CreatingListEditorEventArgs(IModelListView modelListView, CollectionSourceBase collectionSource)
        {
            _modelListView = modelListView;
            _collectionSource = collectionSource;
        }

        public IModelListView ModelListView
        {
            get { return _modelListView; }
        }

        public CollectionSourceBase CollectionSource
        {
            get { return _collectionSource; }
        }

        public ListEditor ListEditor { get; set; }
    }

    public interface IWinApplication
    {
    }
    public interface ISupportModelsManager {
        ApplicationModelsManager ModelsManager { get; }
    }
}