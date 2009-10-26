using System.Collections.Generic;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.BaseImpl;
using DevExpress.Persistent.Validation;
using DevExpress.Xpo;
using eXpand.Persistent.Base.PersistentMetaData;

namespace eXpand.Persistent.BaseImpl.PersistentMetaData
{
    public abstract class PersistentTypeInfo : BaseObject, IPersistentTypeInfo {
        
        protected PersistentTypeInfo(Session session) : base(session) { }

        
        
        string _name;
        [RuleRequiredField(null,DefaultContexts.Save)]
        public string Name {
            get { return _name; }
            set { SetPropertyValue("Name", ref _name, value); }
        }

        IList<IPersistentAttributeInfo> IPersistentTypeInfo.TypeAttributes {
            get { return new ListConverter<IPersistentAttributeInfo,PersistentAttributeInfo>(TypeAttributes); }
        }
        
        [Association]
        public XPCollection<PersistentAttributeInfo> TypeAttributes {
            get { return GetCollection<PersistentAttributeInfo>("TypeAttributes"); }
        }

    }
}