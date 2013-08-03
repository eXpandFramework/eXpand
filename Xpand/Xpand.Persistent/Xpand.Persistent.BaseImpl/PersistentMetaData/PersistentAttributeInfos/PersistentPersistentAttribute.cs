using System.ComponentModel;
using DevExpress.Xpo;
using Xpand.Persistent.Base.General.Controllers;
using Xpand.Persistent.Base.PersistentMetaData;
using Xpand.Persistent.Base.PersistentMetaData.PersistentAttributeInfos;

namespace Xpand.Persistent.BaseImpl.PersistentMetaData.PersistentAttributeInfos {
    [InterfaceRegistrator(typeof(IPersistentPersistentAttribute))]
    [DefaultProperty("MapTo")]
    [System.ComponentModel.DisplayName("Persistent")]
    [CreateableItem(typeof(IPersistentMemberInfo))]
    [CreateableItem(typeof(IPersistentClassInfo))]
    [CreateableItem(typeof(IExtendedMemberInfo))]
    public class PersistentPersistentAttribute : PersistentAttributeInfo, IPersistentPersistentAttribute {
        public PersistentPersistentAttribute(Session session)
            : base(session) {
        }
        private string _mapTo;

        public string MapTo {
            get { return _mapTo; }
            set { SetPropertyValue("MapTo", ref _mapTo, value); }
        }
        public override AttributeInfoAttribute Create() {
            return
                new AttributeInfoAttribute(typeof(PersistentAttribute).GetConstructor(new[] { typeof(string) }),
                                  MapTo);
        }
    }
}
