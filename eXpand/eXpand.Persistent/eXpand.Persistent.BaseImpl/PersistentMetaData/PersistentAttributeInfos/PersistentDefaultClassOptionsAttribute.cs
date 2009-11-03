using System;
using System.ComponentModel;
using DevExpress.Persistent.Base;
using DevExpress.Xpo;
using eXpand.Persistent.Base.PersistentMetaData.PersistentAttributeInfos;

namespace eXpand.Persistent.BaseImpl.PersistentMetaData.PersistentAttributeInfos {
    [DefaultProperty("DefaultClassOptionsName")]
    public class PersistentDefaultClassOptionsAttribute :PersistentAttributeInfo
    {
        public PersistentDefaultClassOptionsAttribute(Session session) : base(session) {
        }

        [Browsable(false)]
        [MemberDesignTimeVisibility(false)]
        public string DefaultClassOptionsName
        {
            get { return "DefaultClassOptions"; }
        }
        
        public override AttributeInfo Create() {
            return new AttributeInfo(typeof(DefaultClassOptionsAttribute).GetConstructor(new Type[0]));
        }
    }
}