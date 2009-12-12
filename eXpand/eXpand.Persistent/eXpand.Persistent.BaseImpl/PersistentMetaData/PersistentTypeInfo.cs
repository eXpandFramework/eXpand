using System;
using System.Collections.Generic;
using DevExpress.ExpressApp.SystemModule;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.BaseImpl;
using DevExpress.Persistent.Validation;
using DevExpress.Xpo;
using eXpand.Persistent.Base.PersistentMetaData;
using eXpand.Persistent.Base.PersistentMetaData.PersistentAttributeInfos;
using eXpand.Persistent.BaseImpl.PersistentMetaData.PersistentAttributeInfos;

namespace eXpand.Persistent.BaseImpl.PersistentMetaData {

    public abstract class PersistentTypeInfo : BaseObject, IPersistentTypeInfo {
        public event EventHandler<ObjectCreatedEventArgs> AfterConstructed;

        public void InvokeAfterConstructed(ObjectCreatedEventArgs e) {
            EventHandler<ObjectCreatedEventArgs> handler = AfterConstructed;
            if (handler != null) handler(this, e);
        }

        string _name;
        public override void AfterConstruction()
        {
            base.AfterConstruction();
            InvokeAfterConstructed(new ObjectCreatedEventArgs(this, null));
        }
        protected PersistentTypeInfo(Session session) : base(session) {
        }
        
        [Association("TypeAttributes")]
        [Aggregated]
        public XPCollection<PersistentAttributeInfo> TypeAttributes {
            get { return GetCollection<PersistentAttributeInfo>("TypeAttributes"); }
        }
        #region IPersistentTypeInfo Members
        
        [RuleRequiredField(null, DefaultContexts.Save)]
        public string Name {
            get { return _name; }
            set { SetPropertyValue("Name", ref _name, value); }
        }

        IList<IPersistentAttributeInfo> IPersistentTypeInfo.TypeAttributes {
            get { return new ListConverter<IPersistentAttributeInfo, PersistentAttributeInfo>(TypeAttributes); }
        }


                #endregion
    }

}