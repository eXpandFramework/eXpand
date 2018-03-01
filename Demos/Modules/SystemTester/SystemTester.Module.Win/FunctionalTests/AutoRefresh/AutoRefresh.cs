using System;
using DevExpress.ExpressApp;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.BaseImpl;
using DevExpress.Xpo;

namespace SystemTester.Module.Win.FunctionalTests.AutoRefresh{
    public class AutoRefreshController:ObjectViewController<ListView,AutoRefresh>{
        protected override void OnActivated(){
            base.OnActivated();
            View.CollectionSource.CollectionReloading+=CollectionSourceOnCollectionReloading;
            
        }

        private void CollectionSourceOnCollectionReloading(object sender, EventArgs eventArgs){
            var autoRefresh = ObjectSpace.CreateObject<AutoRefresh>();
            ObjectSpace.CommitChanges();
            View.CollectionSource.Add(autoRefresh);
        }

        protected override void OnDeactivated(){
            base.OnDeactivated();
            View.CollectionSource.CollectionReloading-=CollectionSourceOnCollectionReloading;
        }
    }
    [DefaultClassOptions]
    public class AutoRefresh:BaseObject{
        public AutoRefresh(Session session) : base(session){
        }

        string _name;

        public string Name{
            get{ return _name; }
            set{ SetPropertyValue(nameof(Name), ref _name, value); }
        }
    }

}