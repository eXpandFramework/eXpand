using System;
using System.ComponentModel;
using DevExpress.Xpo;
using eXpand.Persistent.Base.PersistentMetaData.PersistentAttributeInfos;

namespace eXpand.Persistent.BaseImpl.PersistentMetaData.PersistentAttributeInfos {
    [DefaultProperty("AggregatedName")]
    public class PersistentAggregatedAttribute :PersistentAttributeInfo
    {
        public PersistentAggregatedAttribute(Session session) : base(session) {
        }

        public PersistentAggregatedAttribute() {
        }
        [Browsable(false)][MemberDesignTimeVisibility(false)]
        public string AggregatedName
        {
            get { return "Aggregated"; }
        }
        
        public override AttributeInfo Create() {
            return new AttributeInfo(typeof(AggregatedAttribute).GetConstructor(new Type[0]),new object[0]);
        }
    }
}