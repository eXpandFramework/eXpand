using System.ComponentModel;
using DevExpress.Xpo;
using eXpand.Persistent.Base.PersistentMetaData.PersistentAttributeInfos;

namespace eXpand.Persistent.BaseImpl.PersistentMetaData.PersistentAttributeInfos {
    [DefaultProperty("MapInheritanceType")]
    public class PersistentMapInheritanceAttribute : PersistentAttributeInfo {
        MapInheritanceType _mapInheritanceType;

        public PersistentMapInheritanceAttribute(Session session) : base(session) {
        }

        public MapInheritanceType MapInheritanceType {
            get { return _mapInheritanceType; }
            set { SetPropertyValue("MapInheritanceType", ref _mapInheritanceType, value); }
        }

        public override AttributeInfo Create() {
            return
                new AttributeInfo(typeof (MapInheritanceAttribute).GetConstructor(new[] {typeof (MapInheritanceType)}),
                                  MapInheritanceType);
        }
    }
}