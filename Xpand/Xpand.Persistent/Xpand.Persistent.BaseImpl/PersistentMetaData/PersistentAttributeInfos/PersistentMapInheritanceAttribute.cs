using System.ComponentModel;
using DevExpress.Xpo;
using Xpand.Persistent.Base.General.Controllers;
using Xpand.Persistent.Base.PersistentMetaData;
using Xpand.Persistent.Base.PersistentMetaData.PersistentAttributeInfos;

namespace Xpand.Persistent.BaseImpl.PersistentMetaData.PersistentAttributeInfos {
    [DefaultProperty("Map InheritanceType")]
    [System.ComponentModel.DisplayName("MapInheritance")]
    [CreateableItem(typeof(IPersistentClassInfo))]
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