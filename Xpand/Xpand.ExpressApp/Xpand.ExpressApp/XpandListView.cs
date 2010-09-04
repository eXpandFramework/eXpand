using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Model;

namespace Xpand.ExpressApp {
    public class XpandListView:ListView {
        public XpandListView(CollectionSourceBase collectionSource, XafApplication application, bool isRoot) : base(collectionSource, application, isRoot) {
        }

        public XpandListView(IModelListView modelListView, CollectionSourceBase collectionSource, XafApplication application, bool isRoot) : base(modelListView, collectionSource, application, isRoot) {
        }

        public XpandListView(CollectionSourceBase collectionSource, ListEditor listEditor, bool isRoot, XafApplication application) : base(collectionSource, listEditor, isRoot, application) {
        }

        public XpandListView(CollectionSourceBase collectionSource, ListEditor listEditor, bool isRoot) : base(collectionSource, listEditor, isRoot) {
        }

        public XpandListView(CollectionSourceBase collectionSource, ListEditor listEditor) : base(collectionSource, listEditor) {
        }
    }
}