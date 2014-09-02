using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Model;
using Xpand.ExpressApp.ListEditors;
using Xpand.Persistent.Base.General;

namespace Xpand.ExpressApp {
    public class XpandListView : ListView {

        public XpandListView(CollectionSourceBase collectionSource, XafApplication application, bool isRoot)
            : base(collectionSource, application, isRoot) {
            this.UpdateLayoutManager();
        }

        public XpandListView(IModelListView modelListView, CollectionSourceBase collectionSource, XafApplication application, bool isRoot)
            : base(modelListView, collectionSource, application, isRoot) {
            this.UpdateLayoutManager();
        }

        public XpandListView(CollectionSourceBase collectionSource, ListEditor listEditor, bool isRoot, XafApplication application)
            : base(collectionSource, listEditor, isRoot, application) {
            this.UpdateLayoutManager();
        }

        public XpandListView(CollectionSourceBase collectionSource, ListEditor listEditor, bool isRoot)
            : base(collectionSource, listEditor, isRoot) {
            this.UpdateLayoutManager();
        }

        public XpandListView(CollectionSourceBase collectionSource, ListEditor listEditor)
            : base(collectionSource, listEditor) {
            this.UpdateLayoutManager();
        }
        protected override void OnControlsCreated() {
            base.OnControlsCreated();
            var xpandEditor = Editor as IXpandListEditor;
            if (xpandEditor != null)
                xpandEditor.NotifyViewControlsCreated(this);
        }

        public override IList SelectedObjects {
            get{
                if (Editor != null) {
                    return new List<Object>(Editor.GetSelectedObjects().OfType<object>()).Where(t => ObjectTypeInfo.Type.IsInstanceOfType(t)).ToArray();
                }
                return new ReadOnlyCollection<object>(new object[] { });
            }
        }
    }
}
