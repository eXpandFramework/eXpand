using System;
using DevExpress.ExpressApp;

namespace Xpand.ExpressApp.SystemModule {
    public abstract class MasterObjectViewController<TObject> : ViewController<XpandListView> where TObject : class {
        protected MasterObjectViewController() {
            TargetViewNesting = Nesting.Nested;
        }
        private TObject _masterObject;
        public TObject MasterObject {
            get { return _masterObject; }
        }
        protected override void OnActivated() {
            base.OnActivated();
            if (View.CollectionSource is PropertyCollectionSource) {
                var collectionSource = (PropertyCollectionSource)View.CollectionSource;
                collectionSource.MasterObjectChanged += collectionSource_MasterObjectChanged;
            }
        }
        void collectionSource_MasterObjectChanged(object sender, EventArgs e) {
            if (((PropertyCollectionSource)sender).MasterObject is TObject)
                OnMasterObjectChanged(sender);
        }

        protected virtual void OnMasterObjectChanged(object sender) {
            _masterObject = ((PropertyCollectionSource)sender).MasterObject as TObject;
        }
    }
}