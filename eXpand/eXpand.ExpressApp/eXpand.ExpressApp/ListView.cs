using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Model;

namespace eXpand.ExpressApp {
    public class ListView:DevExpress.ExpressApp.ListView {
        public ListView(CollectionSourceBase collectionSource, XafApplication application, bool isRoot) : base(collectionSource, application, isRoot) {
        }

        public ListView(IModelListView modelListView, CollectionSourceBase collectionSource, XafApplication application, bool isRoot) : base(modelListView, collectionSource, application, isRoot) {
        }

        public ListView(CollectionSourceBase collectionSource, ListEditor listEditor, bool isRoot, XafApplication application) : base(collectionSource, listEditor, isRoot, application) {
        }

        public ListView(CollectionSourceBase collectionSource, ListEditor listEditor, bool isRoot) : base(collectionSource, listEditor, isRoot) {
        }

        public ListView(CollectionSourceBase collectionSource, ListEditor listEditor) : base(collectionSource, listEditor) {
        }
    }
}