using System;
using System.ComponentModel;
using DevExpress.Xpo;
using Xpand.Persistent.Base.PersistentMetaData;
using Xpand.Persistent.Base.PersistentMetaData.PersistentAttributeInfos;

namespace Xpand.Persistent.BaseImpl.PersistentMetaData.PersistentAttributeInfos {
    [DefaultProperty("AggregatedName")]
    [System.ComponentModel.DisplayName("Aggregated")]
    [CreateableItem(typeof(IPersistentMemberInfo))]
    [CreateableItem(typeof(IExtendedMemberInfo))]
    public class PersistentAggregatedAttribute : PersistentAttributeInfo {
        public PersistentAggregatedAttribute(Session session)
            : base(session) {
        }

        [Browsable(false)]
        [MemberDesignTimeVisibility(false)]
        public string AggregatedName {
            get { return "Aggregated"; }
        }

        public override AttributeInfoAttribute Create() {
            return new AttributeInfoAttribute(typeof(AggregatedAttribute).GetConstructor(new Type[0]), new object[0]);
        }
    }
}