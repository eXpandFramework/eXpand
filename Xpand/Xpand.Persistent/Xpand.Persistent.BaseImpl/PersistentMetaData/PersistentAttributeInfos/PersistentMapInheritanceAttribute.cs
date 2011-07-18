using System.ComponentModel;
using DevExpress.Xpo;
using Xpand.Persistent.Base.PersistentMetaData.PersistentAttributeInfos;

namespace Xpand.Persistent.BaseImpl.PersistentMetaData.PersistentAttributeInfos {
    [DefaultProperty("MapInheritanceType")]
    public class PersistentMapInheritanceAttribute : PersistentAttributeInfo {
        MapInheritanceType _mapInheritanceType;

        public PersistentMapInheritanceAttribute(Session session) : base(session) {
        }

        public MapInheritanceType MapInheritanceType {
            get { return _mapInheritanceType; }
            set { SetPropertyValue("MapInheritanceType", ref _mapInheritanceType, value); }
        }

        public override AttributeInfoAttribute Create() {
            return
                new AttributeInfoAttribute(typeof (MapInheritanceAttribute).GetConstructor(new[] {typeof (MapInheritanceType)}),
                                  MapInheritanceType);
        }
    }
}