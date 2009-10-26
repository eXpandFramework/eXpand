using System;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.BaseImpl;
using DevExpress.Xpo;
using eXpand.Persistent.Base.PersistentMetaData;

namespace eXpand.Persistent.BaseImpl.PersistentMetaData
{
    public abstract class PersistentAttributeInfo : BaseObject, IPersistentAttributeInfo {
        protected PersistentAttributeInfo(Session session) : base(session) { }

        protected PersistentAttributeInfo()
        {
        }
        [Persistent]
        [VisibleInDetailView(false)]
        public string Name
        {
            get { return ToString(); }
        }
        
        public abstract Attribute Create();

        IPersistentTypeInfo IPersistentAttributeInfo.Owner {
            get { return Owner; }
            set { Owner = value as PersistentTypeInfo; }
        }
        PersistentTypeInfo _owner;

        [Association]
        [VisibleInDetailView(false)]
        [VisibleInListView(false)]
        [VisibleInLookupListView(false)]
        public PersistentTypeInfo Owner
        {
            get { return _owner; }
            set { SetPropertyValue("Owner", ref _owner, value); }
        }
    }
}