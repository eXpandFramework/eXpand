using System.Collections.Generic;
using System.ComponentModel;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.BaseImpl;
using DevExpress.Persistent.Validation;
using DevExpress.Xpo;
using eXpand.Persistent.Base.PersistentMetaData;
using eXpand.Persistent.Base.PersistentMetaData.PersistentAttributeInfos;
using eXpand.Persistent.BaseImpl.PersistentMetaData.PersistentAttributeInfos;

namespace eXpand.Persistent.BaseImpl.PersistentMetaData
{

    public abstract class PersistentTypeInfo : BaseObject, IPersistentTypeInfo, INamePrefix {
        
        protected PersistentTypeInfo(Session session) : base(session) { }

        
        
        string _name;

        [Browsable(false)]
        [MemberDesignTimeVisibility(false)]
        public string NamePrefix
        {
            get { return null; }
        }
        
        [RuleRequiredField(null,DefaultContexts.Save)]
        public string Name {
            get { return _name; }
            set { SetPropertyValue("Name", ref _name, value); }
        }

        IList<IPersistentAttributeInfo> IPersistentTypeInfo.TypeAttributes {
            get {
                return new ListConverter<IPersistentAttributeInfo, PersistentAttributeInfo>(TypeAttributes);
            }
        }

        [Association("TypeAttributes")][Aggregated]
        public XPCollection<PersistentAttributeInfo> TypeAttributes {
            get { return GetCollection<PersistentAttributeInfo>("TypeAttributes"); }
        }

    }
}