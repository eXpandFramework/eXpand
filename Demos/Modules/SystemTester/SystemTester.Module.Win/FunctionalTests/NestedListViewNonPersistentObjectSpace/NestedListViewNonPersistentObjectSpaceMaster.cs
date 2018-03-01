using System.ComponentModel;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.DC;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.BaseImpl;
using DevExpress.Xpo;
using Xpand.Persistent.Base.General;
using Xpand.Persistent.Base.General.Controllers;

namespace SystemTester.Module.Win.FunctionalTests.NestedListViewNonPersistentObjectSpace{
    public class NestedListViewNonPersistentObjectSpaceController:MasterObjectViewController<NestedListViewNonPersistentObjectSpaceNested,NestedListViewNonPersistentObjectSpaceMaster>{
        private BindingList<NestedListViewNonPersistentObjectSpaceNested> _datasource=new BindingList<NestedListViewNonPersistentObjectSpaceNested>();

        protected override void UpdateMasterObject(NestedListViewNonPersistentObjectSpaceMaster masterObject){
            var nonPersistentObjectSpace = ((NonPersistentObjectSpace) ObjectSpace);
            nonPersistentObjectSpace.ObjectsGetting-=NonPersistentObjectSpaceOnObjectsGetting;
            nonPersistentObjectSpace.ObjectsGetting+=NonPersistentObjectSpaceOnObjectsGetting;
            _datasource.Clear();
            _datasource.Add(new NestedListViewNonPersistentObjectSpaceNested(){NestedName = masterObject.MasterName});
            View.CollectionSource.ResetCollection();
        }

        private void NonPersistentObjectSpaceOnObjectsGetting(object sender, ObjectsGettingEventArgs e){
            e.Objects=_datasource;        
        }
    }
    [XpandNavigationItem("NestedListViewNonPersistentObjectSpace/Master")]
    public class NestedListViewNonPersistentObjectSpaceMaster : BaseObject{
        private string _masterName;


        public NestedListViewNonPersistentObjectSpaceMaster(Session session) : base(session){
        }

        public string MasterName{
            get{ return _masterName; }
            set{ SetPropertyValue(nameof(MasterName), ref _masterName, value); }
        }

        NestedListViewNonPersistentObjectSpaceNestedList _list;
        [ExpandObjectMembers(ExpandObjectMembers.InDetailView)]
        [VisibleInListView(false)]
        [NonPersistent]
        public NestedListViewNonPersistentObjectSpaceNestedList List{
            get{ return _list; }
            set{ SetPropertyValue(nameof(List), ref _list, value); }
        }
    }

    [DomainComponent]
    public class NestedListViewNonPersistentObjectSpaceNestedList{
        public NestedListViewNonPersistentObjectSpaceNestedList(){
            Nesteds=new BindingList<NestedListViewNonPersistentObjectSpaceNested>();
        }

        public BindingList<NestedListViewNonPersistentObjectSpaceNested> Nesteds{ get; }
    }

    [DomainComponent]
    public class NestedListViewNonPersistentObjectSpaceNested{
        public string NestedName{ get; set; }
    }
}