using System;
using DevExpress.ExpressApp;

namespace Xpand.Persistent.Base.General.Controllers {
    public abstract class MasterObjectViewController<TNestedObject, TMasterObject> : ViewController<ListView> {
        protected MasterObjectViewController() {
            TargetViewNesting = Nesting.Nested;
            TargetObjectType = typeof(TNestedObject);
        }
        protected override void OnActivated() {
            base.OnActivated();
            var collectionSource = View.CollectionSource as PropertyCollectionSource;
            if (collectionSource != null) {
                collectionSource.MasterObjectChanged += OnMasterObjectChanged;
                if (collectionSource.MasterObject != null)
                    UpdateMasterObject((TMasterObject) collectionSource.MasterObject);
            }
        }

        protected abstract void UpdateMasterObject(TMasterObject masterObject);

        void OnMasterObjectChanged(object sender, EventArgs e) {
            
            UpdateMasterObject((TMasterObject) ((PropertyCollectionSource)sender).MasterObject);
        }
        protected override void OnDeactivated() {
            var collectionSource = View.CollectionSource as PropertyCollectionSource;
            if (collectionSource != null) {
                collectionSource.MasterObjectChanged -= OnMasterObjectChanged;
            }
            base.OnDeactivated();
        }
    }
}