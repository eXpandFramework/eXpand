using DevExpress.ExpressApp;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Model;
using DevExpress.Persistent.Base;
using Xpand.ExpressApp.ListEditors;
using Xpand.Persistent.Base.General;
using Xpand.Utils.Helpers;
using System.Linq;

namespace Xpand.ExpressApp {
    public class XpandListView : ListView {
        
        public XpandListView(CollectionSourceBase collectionSource, XafApplication application, bool isRoot) : base(collectionSource, application, isRoot) {
            UpdateLayoutManager(application.TypesInfo);
        }

        void UpdateLayoutManager(ITypesInfo typesInfo) {
            if (!(LayoutManager is ILayoutManager)) {
                var typeInfo = ReflectionHelper.FindTypeDescendants(typesInfo.FindTypeInfo(typeof(ILayoutManager))).FirstOrDefault();
                if (typeInfo != null)
                    this.SetPropertyInfoBackingFieldValue(view => view.LayoutManager, this, ReflectionHelper.CreateObject(typeInfo.Type));
            }
        }

        public XpandListView(IModelListView modelListView, CollectionSourceBase collectionSource, XafApplication application, bool isRoot) : base(modelListView, collectionSource, application, isRoot) {
            UpdateLayoutManager(application.TypesInfo);
        }

        public XpandListView(CollectionSourceBase collectionSource, ListEditor listEditor, bool isRoot, XafApplication application) : base(collectionSource, listEditor, isRoot, application) {
            UpdateLayoutManager(application.TypesInfo);
        }

        public XpandListView(CollectionSourceBase collectionSource, ListEditor listEditor, bool isRoot) : base(collectionSource, listEditor, isRoot) {
            UpdateLayoutManager(Application.TypesInfo);
        }

        public XpandListView(CollectionSourceBase collectionSource, ListEditor listEditor) : base(collectionSource, listEditor) {
            UpdateLayoutManager(Application.TypesInfo);
        }
        protected override void OnControlsCreated() {
            base.OnControlsCreated();
            var xpandEditor = Editor as IXpandListEditor;
            if (xpandEditor != null)
                xpandEditor.NotifyViewControlsCreated(this);
        }
    }
}